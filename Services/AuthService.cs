using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using WorkHub.Api.Domain.Entities;
using WorkHub.Api.DTOs.Auth;
using WorkHub.Api.Repositories;
using WorkHub.Api.Repositories.Interfaces;
using WorkHub.Api.Security;
using WorkHub.Api.Services.Interfaces;

namespace WorkHub.Api.Services
{
    public class AuthService: IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration; 
        private readonly IHttpContextAccessor _httpContextAccessor; 
        private readonly LoginAttemptTracker _loginAttemptTracker; 
        private readonly IRefreshTokenRepository _refreshTokenRepository; 
        public AuthService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher, 
            IConfiguration configuration,IHttpContextAccessor httpContextAccessor,
            LoginAttemptTracker loginattemptTracker, IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _loginAttemptTracker = loginattemptTracker;
            _httpContextAccessor = httpContextAccessor;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request,string? ipAddress, CancellationToken cancellationToken)
        {
            var email = request.Email.Trim().ToLowerInvariant();           
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

            if (_loginAttemptTracker.IsBlocked(email, ipAddress))
            {
                throw new LoginRateLimitException();
            }

            if(user is null)
            {
                return null;
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                _loginAttemptTracker.RecordFailure(email, ipAddress);
                return null;
            }

            _loginAttemptTracker.Reset(email, ipAddress);

            var rawRefreshToken = RefreshTokenGenerator.Generate();

            var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(7);

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = RefreshTokenGenerator.Hash(rawRefreshToken),
                TokenFamilyId = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = refreshTokenExpiresAtUtc
            };

            await _refreshTokenRepository.CreateAsync(refreshToken,cancellationToken);

            var issuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("JWT issuer is missing.");

            var audience = _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException("JWT audience is missing.");

            var key = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT key is missing.");

            if (Encoding.UTF8.GetByteCount(key) < 32)
                throw new InvalidOperationException(
                    "JWT key must be at least 32 bytes.");

            var expiryMinutes = _configuration.GetValue<int>(
                "Jwt:ExpiryMinutes");

            if (expiryMinutes <= 0)
                throw new InvalidOperationException(
                    "JWT expiry must be greater than zero.");

            var expiresAtUtc = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role)
            };

            var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: credentials);

           

            return new LoginResponse
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                TokenType = "Bearer",
                RefreshToken = rawRefreshToken,
                RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc ,
                ExpiresAtUtc = expiresAtUtc
            };
        }


        public async Task<LoginResponse?> RefreshAsync(
            string refreshToken,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return null;

            var nowUtc = DateTime.UtcNow;
            var tokenHash = RefreshTokenGenerator.Hash(refreshToken);

            var storedToken = await _refreshTokenRepository.GetByHashAsync(
                tokenHash,
                cancellationToken);

            if (storedToken is null ||
                storedToken.RevokedAtUtc is not null ||
                storedToken.ExpiresAtUtc <= nowUtc ||
                !storedToken.User.IsActive)
            {
                return null;
            }

            var user = storedToken.User;

            // Generate the replacement refresh token.
            var newRawRefreshToken = RefreshTokenGenerator.Generate();
            var newRefreshTokenHash =
                RefreshTokenGenerator.Hash(newRawRefreshToken);

            var newRefreshTokenExpiresAtUtc = nowUtc.AddDays(7);

            var replacement = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = newRefreshTokenHash,
                TokenFamilyId = storedToken.TokenFamilyId,
                CreatedAtUtc = nowUtc,
                ExpiresAtUtc = newRefreshTokenExpiresAtUtc
            };

            // Atomically revoke the old token and store its replacement.
            var rotated = await _refreshTokenRepository.TryRotateAsync(
                storedToken.Id,
                replacement,
                nowUtc,
                cancellationToken);

            if (!rotated)
                return null;

            // Issue a fresh access token using the current user details.
            var accessTokenResult = CreateAccessToken(user);

            return new LoginResponse
            {
                AccessToken = accessTokenResult.Token,
                TokenType = "Bearer",
                ExpiresAtUtc = accessTokenResult.ExpiresAtUtc,
                RefreshToken = newRawRefreshToken,
                RefreshTokenExpiresAtUtc = newRefreshTokenExpiresAtUtc
            };
        }


        private (string Token, DateTime ExpiresAtUtc) CreateAccessToken(User user)
        {
            var issuer = _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException("JWT issuer is missing.");

            var audience = _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException("JWT audience is missing.");

            var key = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT key is missing.");

            if (Encoding.UTF8.GetByteCount(key) < 32)
                throw new InvalidOperationException(
                    "JWT key must be at least 32 bytes.");

            var expiryMinutes = _configuration.GetValue<int>(
                "Jwt:ExpiryMinutes");

            if (expiryMinutes <= 0)
                throw new InvalidOperationException(
                    "JWT expiry must be greater than zero.");

            var expiresAtUtc = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var claims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Name, user.Name),
        new(ClaimTypes.Email, user.Email),
        new(ClaimTypes.Role, user.Role)
    };

            var signingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return (token, expiresAtUtc);
        }

    }
}

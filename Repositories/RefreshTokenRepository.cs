using Microsoft.EntityFrameworkCore;
using WorkHub.Api.Data;
using WorkHub.Api.Domain.Entities;
using WorkHub.Api.Repositories.Interfaces;

namespace WorkHub.Api.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        {
            _context.RefreshTokens.Add(refreshToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<RefreshToken?> GetByHashAsync(string tokenHash,CancellationToken cancellationToken)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .AsNoTracking()
                .SingleOrDefaultAsync(rt=> rt.TokenHash == tokenHash ,cancellationToken);
        }

        public async Task<bool> TryRotateAsync(int currentTokenId,RefreshToken replacement,DateTime nowUtc,
            CancellationToken cancellationToken)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            var affectedRows = await _context.RefreshTokens
                    .Where(rt => rt.Id == currentTokenId &&
                    rt.RevokedAtUtc == null &&
                    rt.ExpiresAtUtc > nowUtc)
                    .ExecuteUpdateAsync(
                setter => setter
                .SetProperty(rt => rt.RevokedAtUtc, (DateTime?)nowUtc)
                .SetProperty(rt => rt.ReplacedByTokenHash, replacement.TokenHash),
                cancellationToken
                );
            if(affectedRows != 1)
            {
                await transaction.RollbackAsync(cancellationToken);
            }

            _context.RefreshTokens.Add(replacement);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return true;
        }
    }
}

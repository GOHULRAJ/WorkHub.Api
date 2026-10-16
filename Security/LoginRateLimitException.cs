namespace WorkHub.Api.Security
{
    public class LoginRateLimitException:Exception
    {
        public LoginRateLimitException():base("Too many failed login attempts. Please try again later.") 
        { 
        }
    }
}

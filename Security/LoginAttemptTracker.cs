using System.Collections.Concurrent;

namespace WorkHub.Api.Security
{
    public sealed class LoginAttemptTracker
    {
        private const int MaxFailedAttempt = 5;
        private static readonly TimeSpan TrackingWindow = TimeSpan.FromMinutes(15);
        private static readonly TimeSpan LockOutDuration = TimeSpan.FromMinutes(15);

        private readonly ConcurrentDictionary<string, AttemptState> _attempts = new();

        public bool IsBlocked(string email,string? ipAddress)
        {
            var key = CreateKey(email, ipAddress);
            if (!_attempts.TryGetValue(key, out var state))
                return false;

            lock (state)
            {
                var now = DateTimeOffset.UtcNow;

                if (state.BlockedUntilUtc is { } blockedUntil)
                {
                    if (blockedUntil > now)
                        return true;

                    state.BlockedUntilUtc = null;
                    state.FailedAttempts = 0;
                    state.WindowStartedUtc = now;
                }

                if (now - state.WindowStartedUtc >= TrackingWindow)
                {
                    state.FailedAttempts = 0;
                    state.WindowStartedUtc = now;
                }

                return false;
            }
        }
        public void RecordFailure(string email, string? ipAddress)
        {
            var key = CreateKey(email, ipAddress);
            var now = DateTimeOffset.UtcNow;
            var state = _attempts.GetOrAdd(key, _ => new AttemptState(now));

            lock (state)
            {
                if(now - state.WindowStartedUtc >= TrackingWindow)
                {
                    state.FailedAttempts = 0;
                    state.WindowStartedUtc = now;
                }

                state.FailedAttempts++;
                if(state.FailedAttempts >= MaxFailedAttempt)
                {
                    state.BlockedUntilUtc = now.Add(LockOutDuration);
                }
            }
        }
        public void Reset(string email,string? ipAddress)
        {
            _attempts.TryRemove(CreateKey(email, ipAddress), out _ );
            
        }
        private static string CreateKey(string email, string? ipAddress)
        {
            return $"{email.Trim().ToLowerInvariant()} || {ipAddress ?? "unknown"}";
        }
        private sealed class AttemptState(DateTimeOffset now)
        {
            public int FailedAttempts { get; set; }
            public DateTimeOffset WindowStartedUtc { get; set; } = now;
            public DateTimeOffset? BlockedUntilUtc { get; set; } = now;
        }
    }   

}


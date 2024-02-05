namespace Movies.Service
{
    public class TokenBlacklistService
    {
        private readonly HashSet<string> _blacklistedTokens = new HashSet<string>();

        /// <summary>
        /// Add a token to the blacklist.
        /// </summary>
        /// <param name="token">The token to be added to the blacklist.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddToBlacklistAsync(string token)
        {
            await Task.Run(() => _blacklistedTokens.Add(token));
        }

        /// <summary>
        /// Check if a token is blacklisted.
        /// </summary>
        /// <param name="token">The token to check.</param>
        /// <returns>True if the token is blacklisted, false otherwise.</returns>
        public async Task<bool> IsTokenBlacklistedAsync(string token)
        {
            return await Task.Run(() => _blacklistedTokens.Contains(token));
        }
    }
}

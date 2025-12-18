using System;

namespace SUP.P2FK
{
    /// <summary>
    /// Factory for creating Bitcoin backend instances.
    /// Automatically selects between local RPC or hosted API based on configuration.
    /// </summary>
    public static class BitcoinBackendFactory
    {
        private static BlockchainApiConfig _config;

        static BitcoinBackendFactory()
        {
            _config = BlockchainApiConfig.Load();
        }

        /// <summary>
        /// Reloads configuration from file.
        /// </summary>
        public static void ReloadConfig()
        {
            _config = BlockchainApiConfig.Load();
        }

        /// <summary>
        /// Gets current configuration.
        /// </summary>
        public static BlockchainApiConfig GetConfig()
        {
            return _config;
        }

        /// <summary>
        /// Creates a Bitcoin backend instance based on configuration and network parameters.
        /// </summary>
        /// <param name="url">RPC URL (for local mode) or ignored in API mode</param>
        /// <param name="username">RPC username (for local mode) or ignored in API mode</param>
        /// <param name="password">RPC password (for local mode) or ignored in API mode</param>
        /// <param name="versionByte">Version byte to determine network (111=testnet, 0=mainnet, etc.)</param>
        /// <returns>Bitcoin backend instance</returns>
        public static IBitcoinBackend Create(string url, string username, string password, string versionByte = "111")
        {
            // If API mode is not enabled, use local RPC
            if (!_config.EnableBlockchainApi)
            {
                return new BitcoinRpcBackend(url, username, password);
            }

            // API mode is enabled - determine network from version byte
            string network = DetermineNetwork(versionByte, url);
            
            try
            {
                return new BlockchainApiBackend(_config.BaseUrl, _config.ApiKey, network);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create blockchain API backend: {ex.Message}\n\nPlease check your API configuration in the Connections panel.", ex);
            }
        }

        /// <summary>
        /// Determines the network identifier for the API based on version byte or URL.
        /// </summary>
        private static string DetermineNetwork(string versionByte, string url)
        {
            // Map version bytes to networks
            // 111 = Bitcoin testnet
            // 0 = Bitcoin mainnet
            // For BlockCypher: "test3" or "main"
            
            if (versionByte == "111" || url.Contains("18332"))
            {
                return "test3"; // Bitcoin testnet3
            }
            else if (versionByte == "0" || url.Contains("8332"))
            {
                return "main"; // Bitcoin mainnet
            }
            
            // Default to testnet for safety
            return "test3";
        }

        /// <summary>
        /// Checks if API mode is currently enabled.
        /// </summary>
        public static bool IsApiModeEnabled()
        {
            return _config.EnableBlockchainApi;
        }
    }
}

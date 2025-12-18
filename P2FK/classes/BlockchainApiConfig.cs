using System;
using System.IO;
using Newtonsoft.Json;

namespace SUP.P2FK
{
    /// <summary>
    /// Configuration settings for blockchain API backend.
    /// </summary>
    public class BlockchainApiConfig
    {
        /// <summary>
        /// Base URL for the blockchain API.
        /// Default: https://api.blockcypher.com/v1/btc (BlockCypher API)
        /// </summary>
        public string BaseUrl { get; set; } = "https://api.blockcypher.com/v1/btc";

        /// <summary>
        /// API key or login credential.
        /// For BlockCypher, this is your API token. Register at https://accounts.blockcypher.com/
        /// </summary>
        public string ApiKey { get; set; } = "";

        /// <summary>
        /// Additional credential field (password/secret).
        /// Not used by BlockCypher, but available for other API providers.
        /// </summary>
        public string Secret { get; set; } = "";

        /// <summary>
        /// Enable blockchain API mode instead of local RPC.
        /// Default: false (uses local Bitcoin Core RPC)
        /// </summary>
        public bool EnableBlockchainApi { get; set; } = false;

        private static readonly string ConfigFilePath = "blockchain_api_config.json";

        /// <summary>
        /// Loads configuration from file. Returns default config if file doesn't exist.
        /// </summary>
        public static BlockchainApiConfig Load()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    string json = File.ReadAllText(ConfigFilePath);
                    return JsonConvert.DeserializeObject<BlockchainApiConfig>(json) ?? new BlockchainApiConfig();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load blockchain API config: {ex.Message}");
            }
            
            return new BlockchainApiConfig();
        }

        /// <summary>
        /// Saves configuration to file.
        /// </summary>
        public void Save()
        {
            try
            {
                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(ConfigFilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save blockchain API config: {ex.Message}");
                throw new Exception($"Failed to save blockchain API configuration: {ex.Message}", ex);
            }
        }
    }
}

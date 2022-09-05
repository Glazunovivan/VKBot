using Newtonsoft.Json;
using System;
using System.IO;

namespace ChatBotConfig
{
    public class VkBotConfig
    {
        private const string SettingsFilePath = "settings.json";
        private static readonly VkBotConfig _config;
        private string _accessToken;
        private ulong _groupId;

        static VkBotConfig()
        {
            _config = new VkBotConfig();
        }

        public string AccessToken => _accessToken;
        public ulong GroupId => _groupId;
        public static VkBotConfig Instance => _config;

        private VkBotConfig()
        {
            Configs configs = new Configs();
            configs = configs.LoadConfigs(SettingsFilePath);

            _accessToken = configs.AccessToken;
            _groupId = configs.GroupID;
        }
    }
}

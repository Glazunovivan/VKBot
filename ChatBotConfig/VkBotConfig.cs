namespace ChatBotConfig
{
    public class VkBotConfig
    {
        private const string SettingsFilePath = "settings.json";

        private static readonly VkBotConfig _config;
        private string _accessToken;
        private ulong _groupId;
        private long? _chatId;

        static VkBotConfig() => _config = new VkBotConfig();

        public string AccessToken => _accessToken;
        public ulong GroupId => _groupId;
        public long? ChatID => _chatId;

        /// <summary>
        /// Singleton
        /// </summary>
        public static VkBotConfig Instance => _config;

        private VkBotConfig()
        {
            Configs configs = new Configs();
            configs = configs.LoadConfigs(SettingsFilePath);

            _accessToken = configs.AccessToken;
            _groupId = configs.GroupID;
            _chatId = configs.ChatID;
        }
    }
}

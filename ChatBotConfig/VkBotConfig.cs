namespace ChatBotConfig
{
    public class VkBotConfig
    {
        private static readonly VkBotConfig config;
        private string accessToken;
        private ulong groupId;

        static VkBotConfig()
        {
            config = new VkBotConfig();
        }

        public string AccessToken => accessToken;
        public ulong GroupId => groupId;

        public static VkBotConfig Instance => config;

        private VkBotConfig()
        {
            accessToken = "f12f497c214efa4aafdbd53166756c3b0f2b1f4919910896a080ffb033de716838b6c5bf70c7f4fda7472";
            groupId = 210785287;
        }

    }
}

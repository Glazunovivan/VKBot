using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBot
{
    class VkBotConfig
    {
        private static readonly VkBotConfig _config;

        static VkBotConfig()
        {
            _config = new VkBotConfig();
        }

        public string AccessToken { get; set; }
        public ulong GroupId { get; set; }

        public static VkBotConfig Instance = _config;

        private VkBotConfig()
        {
            this.AccessToken = "f12f497c214efa4aafdbd53166756c3b0f2b1f4919910896a080ffb033de716838b6c5bf70c7f4fda7472";
            this.GroupId = 210785287;
        }
    }
}

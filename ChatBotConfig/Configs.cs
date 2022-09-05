using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBotConfig
{
    [Serializable]
    internal class Configs
    {
        public string AccessToken { get; set; }
        public ulong GroupID { get; set; }
        public long? ChatID { get; set; }

        public Configs() { }

        public Configs(string accessToken, ulong groupId, long? chatID)
        {
            AccessToken = accessToken;
            GroupID = groupId;
            ChatID = chatID;
        }

        public Configs LoadConfigs(string path)
        {
            var settingsJson = File.ReadAllText(path);
            Configs settings = new Configs();
            settings = JsonConvert.DeserializeObject<Configs>(settingsJson);

            return settings;
        }
    }
}

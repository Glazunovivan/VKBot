using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKBotChat
{
    internal class BotKeyboardCreator
    {
        private const string PATH = "keyboard.json";

        private void SaveKeyboard(BotKeyboard botKeyboard)
        {
            string keyboard = JsonConvert.SerializeObject(botKeyboard);

            File.WriteAllText(PATH, keyboard);
        }

        public BotKeyboard LoadKeyboard()
        {
            BotKeyboard botKeyboard;
            if (!File.Exists(PATH))
            {
                botKeyboard = JsonConvert.DeserializeObject<BotKeyboard>(File.ReadAllText(PATH));
            }
            else
            {
                botKeyboard = new BotKeyboard();
                botKeyboard.CreateKeboard();

                SaveKeyboard(botKeyboard);
            }

            return botKeyboard;
        }

    }
}

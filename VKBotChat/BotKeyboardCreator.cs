using Newtonsoft.Json;
using System.IO;

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

            //TODO : поправить эту дичь
            if (!File.Exists(PATH))
            {
                botKeyboard = JsonConvert.DeserializeObject<BotKeyboard>(File.ReadAllText(PATH));
            }
            else
            {
                botKeyboard = new BotKeyboard();
                botKeyboard.DefaultCreateKeboard();

                SaveKeyboard(botKeyboard);
            }

            return botKeyboard;
        }

    }
}

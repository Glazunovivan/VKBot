using Newtonsoft.Json;
using System.IO;

namespace VKBotChat
{
    internal class BotKeyboardCreator
    {
        private const string PATH = "keyboard.json";
        private BotKeyboard _botKeyboard;

        public BotKeyboardCreator()
        {
            _botKeyboard = new BotKeyboard();    
        }

        private void SaveKeyboard(BotKeyboard botKeyboard)
        {
            string keyboard = JsonConvert.SerializeObject(botKeyboard);

            File.WriteAllText(PATH, keyboard);
        }

        public BotKeyboardCreator LoadKeyboard()
        {
            //TODO : поправить эту дичь
            if (!File.Exists(PATH))
            {
                _botKeyboard = JsonConvert.DeserializeObject<BotKeyboard>(File.ReadAllText(PATH));
            }
            else
            {
                _botKeyboard = new BotKeyboard();
                _botKeyboard.DefaultCreateKeboard();

                SaveKeyboard(_botKeyboard);
            }
            return this;
        }

        public BotKeyboard Create()
        {
            return _botKeyboard;
        }

    }
}

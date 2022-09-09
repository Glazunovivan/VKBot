using System;
using ChatBotConfig;
using VkBotChat;


namespace VKBotChat
{
    class Program
    {
        static void Main(string[] args)
        {
            ParserTimetable.ParserTimetable.Start();
            
            BotKeyboard botKeyboard = new BotKeyboard();
            botKeyboard.SaveSettings(botKeyboard);

            new Bot(VkBotConfig.Instance, botKeyboard).Start();

            Console.WriteLine("Кукусики, мы тут чат-ботимся!");

            Console.WriteLine("Нажмите любую кнопку для завершения работы");
            Console.ReadLine();
        }
    }
}

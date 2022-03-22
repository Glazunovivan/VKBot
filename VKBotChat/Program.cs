using System;
using ChatBotConfig;
using VkBotChat;
using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace VKBotChat
{
    class Program
    {
        public static VkApi api = new VkApi();

        static void Main(string[] args)
        {
            ParserTimetable.Program.Start();

            new Bot(VkBotConfig.Instance).Start();
            Console.WriteLine("Кукусики, мы тут чат-ботимся!");

            Console.ReadLine();
        }
    }
}

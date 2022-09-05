using System;
using ChatBotConfig;
using Microsoft.Extensions.Configuration;
using VkBotChat;
using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace VKBotChat
{
    class Program
    {
        static void Main(string[] args)
        {
            ParserTimetable.ParserTimetable.Start();

            new Bot(VkBotConfig.Instance).Start();

            Console.WriteLine("Кукусики, мы тут чат-ботимся!");

            Console.WriteLine("Нажмите любую кнопку для завершения работы");
            Console.ReadLine();
        }
    }
}

using System;
using ChatBotConfig;
using VkBotChat;


namespace VKBotChat
{
    class Program
    {
        static void Main(string[] args)
        {
            new Bot(VkBotConfig.Instance).Start();

            Console.WriteLine("Кукусики, мы тут чат-ботимся!");

            Console.WriteLine("Нажмите любую кнопку для завершения работы");
            Console.ReadLine();
        }
    }
}

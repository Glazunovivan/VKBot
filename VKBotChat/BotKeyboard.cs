using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Keyboard;

namespace VKBotChat
{
    internal class BotKeyboard : MessageKeyboard
    {
        public BotKeyboard() { }

        public void CreateKeboard()
        {
            Inline = false;
            OneTime = false;

            Buttons = new List<List<MessageKeyboardButton>>()
            {
                //1я строка
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton()
                    {
                        Action = new MessageKeyboardButtonAction()
                        {
                            Type =  KeyboardButtonActionType.Callback,
                            Payload = "{\"button\": \"TimetableToday\"}",
                            Label = "Расписание на сегодня"
                        },
                        Color = KeyboardButtonColor.Primary

                    },
                    new MessageKeyboardButton()
                    {
                        Action = new MessageKeyboardButtonAction()
                        {
                            Type =  KeyboardButtonActionType.Callback,
                            Payload = "{\"button\": \"NextLesson\"}",
                            Label = "Следующая пара"
                        },
                        Color = KeyboardButtonColor.Primary
                    },
                },

                //2я строка
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton()
                    {
                        Action = new MessageKeyboardButtonAction()
                        {
                            Type =  KeyboardButtonActionType.OpenLink,
                            Label="E-learning",
                            Link = new Uri("https://e-learning.mgupp.ru/login/index.php")
                        }
                    },
                    new MessageKeyboardButton()
                    {
                        Action = new MessageKeyboardButtonAction()
                        {
                            Type = KeyboardButtonActionType.OpenLink,
                            Label = "Личный кабинет",
                            Link = new Uri("https://mgupp.ru/cabinet/index.php")
                        }
                    }
                },

                //3я строка
                new List<MessageKeyboardButton>()
                {
                    //new MessageKeyboardButton()
                    //{
                    //    Action = new MessageKeyboardButtonAction()
                    //    {
                    //        Type = KeyboardButtonActionType.Callback,
                    //        Label = "ДЗ в ЛС",
                    //        Payload = "{\r\n  \"button\": \"GetHW\"\r\n}"
                    //    }
                    //},
                    new MessageKeyboardButton()
                    {
                        Action = new MessageKeyboardButtonAction()
                        {
                            Type = KeyboardButtonActionType.Callback,

                            Label = "ТЕСТ(ВРЕМЯ)",
                            Payload = "{\r\n  \"button\": \"TESTTIME\"\r\n}"
                        }
                    }
                }
            };
        }
    }
}

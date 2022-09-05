using ChatBotConfig;
using System;
using System.Collections.Generic;
using System.Threading;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.GroupUpdate;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace VkBotChat
{
    class Bot
    {
        public event Action<GroupUpdate> OnMessage;
        
        private VkApi vkClient;
        private LongPollServerResponse longPollServerResponse;
        private string currentTs;
        private MessageKeyboard messageKeyboard;

        private long? _chatID;
        private long _userID;

        public Bot(VkBotConfig config)
        {
            vkClient = new VkApi();
            vkClient.Authorize(new ApiAuthParams
            {
                AccessToken = config.AccessToken,
                Settings = Settings.All | Settings.Messages
            });
            
            _chatID = config.ChatID;

            longPollServerResponse = vkClient.Groups.GetLongPollServer(config.GroupId);
            currentTs = longPollServerResponse.Ts;
        }

        public void Start(Action<GroupUpdate> onMessage = null)
        {
            Console.WriteLine("Запуск бота...");
            CreateKeyboard();

            OnMessage += SendMessageChatGroup;

            new Thread(OnUpdate).Start();
            new Thread(OnTimerUpdate).Start();
        }

        /// <summary>
        /// Отправляет сообщение в чат группы по событию
        /// </summary>
        /// <param name="e"></param>
        private void SendMessageChatGroup(GroupUpdate e)
        {
            MessagesSendParams msg = new MessagesSendParams();

            msg = new MessagesSendParams()
            {
                RandomId = Guid.NewGuid().GetHashCode(),
                PeerId = e.Message.PeerId,
                Keyboard = messageKeyboard
            };

            switch (e?.Message?.Text)
            {
                case "кнопки":
                    msg.Message = "Включаю кнопки";
                    msg.Keyboard = messageKeyboard;
                    break;
                default:
                    return;
            }
            vkClient.Messages.Send(msg);
        }

        /// <summary>
        /// Отправляет уведомление о следующем занятии
        /// </summary>
        /// <param name="item"></param>
        /// <param name="payload"></param>
        private void SendSnowSnackbar(GroupUpdate item)
        {
            EventData eventData = new EventData()
            {
                Type = MessageEventType.SnowSnackbar
            };

            switch (item?.MessageEvent?.Payload)
            {
                case "{\r\n  \"button\": \"NextLesson\"\r\n}":
                    eventData.Text = ParserTimetable.ParserTimetable.ShowNextLesson(DateTime.Now);
                    if (eventData.Text == string.Empty)
                    {
                        eventData.Text = "На сегодня занятий больше нет, отдыхайте ;)";
                    }
                    break;
                default:
                    eventData.Text = "Не понимаю :(";
                    break;
            }

            vkClient.Messages.SendMessageEventAnswer(item.MessageEvent.EventId,
                                                     (long)item.MessageEvent.UserId,
                                                     (long)item.MessageEvent.PeerId,
                                                     eventData);
        }

        private void NotificationChat(byte typeNotification)
        {
            string time = String.Empty;
            
            string lesson = ParserTimetable.ParserTimetable.ShowNextLesson(DateTime.Now);

            if (lesson == string.Empty)
            {
                return;
            }

            switch (typeNotification)
            {
                //утро
                case 0:
                    time = "через 30 минут занятие!";
                    break;
                //за 15 минут до начала
                case 1:
                    time = "через 15 минут занятие!";
                    break;
                //обед
                case 2:
                    time = "через 20 минут занятие!";
                    break;
                case 3:
                    time = "через 5 минут занятие!";
                    break;
            }

            MessagesSendParams msg = new MessagesSendParams()
            {
                PeerId = _chatID,
                RandomId = Guid.NewGuid().GetHashCode(),
                Message = $"@all, {time} {lesson}",
                Keyboard = messageKeyboard
            };

            vkClient.Messages.Send(msg);
        }

        private void CallbackAnswerInChat(GroupUpdate item)
        {
            switch (item?.MessageEvent?.Payload)
            {
                //отправляет расписание на текущий день
                case "{\r\n  \"button\": \"TimetableToday\"\r\n}":
                    //SendMessage(item);
                    //"{\r\n  \"button\": \"TimetableToday\"\r\n}"
                    SendMessageUser(item);
                    break;
                //присылает уведомление о следующем занятии
                case "{\r\n  \"button\": \"NextLesson\"\r\n}":
                    //"{\r\n  \"button\": \"NextLesson\"\r\n}"
                    SendSnowSnackbar(item);
                    break;
                //присылает ДЗ
                case "{\r\n  \"button\": \"GetHomeWork\"\r\n}":
                    //"{\r\n  \"button\": \"GetHW\"\r\n}"
                    SendMessageUser(item);
                    break;
                //тест времени
                case "{\r\n  \"button\": \"TESTTIME\"\r\n}":
                    NotificationChat(0);
                    break;
                default:
                    break;
            }
        }

        private void OnTimerUpdate()
        {
            while (true)
            {
                byte typeNotif = IsParaTime();
                
                if (typeNotif != 240)
                {
                    NotificationChat(typeNotif);
                }

                //1 раз в минуту проверка
                Thread.Sleep(60000);    
            }
        }

        /// <summary>
        /// Время для оповещения о начале пары
        /// </summary>
        /// <returns></returns>
        private byte IsParaTime()
        {
            if (DateTime.Now.Hour == 8 &&
                    DateTime.Now.Minute == 0)
            {
                return 0;
            }
            else if(DateTime.Now.Hour == 10 &&
                    DateTime.Now.Minute == 10)
            {
                return 1;
            }
            else if (DateTime.Now.Hour == 12 &&
                    DateTime.Now.Minute == 25)
            {
                return 2;
            }
            else if (DateTime.Now.Hour == 14 &&
                    DateTime.Now.Minute == 15)
            {
                return 1;
            }
            else if (DateTime.Now.Hour == 16 &&
                    DateTime.Now.Minute == 00)
            {
                return 2;
            }
            else if (DateTime.Now.Hour == 17 &&
                    DateTime.Now.Minute == 45)
            {
                return 3;
            }
            else if (DateTime.Now.Hour == 19 &&
                    DateTime.Now.Minute == 25)
            {
                return 3;
            }

            return 240;
        }

        private void OnUpdate()
        {
            while (true)
            {
                var longPoll = vkClient.Groups.GetBotsLongPollHistory(
                    new BotsLongPollHistoryParams()
                    {
                        Ts = currentTs,
                        Key = longPollServerResponse.Key,
                        Server = longPollServerResponse.Server
                    }
                    );

                if (OnMessage != null)
                {
                    foreach (GroupUpdate item in longPoll.Updates)
                    {
                        currentTs = longPoll.Ts;

                        if (item?.MessageEvent != null)
                        {
                            CallbackAnswerInChat(item);
                        }

                        if (item?.Message?.RandomId != 0)
                        {
                            continue;
                        }

                        OnMessage?.Invoke(item);
                        Thread.Sleep(100);
                    }
                }
                Thread.Sleep(2000);
            }
        }

        //TODO: сделать загрузку из файла json, для удобного редактирования
        private void CreateKeyboard()
        {
            messageKeyboard = new MessageKeyboard();
            messageKeyboard.Inline = false;
            messageKeyboard.OneTime = false;

            messageKeyboard.Buttons = new List<List<MessageKeyboardButton>>()
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

        /// <summary>
        /// Отправляет сообщение в чат
        /// </summary>
        /// <param name="item"></param>
        private void SendMessageChat(GroupUpdate item)
        {
            MessagesSendParams msg = new MessagesSendParams()
            {
                RandomId = Guid.NewGuid().GetHashCode(),
                Message = ParserTimetable.ParserTimetable.ShowTimetableOfDay(DateTime.Now),
                Keyboard = messageKeyboard,
                PeerId = item.MessageEvent.PeerId
            };

            vkClient.Messages.SendMessageEventAnswer(item.MessageEvent.EventId,
                                                    (long)item.MessageEvent.UserId,
                                                    (long)item.MessageEvent.PeerId,
                                                    null);
            vkClient.Messages.Send(msg);
        }

        private void SendMessageUser(GroupUpdate item)
        {
            try
            {
                MessagesSendParams msg = new MessagesSendParams()
                {
                    RandomId = Guid.NewGuid().GetHashCode(),
                    Message = ParserTimetable.ParserTimetable.ShowTimetableOfDay(DateTime.Now),
                    Keyboard = messageKeyboard,
                    PeerId = item.MessageEvent.UserId
                };

                switch (item.MessageEvent.Payload)
                {
                    case "{\r\n  \"button\": \"GetHomeWork\"\r\n}":
                        msg.Message = HomeWork.HomeWorkMain.GetHomeWorksString();
                        break;

                    case "{\r\n  \"button\": \"TimetableToday\"\r\n}":
                        msg.Message = ParserTimetable.ParserTimetable.ShowTimetableOfDay(DateTime.Now);
                        break;
                }

                vkClient.Messages.SendMessageEventAnswer(item.MessageEvent.EventId,
                                                        (long)item.MessageEvent.UserId,
                                                        (long)item.MessageEvent.PeerId,
                                                        null);
                vkClient.Messages.Send(msg);
            }
            //могу отсутствовать права на отправку сообщений
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}

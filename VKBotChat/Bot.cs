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
        //https://vknet.github.io/vk/
        private VkApi vkClient;

        // Объект, с помощью которого можно подключиться
        // к серверу быстрых сообщений
        // с целью их получения и обработки других событий
        // http://vk.com/dev/messages.getLongPollServer
        private LongPollServerResponse longPollServerResponse;

        private VkBotConfig config;

        string currentTs;

        public event Action<GroupUpdate> OnMessage;

        private MessageKeyboard messageKeyboard;
        
        public Bot(VkBotConfig config)
        {
            this.config = config;
            vkClient = new VkApi();
            vkClient.Authorize(new ApiAuthParams
            {
                AccessToken = config.AccessToken,
                Settings = Settings.All | Settings.Messages
            });

            longPollServerResponse = vkClient.Groups.GetLongPollServer(config.GroupId);
            currentTs = longPollServerResponse.Ts;
        }

        public void Start(Action<GroupUpdate> onMessage = null)
        {
            Console.WriteLine("Запуск бота...");
            CreateKeyboard();

            OnMessage += SendMessageChatGroup;

            new Thread(OnUpdate).Start();
        }

        /// <summary>
        /// Отправка сообщения в групповой чат
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="text"></param>
        /// <param name="keyboard"></param>
        /// 
        //private void SendMessageChatGroup(long? chatId, MessageKeyboard keyboard = null)
        //{
        //    var msg = new MessagesSendParams()
        //    {
        //        RandomId = Guid.NewGuid().GetHashCode(),
        //        PeerId = chatId,
        //        Message = "Чота"
        //    };

        //    if (keyboard != null)
        //    {
        //        msg.Keyboard = keyboard;
        //    }

        //    vkClient.Messages.Send(msg);
        //}

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

        private async void OnUpdate()
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

        private void CallbackAnswerInChat(GroupUpdate item)
        {  
            switch (item?.MessageEvent?.Payload)
            {
                case "{\r\n  \"button\": \"TimetableToday\"\r\n}":  //отправляет расписание на текущий день
                    SendMessage(item);
                    break;
                    
                case "{\r\n  \"button\": \"NextLesson\"\r\n}":  //присылает уведомление о следующем занятии
                    SendNotification(item, "{\r\n  \"button\": \"NextLesson\"\r\n}");
                    break;
                case "{\r\n  \"button\": \"GetHW\"\r\n}":  //присылает уведомление о следующем занятии
                    SendMessageUser(item, "{\r\n  \"button\": \"GetHW\"\r\n}");
                    break;
                default:
                    break;
            }
        }

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
                    }
                },

                //3я строка
                new List<MessageKeyboardButton>()
                {
                    new MessageKeyboardButton()
                    {
                        Action = new MessageKeyboardButtonAction()
                        {
                            Type = KeyboardButtonActionType.OpenLink,
                            Label = "Личный кабинет",
                            Link = new Uri("https://mgupp.ru/cabinet/index.php")
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Отправляет уведомление о следующем занятии
        /// </summary>
        /// <param name="item"></param>
        /// <param name="payload"></param>
        private void SendNotification(GroupUpdate item, string payload)
        {
            EventData eventData = new EventData();
            eventData.Type = MessageEventType.SnowSnackbar;
            switch (payload)
            {
                case "{\r\n  \"button\": \"NextLesson\"\r\n}":
                    eventData.Text = ParserTimetable.ParserTimetable.ShowNextLesson(DateTime.Now);
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

        /// <summary>
        /// Отправляет сообщение в чат
        /// </summary>
        /// <param name="item"></param>
        private void SendMessage(GroupUpdate item)
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
            vkClient.Messages.SendAsync(msg);
        }

        private void SendMessageUser(GroupUpdate item, string payload)
        {
            MessagesSendParams msg = new MessagesSendParams()
            {
                RandomId = Guid.NewGuid().GetHashCode(),
                Message = ParserTimetable.ParserTimetable.ShowTimetableOfDay(DateTime.Now),
                Keyboard = messageKeyboard,
                PeerId = item.MessageEvent.UserId
            };

            switch (payload) 
            {
                case "{\r\n  \"button\": \"GetHW\"\r\n}":
                    msg.Message = 
                    break;
            }

            vkClient.Messages.SendMessageEventAnswer(item.MessageEvent.EventId,
                                                    (long)item.MessageEvent.UserId,
                                                    (long)item.MessageEvent.PeerId,
                                                    null);
            vkClient.Messages.SendAsync(msg);
        }
    }
}

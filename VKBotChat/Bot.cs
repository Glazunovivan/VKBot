using ChatBotConfig;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VKBotChat;
using VKBotChat.Commands;
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
        
        private VkApi _vkClient;
        private LongPollServerResponse _longPollServerResponse;
        private string _currentTs;
        private MessageKeyboard _messageKeyboard;

        private long? _chatID;

        public Bot(VkBotConfig config)
        {
            _vkClient = new VkApi();
            _vkClient.Authorize(new ApiAuthParams
            {
                AccessToken = config.AccessToken,
                Settings = Settings.All | Settings.Messages
            });
            _chatID = config.ChatID;

            BotKeyboardCreator botKeyboardCreator = new BotKeyboardCreator();
            _messageKeyboard = botKeyboardCreator.LoadKeyboard();
            
            _longPollServerResponse = _vkClient.Groups.GetLongPollServer(config.GroupId);
            _currentTs = _longPollServerResponse.Ts;
        }

        public void Start(Action<GroupUpdate> onMessage = null)
        {
            Console.WriteLine("Запуск бота...");
            Console.WriteLine("Раздаем задачи");

            Task observeUpdate = new Task(OnUpdate);
            Task timerUpdate = new Task(OnTimerUpdate);

            observeUpdate.Start();
            timerUpdate.Start();
        }

        /// <summary>
        /// Отправляет сообщение в чат группы по событию
        /// </summary>
        private void ButtonsOn(GroupUpdate @event)
        {
            MessagesSendParams msg = new MessagesSendParams()
            {
                RandomId = Guid.NewGuid().GetHashCode(),
                PeerId = @event.Message.PeerId,
                Message = "Включаю кнопки",
                Keyboard = _messageKeyboard
            };
            _vkClient.Messages.Send(msg);
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
                Keyboard = _messageKeyboard
            };

            _vkClient.Messages.Send(msg);
        }

        private void CallbackButtonsAnswerInChat(GroupUpdate @event)
        {
            Command command;
           
            switch (@event?.MessageEvent?.Payload)
            {
                //отправляет расписание на текущий день
                case "{\r\n  \"button\": \"TimetableToday\"\r\n}":
                    command = new PrivateMessageCommand(@event, _messageKeyboard);
                    command.Action(_vkClient);
                    break;

                //присылает уведомление о следующем занятии
                case "{\r\n  \"button\": \"NextLesson\"\r\n}":
                    command = new ShowSnowSnackbar(@event);
                    command.Action(_vkClient);
                    break;

                //присылает ДЗ
                case "{\r\n  \"button\": \"GetHomeWork\"\r\n}":
                    command = new PrivateMessageCommand(@event, _messageKeyboard);
                    command.Action(_vkClient);
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
                Console.WriteLine("On timer update");
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
                Console.WriteLine("OnUpdate");
                var longPoll = _vkClient.Groups.GetBotsLongPollHistory(
                    new BotsLongPollHistoryParams()
                    {
                        Ts = _currentTs,
                        Key = _longPollServerResponse.Key,
                        Server = _longPollServerResponse.Server
                    }
                    );

                foreach (GroupUpdate item in longPoll.Updates)
                {
                    _currentTs = longPoll.Ts;

                    if (item?.MessageEvent != null)
                    {
                        CallbackButtonsAnswerInChat(item);
                    }

                    if (item?.Message?.Text == "кнопки")
                    {
                        ButtonsOn(item);
                    }

                    if (item?.Message?.RandomId != 0)
                    {
                        continue;
                    }

                    Thread.Sleep(100);
                }
                Thread.Sleep(2000);
            }
        }

    }
}

using ChatBotConfig;
using System;
using System.Threading;
using System.Threading.Tasks;
using VKBotChat;
using VKBotChat.Commands;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.GroupUpdate;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace VkBotChat
{
    class Bot
    {   
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
                    //NotificationChat(0);
                    break;
                default:
                    break;
            }
        }

        private void OnTimerUpdate()
        {
            Command notification = new NotificationMessageChatCommand(_chatID, _messageKeyboard);

            while (true)
            {
                Console.WriteLine("On timer update");
                
                notification.Action(_vkClient);

                //1 раз в минуту проверка
                Thread.Sleep(60000);    
            }
        }

        private void OnUpdate()
        {
            while (true)
            {
                Console.WriteLine("On check Update");
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

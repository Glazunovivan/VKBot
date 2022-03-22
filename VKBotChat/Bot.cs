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

        private void CreateKeyboard()
        {
            messageKeyboard = new MessageKeyboard();
            messageKeyboard.Inline = false;
            messageKeyboard.OneTime = false;

            messageKeyboard.Buttons = new List<List<MessageKeyboardButton>>()
            {
                new List<MessageKeyboardButton>()   //1я строка
                {   
                    new MessageKeyboardButton()
                    {
                        Action = new MessageKeyboardButtonAction()
                        {
                            Type =  KeyboardButtonActionType.Callback,
                            Label="Показать расписание на сегодня"
                        }
                        
                    },
                },
                
                new List<MessageKeyboardButton>()   //2я строка
                {
                    new MessageKeyboardButton()
                    {
                        Action = new MessageKeyboardButtonAction()
                        {
                            Type =  KeyboardButtonActionType.OpenLink,
                            Label="Перейти в e-learning",
                            Link = new Uri("https://e-learning.mgupp.ru/login/index.php")
                        }
                    }
                }
            };
        }

        public void Start(Action<GroupUpdate> onMessage = null)
        {
            CreateKeyboard();

            if (onMessage != null)
            {
                OnMessage += OnMessage;
            }
            else
            {
                OnMessage += MessageKeyboard;
            }

            new Thread(OnUpdate).Start();
        }

        private void MessageKeyboard(GroupUpdate e)
        {
            SendMessageChatGroup(e.Message.PeerId, "Keyboard", messageKeyboard);
        }

        /// <summary>
        /// Отправка сообщения в групповой чат
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="text"></param>
        /// <param name="keyboard"></param>
        private void SendMessageChatGroup(long? chatId, string text, MessageKeyboard keyboard = null)
        {
            var msg = new MessagesSendParams()
            {
                RandomId = Guid.NewGuid().GetHashCode(),
                PeerId = chatId,
                Message = text
            };

            if (keyboard != null)
            {
                msg.Keyboard = keyboard;
            }

            vkClient.Messages.Send(msg);
        }

        private void OnUpdate()
        {
            while (true)
            {
                var res = vkClient.Groups.GetBotsLongPollHistory(
                    new BotsLongPollHistoryParams()
                    {
                        Key = longPollServerResponse.Key,
                        Ts = currentTs,
                        Server = longPollServerResponse.Server
                    });

                if (OnMessage != null)
                {
                    foreach (GroupUpdate item in res.Updates)
                    {
                        currentTs = res.Ts;

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

    }
}

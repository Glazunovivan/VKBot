using System;
using VkNet;
using VkNet.Model.GroupUpdate;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace VKBotChat.Commands
{
    /// <summary>
    /// Отправка сообщения в чат c пометкой "all" 
    /// </summary>
    public class NotificationMessageChatCommand : Command
    {
        private MessageKeyboard _messageKeyboard;
        private long? _chatId;

        public NotificationMessageChatCommand(long? chatId, MessageKeyboard messageKeyboard, GroupUpdate @event = null) : base(@event)
        {
            _chatId = chatId;
            _messageKeyboard = messageKeyboard;
        }

        public override void Action(VkApi api)
        {
            string time = string.Empty;
            string lesson = ParserTimetable.Timetable.Instance.ShowNextLesson(DateTime.Now);
            
            byte typeNotification = IsParaTime();
            
            if (typeNotification == 240)
            {
                return;
            }

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
                PeerId = _chatId,
                RandomId = Guid.NewGuid().GetHashCode(),
                Message = $"@all, {time} {lesson}",
                Keyboard = _messageKeyboard
            };

            api.Messages.Send(msg);
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
            else if (DateTime.Now.Hour == 10 &&
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
    }
}

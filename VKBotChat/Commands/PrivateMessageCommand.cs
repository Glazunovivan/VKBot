using System;
using VkNet;
using VkNet.Model.GroupUpdate;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace VKBotChat.Commands
{
    /// <summary>
    /// Отправляет личное сообщение пользователю, если у того есть права
    /// </summary>
    public class PrivateMessageCommand : Command
    {
        private MessageKeyboard _messageKeyboard;

        public PrivateMessageCommand(GroupUpdate @event, MessageKeyboard keyboard) : base(@event)
        {
            _messageKeyboard = keyboard;
        }

        public override void Action(VkApi api)
        {
            try
            {
                MessagesSendParams msg = new MessagesSendParams()
                {
                    RandomId = Guid.NewGuid().GetHashCode(),
                    Message = ParserTimetable.Timetable.Instance.ShowTimetableOfDay(DateTime.Now),
                    Keyboard = _messageKeyboard,
                    PeerId = Event.MessageEvent.UserId
                };

                switch (Event.MessageEvent.Payload)
                {
                    case "{\r\n  \"button\": \"GetHomeWork\"\r\n}":
                        msg.Message = HomeWork.HomeWorkMain.GetHomeWorksString();
                        break;

                    case "{\r\n  \"button\": \"TimetableToday\"\r\n}":
                        msg.Message = ParserTimetable.Timetable.Instance.ShowTimetableOfDay(DateTime.Now);
                        break;
                }

                api.Messages.SendMessageEventAnswer(Event.MessageEvent.EventId,
                                                        (long)Event.MessageEvent.UserId,
                                                        (long)Event.MessageEvent.PeerId,
                                                        null);
                api.Messages.Send(msg);
            }
            //могу отсутствовать права на отправку сообщений
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

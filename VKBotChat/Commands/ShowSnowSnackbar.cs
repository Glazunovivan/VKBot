using System;
using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.GroupUpdate;

namespace VKBotChat.Commands
{
    /// <summary>
    /// Отправляет уведомление о следующем занятии
    /// </summary>
    public class ShowSnowSnackbar :Command, ICommand
    {
        public ShowSnowSnackbar(GroupUpdate @event) : base(@event) { }

        public void Action(VkApi api)
        {
            EventData eventData = new EventData()
            {
                Type = MessageEventType.SnowSnackbar
            };

            switch (Event?.MessageEvent?.Payload)
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

            api.Messages.SendMessageEventAnswer(Event.MessageEvent.EventId,
                                                     (long)Event.MessageEvent.UserId,
                                                     (long)Event.MessageEvent.PeerId,
                                                     eventData);
        }
    }
}

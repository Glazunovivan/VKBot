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
    public class ShowSnowSnackbarCommand :Command
    {
        public ShowSnowSnackbarCommand(GroupUpdate @event) 
            : base(@event) 
        { }

        public override void Action(VkApi api)
        {
            EventData eventData = new EventData()
            {
                Type = MessageEventType.SnowSnackbar
            };

            eventData.Text = ParserTimetable.Timetable.Instance.ShowNextLesson(DateTime.Now);
            if (eventData.Text == string.Empty)
            {
                eventData.Text = "На сегодня занятий больше нет, отдыхайте ;)";
            }

            api.Messages.SendMessageEventAnswer(Event.MessageEvent.EventId,
                                                     (long)Event.MessageEvent.UserId,
                                                     (long)Event.MessageEvent.PeerId,
                                                     eventData);
        }
    }
}

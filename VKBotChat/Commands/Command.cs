using VkNet.Model.GroupUpdate;

namespace VKBotChat.Commands
{
    public class Command
    {
        protected GroupUpdate Event;
        protected Command(GroupUpdate @event)
        {
            Event = @event;
        }
    }
}

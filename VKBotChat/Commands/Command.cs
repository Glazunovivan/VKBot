using VkNet;
using VkNet.Model.GroupUpdate;

namespace VKBotChat.Commands
{
    public abstract class Command : ICommand
    {
        protected GroupUpdate Event;

        protected Command(GroupUpdate @event)
        {
            Event = @event;
        }

        public abstract void Action(VkApi api);
    }
}

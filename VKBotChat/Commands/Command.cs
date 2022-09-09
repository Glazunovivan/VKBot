using VkNet;
using VkNet.Model.GroupUpdate;

namespace VKBotChat.Commands
{
    public class Command : ICommand
    {
        protected GroupUpdate Event;

        protected Command(GroupUpdate @event)
        {
            Event = @event;
        }

        public virtual void Action(VkApi api)
        {
            throw new System.NotImplementedException();
        }
    }
}

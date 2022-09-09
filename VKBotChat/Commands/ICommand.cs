using VkNet;

namespace VKBotChat.Commands
{
    public interface ICommand
    {
        public void Action(VkApi api);
    }
}

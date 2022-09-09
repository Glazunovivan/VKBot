using VkNet;

namespace VKBotChat.Commands.Interfaces
{
    public interface ICommand
    {
        public void Action(VkApi api);
    }
}

using VkNet;

namespace VKBotChat.Commands
{
    public interface ICommand
    {
        public void Do(VkApi api);
    }
}

using System;
using VkNet;
using VkNet.Model.GroupUpdate;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace VKBotChat.Commands
{
    /// <summary>
    /// Отправляет сообщение в чат группы по событию
    /// </summary>
    public class ChatMessageCommand : Command
    {
        private MessageKeyboard _messageKeyboard;

        public ChatMessageCommand(GroupUpdate @event, MessageKeyboard messageKeyboard)
            : base(@event) 
        {
            _messageKeyboard = messageKeyboard;
        }
        
        public override void Action(VkApi api)
        {
            MessagesSendParams msg = new MessagesSendParams()
            {
                RandomId = Guid.NewGuid().GetHashCode(),
                PeerId = Event.Message.PeerId,
                Message = "Йоу",
                Keyboard = _messageKeyboard
            };

            api.Messages.Send(msg);
        }
    }
}

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
    public class ChatMessageCommand : Command,ICommand
    {
        private MessageKeyboard _messageKeyboard;

        public ChatMessageCommand(GroupUpdate @event, MessageKeyboard messageKeyboard): base(@event) 
        {
            _messageKeyboard = messageKeyboard;
        }
        
        public void Do(VkApi api)
        {
            MessagesSendParams msg = new MessagesSendParams();

            msg = new MessagesSendParams()
            {
                RandomId = Guid.NewGuid().GetHashCode(),
                PeerId = Event.Message.PeerId,
                Keyboard = _messageKeyboard
            };

            switch (Event?.Message?.Text)
            {
                case "кнопки":
                    msg.Message = "Включаю кнопки";
                    msg.Keyboard = _messageKeyboard;
                    break;
                default:
                    return;
            }
            api.Messages.Send(msg);
        }
    }
}

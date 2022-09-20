using Telegram.Bot.Types;

namespace SalatBot.Scripts.Main.Interfaces
{
    public interface IMessageProcessor
    {
        void Process(Message message);
        void SendMessage(string text);
        void SendInlineMessage(Message message, string[][] keyboardData);
    }
}
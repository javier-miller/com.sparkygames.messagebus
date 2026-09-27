using SparkyGames.MessageBus;

namespace SparkyGames.MessageBus.Samples
{
    public sealed class MessageBusSampleMessage : IMessage
    {
        public const string BusName = "MessageBusSample";

        public MessageBusSampleMessage(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }
}

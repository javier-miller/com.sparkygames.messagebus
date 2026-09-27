using SparkyGames.UnityServiceBus;

namespace SparkyGames.UnityServiceBus.Samples
{
    public sealed class ServiceBusSampleMessage : IMessage
    {
        public const string BusName = "ServiceBusSample";

        public ServiceBusSampleMessage(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }
}

using UnityEngine;

namespace SparkyGames.MessageBus.Samples
{
    public sealed class MessageBusSampleSender : MonoBehaviour
    {
        [SerializeField] private string _text = "Hello from Message Bus";

        private void Start()
        {
            UnityMessageBroker.GetOrCreateBus(MessageBusSampleMessage.BusName)
                .Publish(new MessageBusSampleMessage(_text));
        }
    }
}

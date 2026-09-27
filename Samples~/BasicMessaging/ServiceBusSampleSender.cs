using UnityEngine;

namespace SparkyGames.UnityServiceBus.Samples
{
    public sealed class ServiceBusSampleSender : MonoBehaviour
    {
        [SerializeField] private string _text = "Hello from Service Bus";

        private void Start()
        {
            UnityMessageBroker.GetOrCreateBus(ServiceBusSampleMessage.BusName)
                .Publish(new ServiceBusSampleMessage(_text));
        }
    }
}

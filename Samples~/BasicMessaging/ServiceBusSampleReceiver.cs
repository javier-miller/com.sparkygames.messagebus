using System;
using UnityEngine;

namespace SparkyGames.UnityServiceBus.Samples
{
    public sealed class ServiceBusSampleReceiver : MonoBehaviour
    {
        private IDisposable _subscription;

        private void OnEnable()
        {
            _subscription = UnityMessageBroker
                .GetOrCreateBus(ServiceBusSampleMessage.BusName)
                .Subscribe<ServiceBusSampleMessage>(OnMessage);
        }

        private void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }

        private void OnMessage(ServiceBusSampleMessage message)
        {
            Debug.Log("Service Bus sample received: " + message.Text, this);
        }
    }
}

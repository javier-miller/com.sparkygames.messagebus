using System;
using UnityEngine;

namespace SparkyGames.MessageBus.Samples
{
    public sealed class MessageBusSampleReceiver : MonoBehaviour
    {
        private IDisposable _subscription;

        private void OnEnable()
        {
            _subscription = UnityMessageBroker
                .GetOrCreateBus(MessageBusSampleMessage.BusName)
                .Subscribe<MessageBusSampleMessage>(OnMessage);
        }

        private void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }

        private void OnMessage(MessageBusSampleMessage message)
        {
            Debug.Log("Message Bus sample received: " + message.Text, this);
        }
    }
}

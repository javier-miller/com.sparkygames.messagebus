using System;
using UnityEngine;

namespace SparkyGames.MessageBus.Tests
{
    internal sealed class MessageBusTestMessage : IMessage { }

    public sealed class MessageBusTestReceiver : MonoBehaviour
    {
        private const string BusName = "MessageBusPlayModeTests";
        private IDisposable _subscription;

        public int ReceivedCount { get; private set; }

        private void OnEnable()
        {
            _subscription = UnityMessageBroker.GetOrCreateBus(BusName)
                .Subscribe<MessageBusTestMessage>(_ => ReceivedCount++);
        }

        private void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}

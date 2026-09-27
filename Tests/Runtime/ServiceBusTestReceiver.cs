using System;
using UnityEngine;

namespace SparkyGames.UnityServiceBus.Tests
{
    internal sealed class ServiceBusTestMessage : IMessage { }

    public sealed class ServiceBusTestReceiver : MonoBehaviour
    {
        private const string BusName = "ServiceBusPlayModeTests";
        private IDisposable _subscription;

        public int ReceivedCount { get; private set; }

        private void OnEnable()
        {
            _subscription = UnityMessageBroker.GetOrCreateBus(BusName)
                .Subscribe<ServiceBusTestMessage>(_ => ReceivedCount++);
        }

        private void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}

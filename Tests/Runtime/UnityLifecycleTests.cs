using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace SparkyGames.UnityServiceBus.Tests
{
    public sealed class UnityLifecycleTests
    {
        private const string BusName = "ServiceBusPlayModeTests";

        [UnityTest]
        public IEnumerator DestroyingOrDisablingOneComponentKeepsOtherSubscribers()
        {
            var firstObject = new GameObject("first receiver");
            var secondObject = new GameObject("second receiver");
            var first = firstObject.AddComponent<ServiceBusTestReceiver>();
            var second = secondObject.AddComponent<ServiceBusTestReceiver>();
            var bus = UnityMessageBroker.GetOrCreateBus(BusName);

            bus.Publish(new ServiceBusTestMessage());
            Assert.That(first.ReceivedCount, Is.EqualTo(1));
            Assert.That(second.ReceivedCount, Is.EqualTo(1));

            UnityEngine.Object.Destroy(firstObject);
            yield return null;

            bus.Publish(new ServiceBusTestMessage());
            Assert.That(second.ReceivedCount, Is.EqualTo(2));
            Assert.That(UnityMessageBroker.GetOrCreateBus(BusName), Is.SameAs(bus));

            secondObject.SetActive(false);
            bus.Publish(new ServiceBusTestMessage());
            Assert.That(second.ReceivedCount, Is.EqualTo(2));

            secondObject.SetActive(true);
            bus.Publish(new ServiceBusTestMessage());
            Assert.That(second.ReceivedCount, Is.EqualTo(3));

            UnityEngine.Object.Destroy(secondObject);
            yield return null;
            UnityMessageBroker.RemoveBus(BusName);
        }

        [Test]
        public void PlaySessionResetDisposesGlobalBusesAndClearsRegistry()
        {
            var name = "servicebus-reset-" + Guid.NewGuid();
            var oldBus = UnityMessageBroker.GetOrCreateBus(name);
            var reset = typeof(UnityMessageBroker).GetMethod(
                "ResetForPlaySession", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.That(reset, Is.Not.Null);

            reset.Invoke(null, null);

            Assert.Throws<ObjectDisposedException>(() => oldBus.Publish(new ServiceBusTestMessage()));
            Assert.That(UnityMessageBroker.GetAll(), Is.Empty);
            var newBus = UnityMessageBroker.GetOrCreateBus(name);
            Assert.That(newBus, Is.Not.SameAs(oldBus));
            UnityMessageBroker.RemoveBus(name);
        }
    }
}

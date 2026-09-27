using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace SparkyGames.UnityServiceBus.Tests
{
    public sealed class BusTests
    {
        private interface ITaggedMessage : IMessage { }

        private class BaseMessage : IMessage { }

        private sealed class DerivedMessage : BaseMessage, ITaggedMessage { }

        [Test]
        public void PublishDeliversInRegistrationOrderToAssignableAndGeneralSubscribers()
        {
            using (var bus = new Bus("test"))
            {
                var calls = new List<string>();
                bus.Subscribe<ITaggedMessage>(_ => calls.Add("interface"));
                bus.Subscribe<BaseMessage>(_ => calls.Add("base"));
                bus.Subscribe((IMessage _) => calls.Add("all"));

                bus.Publish(new DerivedMessage());

                CollectionAssert.AreEqual(new[] { "interface", "base", "all" }, calls);
            }
        }

        [Test]
        public void ChangesDuringPublishAffectOnlyTheAppropriateDeliveries()
        {
            using (var bus = new Bus("test"))
            {
                var calls = new List<string>();
                IDisposable removed = null;
                bus.Subscribe<BaseMessage>(_ =>
                {
                    calls.Add("first");
                    removed.Dispose();
                    bus.Subscribe<BaseMessage>(__ => calls.Add("new"));
                });
                removed = bus.Subscribe<BaseMessage>(_ => calls.Add("removed"));

                bus.Publish(new BaseMessage());
                CollectionAssert.AreEqual(new[] { "first" }, calls);

                calls.Clear();
                bus.Publish(new BaseMessage());
                CollectionAssert.AreEqual(new[] { "first", "new" }, calls);
            }
        }

        [Test]
        public void NestedPublishUsesCurrentSubscriptionsAndRunsImmediately()
        {
            using (var bus = new Bus("test"))
            {
                var calls = new List<string>();
                bus.Subscribe<BaseMessage>(message =>
                {
                    calls.Add("first " + (message is DerivedMessage ? "nested" : "outer"));
                    if (!(message is DerivedMessage))
                        bus.Publish(new DerivedMessage());
                });
                bus.Subscribe<BaseMessage>(message =>
                    calls.Add("second " + (message is DerivedMessage ? "nested" : "outer")));

                bus.Publish(new BaseMessage());

                CollectionAssert.AreEqual(new[]
                {
                    "first outer", "first nested", "second nested", "second outer"
                }, calls);
            }
        }

        [Test]
        public void SubscriberErrorsAreCollectedAfterEverySubscriberRuns()
        {
            using (var bus = new Bus("test"))
            {
                var reachedLast = false;
                bus.Subscribe<BaseMessage>(_ => throw new InvalidOperationException("first"));
                bus.Subscribe<BaseMessage>(_ => throw new ArgumentException("second"));
                bus.Subscribe<BaseMessage>(_ => reachedLast = true);

                var error = Assert.Throws<AggregateException>(() => bus.Publish(new BaseMessage()));

                Assert.That(reachedLast, Is.True);
                Assert.That(error.InnerExceptions.Count, Is.EqualTo(2));
                Assert.That(error.InnerExceptions[0], Is.TypeOf<InvalidOperationException>());
                Assert.That(error.InnerExceptions[1], Is.TypeOf<ArgumentException>());
            }
        }

        [Test]
        public void DisposingDuringPublishStopsRemainingCallbacks()
        {
            var bus = new Bus("test");
            var reachedLast = false;
            bus.Subscribe<BaseMessage>(_ => bus.Dispose());
            var token = bus.Subscribe<BaseMessage>(_ => reachedLast = true);

            bus.Publish(new BaseMessage());

            Assert.That(reachedLast, Is.False);
            Assert.DoesNotThrow(() => token.Dispose());
            Assert.DoesNotThrow(() => bus.Dispose());
            Assert.Throws<ObjectDisposedException>(() => bus.Publish(new BaseMessage()));
            Assert.Throws<ObjectDisposedException>(() => bus.Subscribe<BaseMessage>(_ => { }));
        }

        [Test]
        public void DisposedSubscriptionNoLongerReceivesMessages()
        {
            using (var bus = new Bus("test"))
            {
                var count = 0;
                var token = bus.Subscribe<BaseMessage>(_ => count++);
                token.Dispose();
                token.Dispose();

                bus.Publish(new BaseMessage());

                Assert.That(count, Is.Zero);
            }
        }

        [Test]
        public void InvalidArgumentsAreRejected()
        {
            Assert.Throws<ArgumentException>(() => new Bus(" "));

            using (var bus = new Bus("test"))
            {
                Assert.Throws<ArgumentNullException>(() => bus.Publish(null));
                Assert.Throws<ArgumentNullException>(() => bus.Subscribe((Action<IMessage>)null));
                Assert.Throws<ArgumentNullException>(() => bus.Subscribe((Action<BaseMessage>)null));
            }
        }

        [Test]
        public void BrokerReusesAndRemovesNamedBuses()
        {
            var name = "servicebus-test-" + Guid.NewGuid();
            var first = UnityMessageBroker.GetOrCreateBus(name);

            try
            {
                Assert.That(UnityMessageBroker.GetOrCreateBus(name), Is.SameAs(first));
                var snapshot = UnityMessageBroker.GetAll();
                Assert.That(snapshot, Does.Contain(first));

                Assert.That(UnityMessageBroker.RemoveBus(name), Is.True);
                Assert.That(UnityMessageBroker.RemoveBus(name), Is.False);
                Assert.That(snapshot, Does.Contain(first));
                Assert.Throws<ObjectDisposedException>(() => first.Publish(new BaseMessage()));

                var replacement = UnityMessageBroker.GetOrCreateBus(name);
                Assert.That(replacement, Is.Not.SameAs(first));
            }
            finally
            {
                UnityMessageBroker.RemoveBus(name);
            }
        }
    }
}

using System;
using System.Collections.Generic;

namespace SparkyGames.UnityServiceBus
{
    /// <summary>
    /// Bus Implementation
    /// </summary>
    /// <seealso cref="IBus" />
    public class Bus : IBus
    {
        private IList<SubscriptionBase> _subscriptionCollection = new List<SubscriptionBase>();

        /// <summary>
        /// Occurs when [disposing].
        /// </summary>
        public event EventHandler<IBus> Disposing;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bus"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public Bus(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Publishes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Publish(IMessage message)
        {
            foreach (var subscription in _subscriptionCollection)
                subscription.Publish(message);
        }

        /// <summary>
        /// Publishes the specified message.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="message">The message.</param>
        public void Publish<TType>(TType message) where TType : class, IMessage
        {
            foreach (var subscription in _subscriptionCollection)
                subscription.Publish(message);
        }

        /// <summary>
        /// Subscribes the specified on message.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="onMessage">The on message.</param>
        /// <returns></returns>
        public ISubscriptionResult Subscribe<TType>(Action<TType> onMessage) where TType : class, IMessage
        {
            var subscription = new Subscription<TType>(onMessage);

            subscription.Disposing += Subscription_Disposing;

            _subscriptionCollection.Add(subscription);

            return subscription;
        }

        /// <summary>
        /// Subscribes the specified on message.
        /// </summary>
        /// <param name="onMessage">The on message.</param>
        /// <returns></returns>
        public ISubscriptionResult Subscribe(Action<IMessage> onMessage)
        {
            var subscription = new Subscription(onMessage);

            subscription.Disposing += Subscription_Disposing;

            _subscriptionCollection.Add(subscription);

            return subscription;
        }

        private void Subscription_Disposing(object sender, EventArgs e)
        {
            var subscription = sender as SubscriptionBase;

            subscription.Disposing -= Subscription_Disposing;
            _subscriptionCollection.Remove(subscription);
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var subscription in _subscriptionCollection)
                    {
                        subscription.Disposing -= Subscription_Disposing;
                        subscription.Dispose();
                    }

                    _subscriptionCollection.Clear();
                }

                disposedValue = true;
                Disposing?.Invoke(this, this);
            }
        }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}

using System;

namespace SparkyGames.UnityServiceBus
{
    /// <summary>
    /// Subscription Implementation
    /// </summary>
    /// <seealso cref="SubscriptionBase" />
    public class Subscription : SubscriptionBase
    {
        private Action<IMessage> _onMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscription"/> class.
        /// </summary>
        /// <param name="onMessage">The on message.</param>
        public Subscription(Action<IMessage> onMessage)
        {
            _onMessage = onMessage;
        }

        /// <summary>
        /// Publishes the specified on message.
        /// </summary>
        /// <param name="onMessage">The on message.</param>
        public override void Publish(IMessage onMessage)
        {
            _onMessage?.Invoke(onMessage);
        }
    }
}

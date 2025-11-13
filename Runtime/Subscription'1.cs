using System;

namespace SparkyGames.UnityServiceBus
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TType">The type of the type.</typeparam>
    /// <seealso cref="SubscriptionBase" />
    public class Subscription<TType> : SubscriptionBase where TType : class, IMessage
    {
        private Action<TType> _onMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscription{TType}"/> class.
        /// </summary>
        /// <param name="onMessage">The on message.</param>
        public Subscription(Action<TType> onMessage)
        {
            _onMessage = onMessage;
        }

        /// <summary>
        /// Publishes the specified on message.
        /// </summary>
        /// <param name="onMessage">The on message.</param>
        public override void Publish(IMessage onMessage)
        {
            if (onMessage is TType value)
                _onMessage(value);
        }
    }
}

using System;

namespace SparkyGames.UnityServiceBus
{
    /// <summary>
    /// Bus Interface
    /// </summary>
    public interface IBus : IDisposable
    {
        /// <summary>
        /// Publishes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Publish(IMessage message);

        /// <summary>
        /// Publishes the specified message.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="message">The message.</param>
        void Publish<TType>(TType message) where TType : class, IMessage;

        /// <summary>
        /// Subscribes the specified on message.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="onMessage">The on message.</param>
        /// <returns></returns>
        ISubscriptionResult Subscribe<TType>(Action<TType> onMessage) where TType : class, IMessage;

        /// <summary>
        /// Subscribes the specified on message.
        /// </summary>
        /// <param name="onMessage">The on message.</param>
        /// <returns></returns>
        ISubscriptionResult Subscribe(Action<IMessage> onMessage);
    }
}

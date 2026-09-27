using System;

namespace SparkyGames.UnityServiceBus
{
    /// <summary>Publishes messages and owns their subscriptions.</summary>
    public interface IBus : IDisposable
    {
        /// <summary>Delivers a message synchronously to matching subscribers.</summary>
        void Publish(IMessage message);

        /// <summary>Subscribes to a message type and its assignable subtypes.</summary>
        IDisposable Subscribe<TMessage>(Action<TMessage> onMessage)
            where TMessage : class, IMessage;

        /// <summary>Subscribes to every message.</summary>
        IDisposable Subscribe(Action<IMessage> onMessage);
    }
}

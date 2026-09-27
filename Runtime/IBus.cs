using System;

namespace SparkyGames.MessageBus
{
    /// <summary>Publishes messages and manages subscriptions without owning the bus lifetime.</summary>
    public interface IBus
    {
        /// <summary>Delivers a message synchronously to matching subscribers.</summary>
        void Publish(IMessage message);

        /// <summary>Subscribes to a message type and its assignable subtypes.</summary>
        IDisposable Subscribe<TMessage>(Action<TMessage> onMessage)
            where TMessage : IMessage;

        /// <summary>Subscribes to every message.</summary>
        IDisposable Subscribe(Action<IMessage> onMessage);
    }
}

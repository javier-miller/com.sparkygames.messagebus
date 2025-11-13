namespace SparkyGames.UnityServiceBus
{
    /// <summary>
    /// Subscription Interface
    /// </summary>
    public interface ISubscription
    {
        /// <summary>
        /// Publishes the specified on message.
        /// </summary>
        /// <param name="onMessage">The on message.</param>
        void Publish(IMessage onMessage);
    }
}

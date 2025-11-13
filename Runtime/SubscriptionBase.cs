using System;

namespace SparkyGames.UnityServiceBus
{
    /// <summary>
    /// Subscription Base
    /// </summary>
    public abstract class SubscriptionBase : ISubscription, ISubscriptionResult
    {
        /// <summary>
        /// Occurs when [disposing].
        /// </summary>
        public event EventHandler Disposing;

        /// <summary>
        /// Publishes the specified on message.
        /// </summary>
        /// <param name="onMessage">The on message.</param>
        public abstract void Publish(IMessage onMessage);

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Disposing?.Invoke(this, new EventArgs());
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        #endregion
    }
}

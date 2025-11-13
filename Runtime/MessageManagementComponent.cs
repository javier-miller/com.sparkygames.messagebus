using System;
using UnityEngine;

namespace SparkyGames.UnityServiceBus
{
    /// <summary>
    /// Message Management Component
    /// </summary>
    /// <seealso cref="MonoBehaviour" />
    public class MessageManagementComponent : MonoBehaviour
    {
        private Action<IMessage> _handler;
        private IBus _bus;

        /// <summary>
        /// The bus name
        /// </summary>
        public string _busName = "Default";

        /// <summary>
        /// Handlers the message.
        /// </summary>
        /// <param name="handler">The handler.</param>
        public void HandlerMessage(Action<IMessage> handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        private void Start()
        {
            var busName = string.IsNullOrEmpty(_busName) ? "Default" : _busName;
            _bus = UnityMessageBroker.CreateBus(busName);

            _bus.Subscribe((x) =>
            {
                _handler?.Invoke(x);
            });
        }

        /// <summary>
        /// Called when [destroy].
        /// </summary>
        private void OnDestroy()
        {
            if (_bus != null)
                _bus.Dispose();
        }
    }
}

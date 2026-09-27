using System;
using System.Collections.Generic;

namespace SparkyGames.MessageBus
{
    /// <summary>
    /// Delivers messages synchronously to subscribers in registration order.
    /// A Bus is owned and disposed by the code that creates it.
    /// </summary>
    public sealed class Bus : IBus
    {
        private Subscription[] _subscriptions = Array.Empty<Subscription>();
        private bool _isDisposed;

        internal event Action<Bus> Disposed;

        public Bus(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("A bus name is required.", nameof(name));

            Name = name;
        }

        public string Name { get; }

        public void Publish(IMessage message)
        {
            ThrowIfDisposed();
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            // Mutations replace the array. This publication keeps its original order,
            // while disposed entries become inactive immediately.
            var snapshot = _subscriptions;
            List<Exception> errors = null;

            foreach (var subscription in snapshot)
            {
                if (!subscription.IsActive)
                    continue;

                try
                {
                    subscription.Deliver(message);
                }
                catch (Exception error)
                {
                    if (errors == null)
                        errors = new List<Exception>();
                    errors.Add(error);
                }
            }

            if (errors != null)
                throw new AggregateException("One or more message subscribers failed.", errors);
        }

        public IDisposable Subscribe<TMessage>(Action<TMessage> onMessage)
            where TMessage : class, IMessage
        {
            ThrowIfDisposed();
            if (onMessage == null)
                throw new ArgumentNullException(nameof(onMessage));

            return AddSubscription(message =>
            {
                if (message is TMessage typedMessage)
                    onMessage(typedMessage);
            });
        }

        public IDisposable Subscribe(Action<IMessage> onMessage)
        {
            ThrowIfDisposed();
            if (onMessage == null)
                throw new ArgumentNullException(nameof(onMessage));

            return AddSubscription(onMessage);
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
            var subscriptions = _subscriptions;
            _subscriptions = Array.Empty<Subscription>();

            foreach (var subscription in subscriptions)
                subscription.Invalidate();

            var disposed = Disposed;
            Disposed = null;
            disposed?.Invoke(this);
        }

        private IDisposable AddSubscription(Action<IMessage> onMessage)
        {
            var subscription = new Subscription(this, onMessage);
            var next = new Subscription[_subscriptions.Length + 1];
            Array.Copy(_subscriptions, next, _subscriptions.Length);
            next[next.Length - 1] = subscription;
            _subscriptions = next;
            return subscription;
        }

        private void Remove(Subscription subscription)
        {
            var current = _subscriptions;
            var index = Array.IndexOf(current, subscription);
            if (index < 0)
                return;

            var next = new Subscription[current.Length - 1];
            if (index > 0)
                Array.Copy(current, 0, next, 0, index);
            if (index < next.Length)
                Array.Copy(current, index + 1, next, index, next.Length - index);
            _subscriptions = next;
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(Bus));
        }

        private sealed class Subscription : IDisposable
        {
            private Bus _owner;
            private Action<IMessage> _onMessage;

            internal Subscription(Bus owner, Action<IMessage> onMessage)
            {
                _owner = owner;
                _onMessage = onMessage;
            }

            internal bool IsActive => _onMessage != null;

            internal void Deliver(IMessage message)
            {
                _onMessage?.Invoke(message);
            }

            internal void Invalidate()
            {
                _owner = null;
                _onMessage = null;
            }

            public void Dispose()
            {
                var owner = _owner;
                if (owner == null)
                    return;

                Invalidate();
                owner.Remove(this);
            }
        }
    }
}

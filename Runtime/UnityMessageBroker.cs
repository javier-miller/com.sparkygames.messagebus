using System.Collections.Generic;
using System.Linq;

namespace SparkyGames.UnityServiceBus
{
    /// <summary>
    /// Unity Message Broker
    /// </summary>
    public static class UnityMessageBroker
    {
        private static IDictionary<string, IBus> _busCollection = new Dictionary<string, IBus>();

        /// <summary>
        /// Creates the bus.
        /// </summary>
        /// <returns></returns>
        public static IBus CreateBus()
        {
            return CreateBus("Default");
        }

        /// <summary>
        /// Creates the bus.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static IBus CreateBus(string name)
        {
            if (!_busCollection.ContainsKey(name))
            {
                var bus = new Bus(name);
                _busCollection.Add(name, bus);

                bus.Disposing += Bus_Disposing;

                return bus;
            }

            return _busCollection[name];
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IBus> GetAll() => _busCollection.Values.ToList();

        /// <summary>
        /// Buses the disposing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private static void Bus_Disposing(object sender, IBus e)
        {
            var bus = e as Bus;
            bus.Disposing -= Bus_Disposing;
            _busCollection.Remove(bus.Name);
        }
    }
}

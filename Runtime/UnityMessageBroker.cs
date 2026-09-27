using System;
using System.Collections.Generic;
using UnityEngine;

namespace SparkyGames.MessageBus
{
    /// <summary>Provides named buses shared for one Unity Play session.</summary>
    public static class UnityMessageBroker
    {
        private static readonly Dictionary<string, Bus> Buses =
            new Dictionary<string, Bus>(StringComparer.Ordinal);

        public static IBus GetOrCreateBus()
        {
            return GetOrCreateBus("Default");
        }

        public static IBus GetOrCreateBus(string name)
        {
            ValidateName(name);

            if (Buses.TryGetValue(name, out var existing))
                return existing;

            var bus = new Bus(name);
            bus.Disposed += OnBusDisposed;
            Buses.Add(name, bus);
            return bus;
        }

        /// <summary>Disposes and removes a named global bus. Existing users become invalid.</summary>
        public static bool RemoveBus(string name)
        {
            ValidateName(name);
            if (!Buses.TryGetValue(name, out var bus))
                return false;

            bus.Dispose();
            return true;
        }

        /// <summary>Returns a snapshot of the currently registered buses.</summary>
        public static IReadOnlyList<IBus> GetAll()
        {
            var snapshot = new List<IBus>(Buses.Count);
            foreach (var bus in Buses.Values)
                snapshot.Add(bus);
            return snapshot;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetForPlaySession()
        {
            var snapshot = new List<Bus>(Buses.Values);
            Buses.Clear();

            foreach (var bus in snapshot)
            {
                bus.Disposed -= OnBusDisposed;
                bus.Dispose();
            }
        }

        private static void OnBusDisposed(Bus bus)
        {
            if (Buses.TryGetValue(bus.Name, out var current) && ReferenceEquals(current, bus))
                Buses.Remove(bus.Name);
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("A bus name is required.", nameof(name));
        }
    }
}

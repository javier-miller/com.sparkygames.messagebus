using System;

namespace SparkyGames.UnityServiceBus
{
    /// <summary>
    /// Message Interface
    /// </summary>
    public interface IMessage
    {
        string Id { get; }

        DateTime Date { get; }
    }
}

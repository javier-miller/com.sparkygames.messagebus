using System;

namespace SparkyGames.UnityServiceBus
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IMessage" />
    public abstract class MessageBase<TType> : IMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBase"/> class.
        /// </summary>
        protected MessageBase() : this(Guid.NewGuid().ToString(), DateTime.Now)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBase"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        protected MessageBase(string id) : this(id, DateTime.Now)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBase"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="date">The date.</param>
        /// <exception cref="ArgumentNullException">
        /// id
        /// or
        /// date
        /// </exception>
        protected MessageBase(string id, DateTime date)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
            if (date.Equals(DateTime.MinValue)) throw new ArgumentNullException(nameof(date));

            Id = id;
            Date = date;
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; protected set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        public DateTime Date { get; protected set; }
    }
}

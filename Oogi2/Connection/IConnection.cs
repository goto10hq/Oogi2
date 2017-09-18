using Microsoft.Azure.Documents.Client;

namespace Oogi2
{
    /// <summary>
    /// Connection interface.
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// Gets the documentdb client.
        /// </summary>
        /// <value>The documentdb client.</value>
        DocumentClient Client { get; }

        /// <summary>
        /// Gets the database identifier.
        /// </summary>
        /// <value>The database identifier.</value>
        string DatabaseId { get; }

        /// <summary>
        /// Gets the collection identifier.
        /// </summary>
        /// <value>The collection identifier.</value>
        string CollectionId { get; }
    }
}

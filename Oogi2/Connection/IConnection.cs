using Microsoft.Azure.Documents.Client;

namespace Oogi2
{
    public interface IConnection
    {
        DocumentClient Client { get; }
        string DatabaseId { get; }
        string CollectionId { get; }
    }
}

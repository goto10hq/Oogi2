using Microsoft.Azure.Documents;

namespace Oogi2.Queries
{
    public interface IQuery
    {
        SqlQuerySpec ToSqlQuerySpec();
        SqlQuerySpec ToGetFirstOrDefault();        
    }
}

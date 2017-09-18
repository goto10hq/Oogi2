using Microsoft.Azure.Documents;

namespace Oogi2.Queries
{
    public class IdQuery<T> : IQuery where T : class
    {
        readonly string _id;        

        public IdQuery(string id)
        {            
            _id = id;            
        }

        public SqlQuerySpec ToSqlQuerySpec()
        {               
            return new SqlQuerySpec("select * from c where c.id = @id", new SqlParameterCollection { new SqlParameter("@id", _id) });
        }

        public SqlQuerySpec ToGetFirstOrDefault()
        {            
            return new SqlQuerySpec("select top 1 * from c where c.id = @id", new SqlParameterCollection { new SqlParameter("@id", _id) });
        }

        public SqlQuerySpec ToGetAll() => throw new System.NotImplementedException();
    }    
}

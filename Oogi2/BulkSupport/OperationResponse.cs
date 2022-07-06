using System;

namespace Oogi2.BulkSupport
{
    public class OperationResponse<T>
    {
        public T Item { get; set; }        
        public bool IsSuccessful { get; set; }
        public Exception CosmosException { get; set; }
    }
}
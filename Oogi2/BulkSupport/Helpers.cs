using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Oogi2.BulkSupport
{
    public static class Helpers
    {
        public static async Task<OperationResponse<T>> CaptureOperationResponse<T>(Task task, T item)
        {
            try
            {
                await task;
                return new OperationResponse<T>()
                {
                    Item = item,
                    IsSuccessful = true                    
                };
            }
            catch (Exception ex)
            {
                if (ex is CosmosException cosmosException)
                {
                    return new OperationResponse<T>()
                    {
                        Item = item,
                        IsSuccessful = false,
                        CosmosException = cosmosException
                    };
                }

                return new OperationResponse<T>()
                {
                    Item = item,
                    IsSuccessful = false,
                    CosmosException = ex
                };
            }
        }
    }
}
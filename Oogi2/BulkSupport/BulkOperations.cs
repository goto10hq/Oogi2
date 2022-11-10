using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Oogi2.BulkSupport
{
    public class BulkOperations<T>
    {
        public readonly List<Task<OperationResponse<T>>> Tasks;

        private readonly Stopwatch stopwatch = Stopwatch.StartNew();

        public BulkOperations(int operationCount)
        {
            Tasks = new List<Task<OperationResponse<T>>>(operationCount);
        }

        public async Task<BulkOperationResponse<T>> ExecuteAsync()
        {
            await Task.WhenAll(Tasks);

            stopwatch.Stop();
            
            return new BulkOperationResponse<T>()
            {
                TotalTimeTaken = stopwatch.Elapsed,
                SuccessfulItems = Tasks.Count(task => task.Result.IsSuccessful),
                Failures = Tasks.Where(task => !task.Result.IsSuccessful).Select(task => (task.Result.Item, task.Result.CosmosException)).ToList()
            };
        }

        public async Task<BulkOperationResponse<T>> ExecuteAsync(int dop)
        {
            using (var semaphore = new SemaphoreSlim(initialCount: dop, maxCount: dop))
            {
                var tasks = Tasks.Select(async item =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        await item;
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });
                await Task.WhenAll(tasks);
            }

            stopwatch.Stop();

            return new BulkOperationResponse<T>()
            {
                TotalTimeTaken = stopwatch.Elapsed,
                SuccessfulItems = Tasks.Count(task => task.Result.IsSuccessful),
                Failures = Tasks.Where(task => !task.Result.IsSuccessful).Select(task => (task.Result.Item, task.Result.CosmosException)).ToList()
            };
        }        
    }
}

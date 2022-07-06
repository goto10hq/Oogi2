using System.Threading.Tasks;

namespace Oogi2.BulkSupport
{
    public class BulkOperation<T>
    {
        public Task Operation { get; set; }
        public T Item { get; set; }

        public BulkOperation(Task operation, T item)
        {
            Operation = operation;
            Item = item;
        }
    }
}
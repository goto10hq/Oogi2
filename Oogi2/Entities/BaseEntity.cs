namespace Oogi2.Entities
{
    public abstract class BaseEntity
    {
        public string Id { get; set; }
        public abstract string PartitionKey { get; }
        public abstract string Entity { get;  }
    }
}
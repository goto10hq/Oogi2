using System;
using Sushi2;

namespace Oogi2.Tokens
{
    public class SimpleStamp : IStamp, IFormattable, IComparable<DateTime>, IComparable<IStamp>, IComparable<object>, IEquatable<DateTime>, IEquatable<IStamp>, IEquatable<object>
    {        
        public DateTime DateTime { get; set; }
        public int Epoch => DateTime.ToEpoch();

        public SimpleStamp()
        {
            DateTime = DateTime.Now;
        }

        public SimpleStamp(DateTime dt)
        {
            DateTime = dt;
        }
        
        public override string ToString()
        {
            return DateTime.ToString();
        }      

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return DateTime.ToString(format, formatProvider);
        }

        public int CompareTo(DateTime other)
        {
            return DateTime.CompareTo(other);
        }

        public bool Equals(DateTime other)
        {
            return DateTime.Equals(other);
        }       

        public bool Equals(IStamp other)
        {
            return other != null && DateTime.Equals(other.DateTime);
        }

        public override int GetHashCode()
        {
            return DateTime.GetHashCode();
        }

        public int CompareTo(IStamp other)
        {
            return DateTime.CompareTo(other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (!(obj is IStamp))
                return false;

            return Equals((IStamp)obj);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(this, obj))
                return 0;

            if (ReferenceEquals(null, obj))
                return 1;

            if (!(obj is IStamp))
                return 1;

            return CompareTo((IStamp)obj);
        }
    }
}
using System;

namespace Oogi2.Exceptions
{
    public class OogiException : Exception
    {        
        public OogiException(string message) : base(message)
        {
        }
    }
}
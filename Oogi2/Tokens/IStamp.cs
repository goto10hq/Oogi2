using System;

namespace Oogi2.Tokens
{
    public interface IStamp
    {
        DateTime DateTime { get; set; }
        int Epoch { get; }        
    }
}

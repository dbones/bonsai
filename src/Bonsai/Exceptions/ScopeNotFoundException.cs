namespace Bonsai.Exceptions
{
    using System;

    public class ScopeNotFoundException : Exception
    {
        public ScopeNotFoundException(string scopeName) : base($"cannot find {scopeName}")
        {           
        }
    }
}
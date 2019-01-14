namespace Bones.Exceptions
{
    using System;
    using System.Reflection;

    public class DuplicateNamedContractException : Exception
    {
        public Type Contract { get; }
        public string Name { get; }

        public DuplicateNamedContractException(Type contract, string name)
        {
            Contract = contract;
            Name = name;
        }
    }
}
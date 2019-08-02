namespace Bonsai.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Internal;
    using LifeStyles;
    using Planning;

    /// <summary>
    /// this contains all the information for every concrete instance which can be resolved from the container.
    /// </summary>
    public class Contract :  IEquatable<Contract>
    {
        private readonly int _hash;

        public Contract(int id)
        {
            _hash = id;
        }


        /// <summary>
        /// all the services this contract will support
        /// </summary>
        public HashSet<ServiceKey> ServiceKeys { get; set; }

        /// <summary>
        /// a delegate which will create an instance
        /// </summary>
        public CreateInstance CreateInstance { get; set; }

        /// <summary>
        /// denotes if this type is disposable.
        /// </summary>
        public bool IsDisposal { get; set; }
        
        /// <summary>
        /// the instance which is provided by the program.
        /// </summary>
        public object Instance { get; set; }

        /// <summary>
        /// this is how the object should live and if is should be disposed of
        /// </summary>
        public ILifeSpan LifeSpan { get; set; }

        /// <summary>
        /// a simple id, mainly used to help with creating this contract and setting it up
        /// </summary>
        public int Id => _hash;

        public bool Equals(Contract other)
        {
            return _hash == other?.GetHashCode();
        }

        public override int GetHashCode()
        {
            return _hash;
        }

        public override bool Equals(object obj)
        {
            //var item = obj as Contract;
            return _hash == obj?.GetHashCode();
        }

        public override string ToString()
        {
            var keys = string.Join(" ", ServiceKeys.Select(x => x.ToString()));
            return $"{LifeSpan?.GetType().Name} {keys} ";
        }
    }
}
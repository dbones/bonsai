namespace Bones.Contracts
{
    using System;
    using System.Collections.Generic;
    using Internal;
    using LifeStyles;
    using PreContainer;

    /// <summary>
    /// this contains all the information for every concrete instance which can be resolved from the container.
    /// </summary>
    public class Contract
    {
        /// <summary>
        /// a delegate which will create an instance
        /// </summary>
        public CreateInstance CreateInstance { get; set; }

        /// <summary>
        /// all the services this contract will support
        /// </summary>
        public HashSet<ServiceKey> ServiceKeys { get; set; }

        /// <summary>
        /// a delegate which will can dispose of the instance
        /// </summary>
        public DisposeInstance DisposeInstance { get; set; }

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
        public string Id { get; set; } = Guid.NewGuid().ToString();
    }
}
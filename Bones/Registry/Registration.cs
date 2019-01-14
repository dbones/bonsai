namespace Bones.Registry
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Internal;
    using LifeStyles;

    public class Registration
    {
        private readonly string _hash = Guid.NewGuid().ToString();

        public Registration()
        {
            ScopedTo = new Singleton();
        }

        public string Id => _hash;

        public override int GetHashCode()
        {
            return _hash.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var t = obj as Registration;
            if (obj == null)
            {
                return false;
            }
            return this.GetHashCode() == obj.GetHashCode();
        }

        /// <summary>
        /// the types (interfaces/concrete types) this registration will support
        /// </summary>
        public HashSet<ServiceKey> Types { get; set; } = new HashSet<ServiceKey>();
//        public Dictionary<string, Type> Types { get; set; } = new Dictionary<string, Type>();
        

        /// <summary>
        /// the implementation which will support this registration
        /// </summary>
        public Type ImplementedType { get; set; }

        /// <summary>
        /// the provided instance which will support this registration, no need for the <see cref="ImplementedType"/>
        /// </summary>
        public object Instance { get; set; }
        
        /// <summary>
        /// the identified constructor to use to create an instance
        /// </summary>
        public MethodBase Constructor { get; set; }
        
        /// <summary>
        /// the set of methods (including properties) which can be injected into
        /// </summary>
        public List<MethodBase> InjectableMethods { get; set; } = new List<MethodBase>();
        
        //public Func<ResolvingContext, object> Constructor { get; set; }

        /// <summary>
        /// the scope of which an instance will live for
        /// </summary>
        public ILifeSpan ScopedTo { get; set; }

        /// <summary>
        /// all the defined dependencies.
        /// </summary>
        public List<RegistrationDependency> Dependencies { get; set; } = new List<RegistrationDependency>();
    }
}
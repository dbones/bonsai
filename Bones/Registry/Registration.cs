namespace Bones
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class Registration
    {
        public Dictionary<string, TypeInfo> Types { get; set; } = new Dictionary<string, TypeInfo>();

        public TypeInfo ImplementedType { get; set; }

        public object Instance { get; set; }

        //public Func<ResolvingContext, object> Constructor { get; set; }

        public ILifeSpan ScopedTo { get; set; }

        public List<RegistrationDependency> Dependencies { get; set; }
    }
}
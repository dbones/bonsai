namespace Bones.PreContainer
{
    using System;
    using System.Collections.Generic;
    using Internal;
    using Registry;

    public class RegistrationContext
    {
        public List<MethodInformation> InjectOnMethods { get; set; } = new List<MethodInformation>();
        public Registration Registration { get; set; }

        public Type ImplementedType { get; set; }

        public HashSet<ServiceKey> Keys { get; set; } = new HashSet<ServiceKey>();
        
        public string Id { get; set; } = Guid.NewGuid().ToString();
    }
}
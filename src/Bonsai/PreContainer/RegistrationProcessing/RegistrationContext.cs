namespace Bonsai.PreContainer.RegistrationProcessing
{
    using System;
    using System.Collections.Generic;
    using Internal;
    using Registry;

    public class RegistrationContext
    {
        public RegistrationContext(int id)
        {
            Id = id;
        }
        
        public List<MethodInformation> InjectOnMethods { get; set; } = new List<MethodInformation>();
        public Registration Registration { get; set; }

        public Type ImplementedType { get; set; }

        public HashSet<ServiceKey> Keys { get; set; } = new HashSet<ServiceKey>();
        
        public int Id { get; }
    }
}
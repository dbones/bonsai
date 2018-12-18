namespace Bones
{
    using System.Reflection;

    public class RegistrationDependency
    {
        public string ParameterName { get; set; }
        public TypeInfo RequiredType { get; set; }
        public string AttributeName { get; set; }

        public TypeInfo ImplementedType { get; set; }
        public string Named { get; set; }
        public object Value { get; set; }

        //public InjectType Type { get; set; }
    }
}
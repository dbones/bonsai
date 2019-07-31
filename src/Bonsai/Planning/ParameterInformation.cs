namespace Bonsai.Planning
{
    using System;
    using Contracts;
    using Internal;

    public class ParameterInformation
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public ServiceKey ServiceKey { get; set; }
        public CreateInstance CreateInstance { get; set; }

        public Type ProvidedType { get; set; }
    }
}
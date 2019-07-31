namespace Bonsai.Planning
{
    using System.Collections.Generic;
    using System.Reflection;
    using Registry;

    public class MethodInformation
    {
        public InjectOn InjectOn { get; set; }

        public string Name { get; set; }

        public List<ParameterInformation> Parameters { get; set; } = new List<ParameterInformation>();

        /// <summary>
        /// reference to the method (setter method)
        /// </summary>
        public MethodBase Method { get; set; }
    }
}
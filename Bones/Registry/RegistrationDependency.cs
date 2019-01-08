namespace Bones
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class RegistrationDependency
    {
        public RegistrationDependency(){
            MethodPredicates = new List<Predicate<MethodBase>>();
            ParameterPredicates = new List<Predicate<ParameterInfo>>();
        }


        /// <summary>
        /// the name of the parameter which we are providing information on
        /// </summary>
        public string ParameterName { get; set; }
        /// <summary>
        /// the type we are required to override (i.e. for the ILogger do this please)
        /// </summary>
        public Type RequiredType { get; set; }
        /// <summary>
        /// the name of the property, method which we are providing information on
        /// </summary>
        public string AttributeName { get; set; }
        

        /// <summary>
        /// the type we want to provide (ensure we have registered it)
        /// </summary>
        public Type ImplementedType { get; set; }
        /// <summary>
        /// the implementations named instance
        /// </summary>
        public string Named { get; set; }
        /// <summary>
        /// an instance/value to use
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// the injection we are going to apply
        /// </summary>
        public InjectOn InjectOn { get; set; }


        public List<Predicate<MethodBase>> MethodPredicates {get;}
        public List<Predicate<ParameterInfo>> ParameterPredicates {get;}
    }
}
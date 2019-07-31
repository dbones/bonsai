namespace Bonsai.Exceptions
{
    using System;
    using System.Linq;

    public class CannotFindSupportableConstructorException : Exception
    {
        public CannotFindSupportableConstructorException(Type type)
        {
            var constructors = type.GetConstructors();
            var possibleCtors = string.Join("\n ", constructors.Select(x => {
                var parameters = string.Join(", ", x.GetParameters().Select(p => $"{p.ParameterType.FullName} {p.Name}"));
                return $"{x.Name} [{parameters}]";
            }));
            string error = $"type: {type.FullName}, found ctors:\n{possibleCtors}";
            Message = error;
        }

        public override string Message {get;}
    }
}
namespace Bonsai.Planning
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Contracts;
    using Exceptions;
    using FastExpressionCompiler;
    using RegistrationProcessing;
    using Registry;

    public class FuncConstruct : IConstruct
    {
        public bool CanSupport(RegistrationContext context)
        {
            return true;
        }

        public CreateInstance Create(RegistrationContext context, IEnumerable<Contract> contracts)
        {
            contracts = contracts.ToList();

            var ctor = context.InjectOnMethods.First(x => x.InjectOn == InjectOn.Constructor);

            var createParams = new List<Expression>();
            var parameters = ctor.Parameters;

            var scopeParam = Expression.Parameter(typeof(IAdvancedScope));
            var parentContractParam = Expression.Parameter(typeof(Contract));
            var unAssignedContractParam = Expression.Parameter(typeof(Contract));
            var nullExpression = Expression.Constant(null);
            var nullContractExpression = Expression.Convert(nullExpression, typeof(Contract));

            foreach (var param in parameters)
            {
                var p = param;

                //we have been provided a value in the registration
                if (p.Value != null)
                {
                    var provided = Expression.Constant(p.Value);
                    var cast = Expression.Convert(provided, p.Value.GetType());
                    createParams.Add(cast);
                    continue;
                }

                //registration provided a delegate.
                if (p.CreateInstance != null)
                {
                    var provided = Expression.Constant(p.CreateInstance);
                    var suppliedDelegate = Expression.Invoke(provided,
                        scopeParam,
                        nullContractExpression,
                        parentContractParam);
                    var con = Expression.Convert(suppliedDelegate, p.ProvidedType);

                    createParams.Add(con);
                    continue;
                }

                //ok now we find the contract, so we can create a resolve expression
                var contract = contracts.FirstOrDefault(x => x.ServiceKeys.Contains(p.ServiceKey));
                if (contract == null) throw new MissingContractException(p.ServiceKey);

                var resolve = typeof(IAdvancedScope).GetMethod("Resolve", new[]
                {
                    typeof(Contract),
                    typeof(Contract)
                });

                Expression constantExpr = Expression.Constant(contract);
                MethodCallExpression resolveParam = Expression.Call(
                    scopeParam,
                    resolve,
                    constantExpr,
                    parentContractParam
                );

                var convert = Expression.Convert(resolveParam, p.ServiceKey.Service);
                createParams.Add(convert);
            }

            var method = (ConstructorInfo)ctor.Method;
            var newExpression =
                Expression.New(method, createParams);

            var compiledCtor = Expression
                .Lambda<CreateInstance>(
                    newExpression,
                    scopeParam,
                    parentContractParam,
                    unAssignedContractParam)
                .CompileFast(); //this is used over Compile, as it performs a little faster

            return compiledCtor;
        }
    }
}
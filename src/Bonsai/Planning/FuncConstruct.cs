namespace Bonsai.Planning
{
    using System;
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
            //var parentContract = contracts.First(x => x.ServiceKeys.Contains(context.Keys.First()));

            List<Expression> createParams = new List<Expression>();

            var parameters = ctor.Parameters;

            var scopeParam = Expression.Parameter(typeof(IAdvancedScope));
            var parentContractParam = Expression.Parameter(typeof(Contract));
            var unAssignedContractParam = Expression.Parameter(typeof(Contract));
            var nullExpression = Expression.Constant(null);
            var nullContractExpression = Expression.Convert(nullExpression, typeof(Contract));

            foreach (var param in parameters)
            {
                var p = param;


                if (p.Value != null)
                {
                    var provided = Expression.Constant(p.Value);
                    var cast = Expression.Convert(provided, p.Value.GetType());
                    createParams.Add(cast);
                    continue;
                }


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

                Contract contract;

                try
                {
                    contract = contracts.First(x => x.ServiceKeys.Contains(p.ServiceKey));
                }
                catch (Exception)
                {
                    throw new MissingContractException(p.ServiceKey);
                }

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

            var compiledCtor = Expression.Lambda<CreateInstance>(
                    newExpression,
                    scopeParam,
                    parentContractParam,
                    unAssignedContractParam)
                .CompileFast();

            //object ParameterLessCtor(IAdvancedScope scope, Contract ct, Contract pct) => compiledCtor(scope, ct);
            return compiledCtor;
        }
    }
}
namespace Bonsai.PreContainer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Contracts;
    using FastExpressionCompiler;
    using Registry;

    public class DelegateBuilder
    {
        public IEnumerable<Contract> Create(IEnumerable<RegistrationContext> contexts)
        {
            var contracts = new List<Contract>();
            foreach (var context in contexts)
            {
                var contract = new Contract()
                {
                    Id = context.Id,
                    LifeSpan = context.Registration.ScopedTo,
                    ServiceKeys = context.Keys,
                    
                    Instance = context.Registration.Instance
                };

                if (typeof(IDisposable).IsAssignableFrom(context.ImplementedType))
                {
                    contract.DisposeInstance = Disposal;
                }
                else
                {
                    contract.DisposeInstance = NoOpDisposal;
                }

                contracts.Add( contract);
            }


            foreach (var context in contexts)
            {
                var contract = contracts.First(x => x.Id == context.Id);
                contract.CreateInstance = Create(context, contracts);
            }

            return contracts;
        }

        void NoOpDisposal(object instance)
        {
        }

        void Disposal(object instance)
        {
            ((IDisposable) instance).Dispose();
        }


        CreateInstance Create(RegistrationContext context, IEnumerable<Contract> contracts)
        {
            if (context.Registration.Instance != null) return null;

            var ctor = context.InjectOnMethods.First(x => x.InjectOn == InjectOn.Constructor);

            //List<Func<Scope, object>> createParams = new List<Func<Scope, object>>();
            List<Expression> createParams = new List<Expression>();

            var parameters = ctor.Parameters;

            
            var scopeParam = Expression.Parameter(typeof(IAdvancedScope));
            var resolve = typeof(IAdvancedScope).GetMethod("Resolve", new Type[] {typeof(Contract)});
            foreach (var param in parameters)
            {
                var p = param;
                var contract = contracts.First(x => x.ServiceKeys.Contains(p.ServiceKey));
                
                
                Expression constantExpr = Expression.Constant(contract);
                MethodCallExpression resolveParam = Expression.Call(
                    scopeParam,
                    resolve,
                    constantExpr
                );

                

                var convert = Expression.Convert(resolveParam, p.ServiceKey.Service);
                
                createParams.Add(convert);
            }
            
            var method = (ConstructorInfo) ctor.Method;
            var newExpression =
                Expression.New(method, createParams);
            
            var compiledCtor = Expression.Lambda<Func<IAdvancedScope, object>>(newExpression, scopeParam).CompileFast();

            object ParameterLessCtor(IAdvancedScope scope) => compiledCtor(scope);
            return ParameterLessCtor;
        }
        
    }
}
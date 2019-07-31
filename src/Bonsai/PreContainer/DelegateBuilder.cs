namespace Bonsai.PreContainer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Contracts;
    using Exceptions;
    using FastExpressionCompiler;
    using RegistrationProcessing;
    using Registry;



    public class ContractBuilder
    {

    }



    public class DelegateBuilder
    {
        public List<IConstruct> _constructs = new List<IConstruct>() { new FuncConstruct() };

        public void SetDelegates(ICollection<RegistrationContext> contexts, ICollection<Contract> contracts, Contract contract)
        {
            var context = contexts.First(x => x.Id == contract.Id);

            if (context.Registration.Instance != null)
            {
                //provided instance
                //do nothing
            }
            else if (context.Registration.CreateInstance != null)
            {
                //provided a ctor
                contract.CreateInstance = context.Registration.CreateInstance;
            }
            else
            {
                //need to create a delegate
                contract.CreateInstance = _constructs.First(x => x.CanSupport(context)).Create(context, contracts);
            }


            if (typeof(IDisposable).IsAssignableFrom(context.ImplementedType))
            {
                contract.IsDisposal = true;
                contract.DisposeInstance = Disposal;
            }
            else
            {
                contract.IsDisposal = false;
                contract.DisposeInstance = NoOpDisposal;
            }
        }

        /// <summary>
        /// this is a simple do nothing on disposal method
        /// </summary>
        /// <param name="instance">the instance which this method will execute against</param>
        private static void NoOpDisposal(object instance)
        {
            //TODO: look to improve on how we track disposables.
        }

        /// <summary>
        /// this will invoke the disposable on a method.
        /// </summary>
        /// <param name="instance">the instance which this method will execute against</param>
        private static void Disposal(object instance)
        {
            ((IDisposable)instance).Dispose();
        }

    }

    public interface IConstruct
    {
        bool CanSupport(RegistrationContext context);
        CreateInstance Create(RegistrationContext context, IEnumerable<Contract> contracts);
    }

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

    /*
    public class IlConstruct : IConstruct
    {
        public bool CanSupport(RegistrationContext context)
        {
            return true;
        }

        public CreateInstance Create(RegistrationContext context, IEnumerable<Contract> contracts)
        {
            return null;
        }


        public void asdf(RegistrationContext context)
        {
            var ctor = context.InjectOnMethods.First(x => x.InjectOn == InjectOn.Constructor);

            DynamicMethod dmethod = new DynamicMethod(Guid.NewGuid().ToString(), context.ImplementedType,  new []{typeof(object[])}, false);
            var il = dmethod.GetILGenerator();

            
            int i = 0;
            foreach (var parameter in ctor.Parameters)
            {
                var p = parameter;
                
                
                

                if (p.Value != null)
                {
                    //il.DeclareLocal()
                    //il.Emit(OpCodes.Ldobj, p.Value);
                    
                    
                    var provided = Expression.Constant(p.Value);
                    var cast = Expression.Convert(provided, p.Value.GetType());
                    //createParams.Add(cast);
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
                catch (Exception e)
                {
                    throw new MissingContractException(p.ServiceKey);
                }
                
                
                il.Emit(OpCodes.Ldarg_0); //getscope
                
                il.Emit(OpCodes.Ldelem_Ref); //call index method

                //un-box or cast accordingly
                il.Emit(parameter.ServiceKey.Service.IsValueType ? 
                        OpCodes.Unbox_Any : OpCodes.Castclass, 
                    parameter.ServiceKey.Service);
                i++;
            }
            
            il.Emit(OpCodes.Newobj, (ConstructorInfo)ctor.Method); //call ctor (passing all the parameters on the stack)
            il.Emit(OpCodes.Ret);

            var ctorDelegate = (CreateObjectDelegate)dmethod.CreateDelegate(typeof(CreateObjectDelegate));
        }
        
        

        public void asd(RegistrationContext context)
        {
            var ctor = context.InjectOnMethods.First(x => x.InjectOn == InjectOn.Constructor);

            DynamicMethod dmethod = new DynamicMethod(Guid.NewGuid().ToString(), context.ImplementedType,  new []{typeof(object[])}, false);
            var il = dmethod.GetILGenerator();

            
            int i = 0;
            foreach (var parameter in ctor.Parameters)
            {
                il.Emit(OpCodes.Ldarg_0); //get parameters
                switch (i) //load index
                {
                    case 0: il.Emit(OpCodes.Ldc_I4_0); break;
                    case 1: il.Emit(OpCodes.Ldc_I4_1); break;
                    case 2: il.Emit(OpCodes.Ldc_I4_2); break;
                    case 3: il.Emit(OpCodes.Ldc_I4_3); break;
                    case 4: il.Emit(OpCodes.Ldc_I4_4); break;
                    case 5: il.Emit(OpCodes.Ldc_I4_5); break;
                    case 6: il.Emit(OpCodes.Ldc_I4_6); break;
                    case 7: il.Emit(OpCodes.Ldc_I4_7); break;
                    case 8: il.Emit(OpCodes.Ldc_I4_8); break;
                    default: il.Emit(OpCodes.Ldc_I4, i); break;
                }
                il.Emit(OpCodes.Ldelem_Ref); //call index method

                //un-box or cast accordingly
                il.Emit(parameter.ServiceKey.Service.IsValueType ? 
                        OpCodes.Unbox_Any : OpCodes.Castclass, 
                    parameter.ServiceKey.Service);
                i++;
            }
            
            il.Emit(OpCodes.Newobj, (ConstructorInfo)ctor.Method); //call ctor (passing all the parameters on the stack)
            il.Emit(OpCodes.Ret);

            var ctorDelegate = (CreateObjectDelegate)dmethod.CreateDelegate(typeof(CreateObjectDelegate));
        }
        private delegate object CreateObjectDelegate(object[] parameters);   
    }
    */
}
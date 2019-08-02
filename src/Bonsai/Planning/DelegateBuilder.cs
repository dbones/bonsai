namespace Bonsai.Planning
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using Contracts;
    using Exceptions;
    using RegistrationProcessing;
    using Registry;

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

            contract.IsDisposal = typeof(IDisposable).IsAssignableFrom(context.ImplementedType);
        }


    }

    
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

        /*
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
*/        
        

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
    
}
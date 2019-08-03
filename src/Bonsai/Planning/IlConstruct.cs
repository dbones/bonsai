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

    public delegate object DynamicMethodCtor(
        IAdvancedScope scope,
        Contract contract,
        List<object> paramsCache,
        List<Contract> contractCache,
        List<CreateInstance> createInstancesCache);

    public class IlConstruct : IConstruct
    {
        private static int count = 0;

        public bool CanSupport(RegistrationContext context)
        {
            return true;
        }

        public CreateInstance Create(RegistrationContext context, IEnumerable<Contract> contracts)
        {
            contracts = contracts.ToList();
            var ctor = context.InjectOnMethods.First(x => x.InjectOn == InjectOn.Constructor);
            var methodName = $"{context.ImplementedType.FullName.Replace(".", "_")}_ctor_{count}";

            var paramsCache = new List<object>();
            var getParamsIndexer = typeof(List<object>).GetMethod("get_Item", BindingFlags.Public | BindingFlags.Instance);

            var contractCache = new List<Contract>();
            var getContractsIndexer = typeof(List<Contract>).GetMethod("get_Item", BindingFlags.Public | BindingFlags.Instance);

            var createInstancesCache = new List<CreateInstance>();
            var getCreateInstancesIndexer = typeof(List<CreateInstance>).GetMethod("get_Item", BindingFlags.Public | BindingFlags.Instance);
            var invoke = typeof(CreateInstance).GetMethod("Invoke");

            var resolve = typeof(IAdvancedScope).GetMethod("Resolve", new[]
            {
                typeof(Contract),
                typeof(Contract)
            });


            DynamicMethod dmethod = new DynamicMethod(
                methodName, 
                typeof(object),
                new[] { typeof(IAdvancedScope), typeof(Contract), typeof(List<object>), typeof(List<Contract>), typeof(List<CreateInstance>) }, 
                false);

            var il = dmethod.GetILGenerator();

            foreach (var param in ctor.Parameters)
            {
                var p = param;
                int posistion;

                if (p.Value != null)
                {
                    posistion = paramsCache.Count;
                    // IL_0000: ldarg.3      // paramsCache
                    // IL_0001: ldc.i4.4
                    // IL_0002: callvirt     Generic.List`1<object>::get_Item(int32)
                    // IL_0007: castclass    class Bonsai.Benchmarks.Models.Repository`1<class Bonsai.Benchmarks.Models.User>

                    //get value from paramsCache (which we just added)
                    LoadArg(il, 3);

                    LoadIndex(il, posistion);
                    il.Emit(OpCodes.Callvirt, getParamsIndexer);
                    il.Emit(p.ProvidedType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, p.ProvidedType);

                    paramsCache.Add(p.Value);
                    continue;
                }

                if (p.CreateInstance != null)
                {
                    posistion = createInstancesCache.Count;
                    // IL_000c: ldarg.s      createInstancesCache
                    // IL_000e: ldc.i4       3455 // 0x00000d7f
                    // IL_0013: callvirt     Generic.List`1<class Bonsai.Benchmarks.CreateInstance>::get_Item(int32)
                    // IL_0018: ldarg.1      // scope
                    // IL_0019: ldnull
                    // IL_001a: ldarg.2      // contract
                    // IL_001b: callvirt     instance object Bonsai.Benchmarks.CreateInstance::Invoke(class [Bonsai]Bonsai.IAdvancedScope, class [Bonsai]Bonsai.Contracts.Contract, class [Bonsai]Bonsai.Contracts.Contract)
                    // IL_0020: castclass    Bonsai.Benchmarks.Models.Logger

                    LoadArg(il, 5);

                    LoadIndex(il, posistion);
                    il.Emit(OpCodes.Callvirt, getCreateInstancesIndexer);

                    LoadArg(il, 1);
                    il.Emit(OpCodes.Ldnull);
                    LoadArg(il, 2);

                    il.Emit(OpCodes.Callvirt, invoke);
                    il.Emit(p.ProvidedType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, p.ProvidedType);

                    createInstancesCache.Add(p.CreateInstance);
                    continue;
                }

                //ok now we find the contract, so we can create a resolve expression
                var contract = contracts.FirstOrDefault(x => x.ServiceKeys.Contains(p.ServiceKey));
                if (contract == null) throw new MissingContractException(p.ServiceKey);

                posistion = contractCache.Count;
                // IL_0000: ldarg.1      // scope
                // IL_0001: ldarg.s      contractCache
                // IL_0003: ldc.i4       487 // 0x000001e7
                // IL_0008: callvirt     Generic.List`1<class [Bonsai]Bonsai.Contracts.Contract>::get_Item(int32)
                // IL_000d: ldarg.2      // contract
                // IL_000e: callvirt     instance object [Bonsai]Bonsai.IAdvancedScope::Resolve(class [Bonsai]Bonsai.Contracts.Contract, class [Bonsai]Bonsai.Contracts.Contract)
                // IL_0013: castclass    class Bonsai.Benchmarks.Models.Repository`1<class Bonsai.Benchmarks.Models.User>

                LoadArg(il, 1); //scope

                LoadArg(il, 4); // contractCache
                LoadIndex(il, posistion);
                il.Emit(OpCodes.Callvirt, getContractsIndexer);

                LoadArg(il, 2);
                il.Emit(OpCodes.Callvirt, resolve);
                il.Emit(p.ServiceKey.Service.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, p.ServiceKey.Service);
                contractCache.Add(contract);
            }


            il.Emit(OpCodes.Newobj, (ConstructorInfo)ctor.Method); //call ctor (passing all the parameters on the stack)
            il.Emit(OpCodes.Ret);


            var ctorDelegate = (DynamicMethodCtor)dmethod.CreateDelegate(typeof(DynamicMethodCtor));
            return (scope, contract, parentContract) => ctorDelegate(scope, contract, paramsCache, contractCache, createInstancesCache);
        }

        private void LoadArg(ILGenerator il, int position)
        {
            switch (position)
            {
                case 1: il.Emit(OpCodes.Ldarg_0); break;
                case 2: il.Emit(OpCodes.Ldarg_1); break;
                case 3: il.Emit(OpCodes.Ldarg_2); break;
                case 4: il.Emit(OpCodes.Ldarg_3); break;
                default: il.Emit(OpCodes.Ldarg_S, position -1 ); break;
            }
        }

        private void LoadIndex(ILGenerator il, int position)
        {
            switch (position) //load index
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
                default: il.Emit(OpCodes.Ldc_I4, position); break;
            }
        }

        

    }
}
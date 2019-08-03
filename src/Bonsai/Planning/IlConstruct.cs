namespace Bonsai.Planning
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using Contracts;
    using Exceptions;
    using RegistrationProcessing;
    using Registry;

    public class IlConstruct : IConstruct
    {
        //note this is what we have as a delegate.
        //   Test5(
        //       class [Bonsai]Bonsai.IAdvancedScope scope,
        //       class [Bonsai]Bonsai.Contracts.Contract contract,
        //       class [System.Collections]System.Collections.Generic.List`1<object> paramsCache,
        //       class [System.Collections]System.Collections.Generic.List`1<class [Bonsai]Bonsai.Contracts.Contract> contractCache,
        //       class [System.Collections]System.Collections.Generic.List`1<class Bonsai.Benchmarks.CreateInstance> delegateCache
        //     ) cil managed
        //   {
        //     .maxstack 8

        //     // [207 13 - 207 34]
        //     IL_0000: ldarg.1      // scope
        //     IL_0001: call         void [System.Console]System.Console::Write(object)

        //     // [208 13 - 208 37]
        //     IL_0006: ldarg.2      // contract
        //     IL_0007: call         void [System.Console]System.Console::Write(object)

        //     // [209 13 - 209 40]
        //     IL_000c: ldarg.3      // paramsCache
        //     IL_000d: call         void [System.Console]System.Console::Write(object)

        //     // [210 13 - 210 42]
        //     IL_0012: ldarg.s      contractCache
        //     IL_0014: call         void [System.Console]System.Console::Write(object)

        //     // [211 13 - 211 42]
        //     IL_0019: ldarg.s      delegateCache
        //     IL_001b: call         void [System.Console]System.Console::Write(object)

        //     // [213 13 - 213 25]
        //     IL_0020: ldnull
        //     IL_0021: ret

        //   } // end of method Builder::Test5



        public bool CanSupport(RegistrationContext context)
        {
            return true;
        }

        public CreateInstance Create(RegistrationContext context, IEnumerable<Contract> contracts)
        {
            contracts = contracts.ToList();
            var ctor = context.InjectOnMethods.First(x => x.InjectOn == InjectOn.Constructor);
            var methodName = $"{context.ImplementedType.FullName.Replace(".", "-")}-ctor";

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
                context.ImplementedType, 
                new[] { typeof(IAdvancedScope), typeof(Contract), typeof(Contract) }, 
                false);

            
            var il = dmethod.GetILGenerator();

            foreach (var param in ctor.Parameters)
            {
                var p = param;

                if (p.Value != null)
                {
                    // IL_0000: ldarg.3      // paramsCache
                    // IL_0001: ldc.i4.4
                    // IL_0002: callvirt     instance !0/*object*/ class [System.Collections]System.Collections.Generic.List`1<object>::get_Item(int32)
                    // IL_0007: castclass    class Bonsai.Benchmarks.Models.Repository`1<class Bonsai.Benchmarks.Models.User>

                    //get value from paramsCache (which we just added)
                    LoadArg(il, 3);

                    LoadIndex(il, paramsCache.Count);
                    il.Emit(OpCodes.Callvirt, getParamsIndexer);
                    il.Emit(p.ProvidedType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, p.ProvidedType);

                    paramsCache.Add(p.Value);
                    continue;
                }

                if (p.CreateInstance != null)
                {
                    // IL_000c: ldarg.s      createInstancesCache
                    // IL_000e: ldc.i4       3455 // 0x00000d7f
                    // IL_0013: callvirt     instance !0/*class Bonsai.Benchmarks.CreateInstance*/ class [System.Collections]System.Collections.Generic.List`1<class Bonsai.Benchmarks.CreateInstance>::get_Item(int32)
                    // IL_0018: ldarg.1      // scope
                    // IL_0019: ldnull
                    // IL_001a: ldarg.2      // contract
                    // IL_001b: callvirt     instance object Bonsai.Benchmarks.CreateInstance::Invoke(class [Bonsai]Bonsai.IAdvancedScope, class [Bonsai]Bonsai.Contracts.Contract, class [Bonsai]Bonsai.Contracts.Contract)
                    // IL_0020: castclass    Bonsai.Benchmarks.Models.Logger

                    LoadArg(il, 5);

                    LoadIndex(il, createInstancesCache.Count);
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

                // IL_0000: ldarg.1      // scope
                // IL_0001: ldarg.s      contractCache
                // IL_0003: ldc.i4       487 // 0x000001e7
                // IL_0008: callvirt     instance !0/*class [Bonsai]Bonsai.Contracts.Contract*/ class [System.Collections]System.Collections.Generic.List`1<class [Bonsai]Bonsai.Contracts.Contract>::get_Item(int32)
                // IL_000d: ldarg.2      // contract
                // IL_000e: callvirt     instance object [Bonsai]Bonsai.IAdvancedScope::Resolve(class [Bonsai]Bonsai.Contracts.Contract, class [Bonsai]Bonsai.Contracts.Contract)
                // IL_0013: castclass    class Bonsai.Benchmarks.Models.Repository`1<class Bonsai.Benchmarks.Models.User>

                LoadArg(il, 1); //scope

                LoadArg(il, 4); // contractCache
                LoadIndex(il, contractCache.Count);
                il.Emit(OpCodes.Callvirt, getContractsIndexer);

                LoadArg(il, 2);
                il.Emit(OpCodes.Callvirt, resolve);
                il.Emit(p.ServiceKey.Service.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, p.ServiceKey.Service);
            }


            il.Emit(OpCodes.Newobj, (ConstructorInfo)ctor.Method); //call ctor (passing all the parameters on the stack)
            il.Emit(OpCodes.Ret);


            var ctorDelegate = dmethod.CreateDelegate(typeof(DynamicMethodCtor));
            return (scope, contract, parentContract) => ((DynamicMethodCtor)ctorDelegate)(scope, contract, paramsCache, contractCache, createInstancesCache);
        }

        private void LoadArg(ILGenerator il, int position)
        {
            switch (position)
            {
                case 1: il.Emit(OpCodes.Ldarg_1); break;
                case 2: il.Emit(OpCodes.Ldarg_2); break;
                case 3: il.Emit(OpCodes.Ldarg_3); break;
                default: il.Emit(OpCodes.Ldarg_S, position); break;
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

        private delegate object DynamicMethodCtor(IAdvancedScope scope, Contract contract, List<object> paramsCache, List<Contract> contractCache, List<CreateInstance> createInstancesCache);

    }
}
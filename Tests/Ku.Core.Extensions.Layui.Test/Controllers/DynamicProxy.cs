using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Ku.Core.Extensions.Layui.Test.Controllers
{
    public class DynamicProxy
    {
        private static readonly string AssemblyName = "DynamicProxy",
                               ModuleName = "DynamicProxy",
                               TypeName = "DynamicProxy";

        private AssemblyBuilder CreateDynamicAssembly<T>() where T : class
        {
            return AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()),
                AssemblyBuilderAccess.Run);
            //return AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(AssemblyName + typeof(T).Name),
            //    AssemblyBuilderAccess.Run);
        }

        private ModuleBuilder CreateDynamicModule<T>() where T : class
        {
            return CreateDynamicAssembly<T>().DefineDynamicModule(ModuleName + typeof(T).Name);
        }

        /// <summary>
        /// 创建动态代理
        /// </summary>
        public T CreateDynamicType<T>() where T : class
        {
            TypeBuilder typeBuilder = CreateDynamicModule<T>().DefineType(TypeName + typeof(T).Name, TypeAttributes.Public);
            typeBuilder.AddInterfaceImplementation(typeof(T));
            TypeActuator<T>(typeBuilder);
            return Activator.CreateInstance(typeBuilder.CreateType()) as T;
        }

        private void TypeActuator<T>(TypeBuilder builder) where T : class
        {
            //FieldBuilder fieldBuilder = builder.DefineField("_DynamicProxyActuator", typeof(T), FieldAttributes.Private);
            //BuildCtorMethod(typeof(T), fieldBuilder, builder);
            MethodInfo[] info = GetMethodInfo(typeof(T));
            foreach (MethodInfo methodInfo in info)
            {
                //if (!methodInfo.IsVirtual && !methodInfo.IsAbstract) continue;
                //if (methodInfo.Name == "ToString") continue;
                //if (methodInfo.Name == "GetHashCode") continue;
                //if (methodInfo.Name == "Equals") continue;
                var ParameterTypes = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
                MethodBuilder methodBuilder = CreateMethod(builder, methodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual,
                    CallingConventions.Standard, methodInfo.ReturnType, ParameterTypes);
                var ilMethod = methodBuilder.GetILGenerator();
                BuildMethod(ilMethod, methodInfo, ParameterTypes);
            }
        }

        //private void BuildCtorMethod(Type classType, FieldBuilder fieldBuilder, TypeBuilder typeBuilder)
        //{
        //    var structureBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, null);
        //    var ilCtor = structureBuilder.GetILGenerator();
        //    ilCtor.Emit(OpCodes.Ldarg_0);
        //    ilCtor.Emit(OpCodes.Newobj, classType.GetConstructor(Type.EmptyTypes));
        //    ilCtor.Emit(OpCodes.Stfld, fieldBuilder);
        //    ilCtor.Emit(OpCodes.Ret);
        //}

        private void BuildMethod(ILGenerator il, MethodInfo methodInfo, Type[] ParameterTypes)
        {
            var type = typeof(Uaa).GetMethod("Action");

            LocalBuilder args = il.DeclareLocal(typeof(object[]));
            il.Emit(OpCodes.Ldc_I4_S, ParameterTypes.Length);
            il.Emit(OpCodes.Newarr, typeof(object));
            il.Emit(OpCodes.Stloc, args);
            if (ParameterTypes.Length > 0)
            {
                il.Emit(OpCodes.Ldloc, args);
                for (int index = 0; index < ParameterTypes.Length; index++)
                {
                    il.Emit(OpCodes.Ldc_I4_S, index);
                    il.Emit(OpCodes.Ldarg, index + 1);

                    Type parameterType = ParameterTypes[index];
                    if (parameterType.IsValueType || parameterType.IsGenericParameter)
                        il.Emit(OpCodes.Box, parameterType);

                    il.Emit(OpCodes.Stelem_Ref);
                    il.Emit(OpCodes.Ldloc, args);
                }
                il.Emit(OpCodes.Stloc, args);
            }

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldstr, methodInfo.Name);
            //for (int i = 0; i < ParameterTypes.Length; i++)
            //    il.Emit(OpCodes.Ldarg_S, (short)(i + 1));

            il.Emit(OpCodes.Ldloc_0);

            il.EmitCall(OpCodes.Call, type, null);
            il.Emit(OpCodes.Ret);
        }

        private MethodBuilder CreateMethod(TypeBuilder typeBuilder, string name, MethodAttributes attrs, CallingConventions callingConventions,
            Type type, Type[] parameterTypes)
        {
            return typeBuilder.DefineMethod(name, attrs, callingConventions, type, parameterTypes);
        }

        private MethodInfo[] GetMethodInfo(Type type)
        {
            //return type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            return type.GetMethods();
        }
    }
}

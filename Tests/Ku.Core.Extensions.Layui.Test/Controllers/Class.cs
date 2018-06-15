using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Ku.Core.Extensions.Layui.Test.Controllers
{
    public class Uaa
    {
        public static object Action(object obj, string methodName, object[] p1)
        {
            var interfaceType = obj.GetType().GetInterfaces()[0];
            MethodInfo methodInfo = interfaceType.GetMethods().SingleOrDefault(x=>x.Name.Equals(methodName) && x.GetParameters().Length == p1.Length);
            return "";
        }

        public static T GetT<T>() where T : class
        {
            AssemblyBuilder dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()),
                AssemblyBuilderAccess.Run);
            //动态创建模块
            ModuleBuilder mb = dynamicAssembly.DefineDynamicModule("AAAAAAA");
            TypeBuilder tb = mb.DefineType("MyClass", TypeAttributes.Public);
            tb.AddInterfaceImplementation(typeof(T));

            Type interfaceType = typeof(T);
            var methods = interfaceType.GetMethods();
            foreach (var method in methods)
            {
                var aa = method.GetParameters();
                MethodBuilder myHelloMethod = tb.DefineMethod(method.Name,
                        MethodAttributes.Public | MethodAttributes.Virtual,
                        method.ReturnType, aa.Select(x=>x.ParameterType).ToArray());

                ILGenerator myMethodIL = myHelloMethod.GetILGenerator();
                for (byte x = 0; x < aa.Length; x++)
                {
                    myMethodIL.Emit(OpCodes.Ldarg_S, x);
                }
                //myMethodIL.Emit(OpCodes.Ldstr, "Hi, ");
                //myMethodIL.Emit(OpCodes.Ldarg_1);
                //myMethodIL.Emit(OpCodes.Ldstr, "!");
                myMethodIL.Emit(OpCodes.Call, typeof(string).GetMethod("Concat", aa.Select(x => x.ParameterType).ToArray()));
                myMethodIL.Emit(OpCodes.Ret);

            }

            Type myType = tb.CreateType();

            T inst = (T)Activator.CreateInstance(myType);

            return inst;
        }
    }
}

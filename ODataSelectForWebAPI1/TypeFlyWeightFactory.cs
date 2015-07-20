using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ODataSelectForWebAPI1
{
    using System.Reflection;
    using System.Reflection.Emit;

    class FlyWeightTypeFactory
    {
        private static Dictionary<string, Type> _builtTypes = new Dictionary<string, Type>();
        private static Object _lockObject = new Object();

        private static AssemblyName _assemblyName = new AssemblyName("DynamicTypeAssembly");
        private static AssemblyBuilder _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
            _assemblyName, AssemblyBuilderAccess.Run);
        private static ModuleBuilder _moduleBuilder = _assemblyBuilder.DefineDynamicModule("DynamicTypeModule");

        public static Type New(Dictionary<string, Type> fields)
        {
            lock (_lockObject)
            {
                var className = String.Format(
                    "{{{0}}}",
                    String.Join(
                        ";",
                        fields.Select(
                            field =>
                            String.Format(
                                "{0}:{1}",
                                field.Key,
                                field.Value.GetGenericArguments().Count() > 0
                                    ? String.Format(
                                        "{0}<{1}>",
                                        field.Value.Name,
                                        String.Join("|", field.Value.GetGenericArguments().Select(s => s.Name)))
                                    : field.Value.Name))));

                if (!_builtTypes.ContainsKey(className))
                {
                    var typeBuilder = _moduleBuilder.DefineType(className, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable);

                    foreach (var field in fields)
                    {
                        typeBuilder.DefineField(field.Key, field.Value, FieldAttributes.Public);
                    }

                    _builtTypes.Add(className, typeBuilder.CreateType());
                }

                return _builtTypes[className];
            }
        }

        public static Type NewCollection(Dictionary<string, Type> fields)
        {
            lock (_lockObject)
            {
                var className = String.Format(
                    "({{{0}}})",
                    String.Join(";", fields.Select(field => String.Format("{0}:{1}", field.Key, field.Value.Name))));

                if (!_builtTypes.ContainsKey(className))
                {
                    var collectionType = New(fields);
                    var collection = typeof(List<>).MakeGenericType(collectionType);
                    _builtTypes.Add(className, collection);
                }

                return _builtTypes[className];
            }
        }
    }
}

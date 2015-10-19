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

        public static KeyValuePair<string, Type> New(Dictionary<string, Type> fields)
        {
            lock (_lockObject)
            {
                var classKeyName = String.Format(
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

                if (!_builtTypes.ContainsKey(classKeyName))
                {
                    var className = String.Format("DynamicType_{0}", _builtTypes.Count);

                    var typeBuilder = _moduleBuilder.DefineType(className, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable);

                    foreach (var field in fields)
                    {
                        typeBuilder.DefineField(field.Key, field.Value, FieldAttributes.Public);
                    }

                    _builtTypes.Add(classKeyName, typeBuilder.CreateType());
                }

                return new KeyValuePair<string, Type>(classKeyName, _builtTypes[classKeyName]);
            }
        }

        public static KeyValuePair<string, Type> NewCollection(Dictionary<string, Type> fields)
        {
            lock (_lockObject)
            {
                var classKeyName = String.Format(
                    "({{{0}}})",
                    String.Join(";", fields.Select(field => String.Format("{0}:{1}", field.Key, field.Value.Name))));

                if (!_builtTypes.ContainsKey(classKeyName))
                {
                    var collectionType = New(fields);
                    var collection = typeof(IEnumerable<>).MakeGenericType(collectionType.Value);

                    _builtTypes.Add(classKeyName, collection);
                }

                return new KeyValuePair<string, Type>(classKeyName, _builtTypes[classKeyName]);
            }
        }
    }
}

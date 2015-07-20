using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ODataSelectForWebAPI1
{
    using System.Collections;
    using System.Linq.Expressions;
    using System.Reflection;

    public class DynamicSelection
    {
        private static int _id;
        private static Object _lock = new Object();

        public static IQueryable Select(IQueryable source, Type type)
        {
            lock (_lock)
            {
                _id = 0;

                return source.Provider.CreateQuery(BindQueryable(source, type));
            }
        }

        private static Expression BindProperties(Expression sourceItem, Type type)
        {
            var bindings = new List<MemberBinding>();

            BindSystemTypeProperties(type, sourceItem, bindings);
            BindNonSystemTypeProperties(type, sourceItem, bindings);

            return Expression.MemberInit(Expression.New(type.GetConstructor(Type.EmptyTypes)), bindings);
        }

        private static Expression BindQueryable(IQueryable source, Type type)
        {
            ParameterExpression sourceItem = Expression.Parameter(source.ElementType, String.Format("t{0}", _id));
            _id++;

            Expression selector = Expression.Lambda(BindProperties(sourceItem, type), sourceItem);

            return Expression.Call(typeof(Queryable), "Select", new Type[] { source.ElementType, type },
                             Expression.Constant(source), selector);
        }

        private static void BindNonSystemTypeProperties(Type type, Expression sourceItem, List<MemberBinding> bindings)
        {
            foreach (var property in type.GetFields().Where(p => p.FieldType.Namespace != "System"))
            {
                if (property.FieldType.Namespace != null && property.FieldType.Namespace.StartsWith("System.Collection"))
                {
                    var selectExpression = BindEnumerable(sourceItem, property);

                    var member = Expression.New(
                        property.FieldType.GetConstructor(new[] { property.FieldType }),
                        new [] { selectExpression });

                    var innerMemberBind = Expression.Bind(property, member);
                    bindings.Add(innerMemberBind);
                }
                else
                {
                    var memberInit = BindProperties(Expression.Property(sourceItem, property.Name), property.FieldType);
                    var innerMemberBind = Expression.Bind(property, memberInit);
                    bindings.Add(innerMemberBind);
                }
            }
        }

        private static MethodCallExpression BindEnumerable(Expression sourceItem, FieldInfo property)
        {
            var sourceCollection = Expression.Property(sourceItem, property.Name);
            var sourceElementType = sourceCollection.Type.GetGenericArguments().ElementAt(0);
            var destinationElementType = property.FieldType.GetGenericArguments().ElementAt(0);

            var source = Expression.Parameter(sourceElementType, String.Format("t{0}", _id));
            _id++;

            var destinationProperties = BindProperties(source, destinationElementType);
            var selector = Expression.Lambda(destinationProperties, source);

            var selectExpression = Expression.Call(
                typeof(Enumerable),
                "Select",
                new Type[] { sourceElementType, destinationElementType },
                sourceCollection,
                selector);
            return selectExpression;
        }

        private static void BindSystemTypeProperties(Type type, Expression sourceItem, List<MemberBinding> bindings)
        {
            foreach (var property in type.GetFields().Where(p => p.FieldType.Namespace == "System"))
            {
                var propertyName = property.Name;
                var member = Expression.Bind(property, Expression.Property(sourceItem, propertyName));
                bindings.Add(member);
            }
        }
    }
}

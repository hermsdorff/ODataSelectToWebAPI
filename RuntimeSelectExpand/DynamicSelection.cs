using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuntimeSelectExpand
{
    using System.Linq.Expressions;

    public class DynamicSelection
    {
        public static IQueryable Select(IQueryable source, Type type)
        {
            ParameterExpression sourceItem = Expression.Parameter(source.ElementType, "t");

            var bindings = new List<MemberBinding>();

            BindSystemTypeProperties(type, sourceItem, bindings);

            foreach (var property in type.GetFields().Where(p => p.FieldType.Namespace != "System"))
            {
                var innerBindings = new List<MemberBinding>();
                BindSystemTypeProperties(property.FieldType, Expression.Property(sourceItem, property.Name), innerBindings);
                
                var innerMemberBind = Expression.Bind(
                    property,
                    Expression.MemberInit(
                        Expression.New(property.FieldType), innerBindings));

                bindings.Add(innerMemberBind);
            }

            Expression selector = Expression.Lambda(
                Expression.MemberInit(
                    Expression.New(type.GetConstructor(Type.EmptyTypes)), bindings), sourceItem);

            return source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "Select", new Type[] { source.ElementType, type },
                         Expression.Constant(source), selector));

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

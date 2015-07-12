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

            Expression selector = Expression.Lambda(BindProperties(sourceItem, type), sourceItem);

            return source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "Select", new Type[] { source.ElementType, type },
                         Expression.Constant(source), selector));
        }

        private static Expression BindProperties(Expression sourceItem, Type type)
        {
            List<MemberBinding> bindings = new List<MemberBinding>();

            BindSystemTypeProperties(type, sourceItem, bindings);
            BindNonSystemTypeProperties(type, sourceItem, bindings);

            return Expression.MemberInit(Expression.New(type.GetConstructor(Type.EmptyTypes)), bindings);
        }

        private static void BindNonSystemTypeProperties(Type type, Expression sourceItem, List<MemberBinding> bindings)
        {
            foreach (var property in type.GetFields().Where(p => p.FieldType.Namespace != "System"))
            {
                var innerBindings = new List<MemberBinding>();
                var memberInit = BindProperties(Expression.Property(sourceItem, property.Name), property.FieldType);
                var innerMemberBind = Expression.Bind(property, memberInit);
                bindings.Add(innerMemberBind);
            }
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

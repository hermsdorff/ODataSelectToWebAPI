using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuntimeSelectExpand
{
    using System.Linq.Expressions;

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

        private static Expression BindProperties(IQueryable source, Expression sourceItem, Type type)
        {
            List<MemberBinding> bindings = new List<MemberBinding>();

            BindSystemTypeProperties(type, sourceItem, bindings);
            BindNonSystemTypeProperties(source, type, sourceItem, bindings);

            return Expression.MemberInit(Expression.New(type.GetConstructor(Type.EmptyTypes)), bindings);
        }

        private static Expression BindQueryable(IQueryable source, Type type)
        {
            ParameterExpression sourceItem = Expression.Parameter(source.ElementType, string.Format("t{0}", _id));
            Expression selector = Expression.Lambda(BindProperties(source, sourceItem, type), sourceItem);

            _id++;

            return Expression.Call(typeof(Queryable), "Select", new Type[] { source.ElementType, type },
                             Expression.Constant(source), selector);
        }

        private static void BindNonSystemTypeProperties(IQueryable source, Type type, Expression sourceItem, List<MemberBinding> bindings)
        {
            foreach (var property in type.GetFields().Where(p => p.FieldType.Namespace != "System"))
            {
                if (property.FieldType.Namespace != null && property.FieldType.Namespace.StartsWith("System.Collection"))
                {
                    var f = Expression.Property(sourceItem, property.Name);
                    BindQueryable(null, property.FieldType);
                }
                else
                {
                    var innerBindings = new List<MemberBinding>();
                    var memberInit = BindProperties(source, Expression.Property(sourceItem, property.Name), property.FieldType);
                    var innerMemberBind = Expression.Bind(property, memberInit);
                    bindings.Add(innerMemberBind);
                }
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

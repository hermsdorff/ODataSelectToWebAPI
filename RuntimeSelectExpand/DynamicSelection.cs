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
            IEnumerable<MemberBinding> bindings = type.GetFields().Select(p => Expression.Bind(p, Expression.Property(sourceItem, sourceProperties[p.Name])))
        }
    }
}

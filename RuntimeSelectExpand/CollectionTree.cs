namespace RuntimeSelectExpand
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CollectionTree : ExpressionTree
    {
        public CollectionTree(string property): base(property)
        {
            
        }

        public override void BuildType()
        {
            var fields = new Dictionary<string, Type>();

            foreach (var item in Items.Where(i=>i.ElementType != typeof(void)))
            {
                item.BuildType();
                fields.Add(item.Name, item.QueryType);
            }

            QueryType = FlyWeightTypeFactory.NewCollection(fields);
        }

        public override void Bind(Type type)
        {
            if (!type.Namespace.StartsWith("System.Collections"))
            {
                ElementType = typeof(void);
                return;
            }
            var genericTypes = type.GetGenericArguments();

            if (Items.Count() == 0)
            {
                if (genericTypes.Count() == 1)
                {
                    if (genericTypes[0].Namespace == "System")
                    {
                        var item = new SystemTreeLeaf();
                        _items.Add(item);
                        item.Bind(genericTypes[0]);
                    }
                    else
                    {
                        AddChildElements(genericTypes[0]);
                    }
                }
            }
            else
            {
                if (genericTypes.Count() == 1)
                {
                    BindChildElements(genericTypes[0]);
                }
            }
        }
    }
}
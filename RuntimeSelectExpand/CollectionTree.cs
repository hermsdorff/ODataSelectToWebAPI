namespace RuntimeSelectExpand
{
    using System;
    using System.Linq;

    public class CollectionTree : ExpressionTree
    {
        public CollectionTree(string property): base(property)
        {
            
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
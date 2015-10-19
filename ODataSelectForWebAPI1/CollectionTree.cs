namespace ODataSelectForWebAPI1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CollectionTree : ExpressionTree
    {
        private bool _isCollection;

        public CollectionTree(string property): base(property)
        {
            
        }

        public override void BuildType()
        {
            if(!_isCollection)
            {
                base.BuildType();
                return;
            }
            
            var fields = new Dictionary<string, Type>();

            foreach (var item in Items.Where(i=>i.ElementType != typeof(void)))
            {
                item.BuildType();
                fields.Add(item.Name, item.QueryType.Value);
            }

            QueryType = FlyWeightTypeFactory.NewCollection(fields);
        }

        public override void Bind(Type type, int depth)
        {
            if (!type.Namespace.StartsWith("System.Collections"))
            {
                if (Items.Count() > 0)
                {
                    base.Bind(type, depth);
                }
                else
                {
                    ElementType = typeof(void);
                }
                return;
            }
            _isCollection = true;
            var genericTypes = type.GetGenericArguments();

            if (Items.Count() == 0)
            {
                if (genericTypes.Count() == 1)
                {
                    if (genericTypes[0].Namespace == "System")
                    {
                        var item = new SystemTreeLeaf();
                        _items.Add(item);
                        item.Bind(genericTypes[0], depth);
                    }
                    else
                    {
                        AddChildElements(genericTypes[0], depth);
                    }
                }
            }
            else
            {
                if (genericTypes.Count() == 1)
                {
                    BindChildElements(genericTypes[0], depth);
                }
            }
        }
    }
}
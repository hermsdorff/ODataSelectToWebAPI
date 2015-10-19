namespace ODataSelectForWebAPI1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    public class ExpressionTree : IExpressionTree
    {
        protected List<IExpressionTree> _items = new List<IExpressionTree>();

        #region Implementation of IExpressionTree

        public string Name { get; private set; }

        public IEnumerable<IExpressionTree> Items
        {
            get
            {
                return _items;
            }
        }

        public Type ElementType { get; protected set; }

        public void AddProperty(string property)
        {
            if (property.IndexOf('/') == -1)
            {
                _items.Add(new ExpressionTree(property));
            }
            else
            {
                var twoParts = property.Split("/".ToCharArray(), 2);
                var lowLevel = twoParts[0];
                var secondPart = twoParts[1];

                if (!_items.Any(i => i.Name.Equals(lowLevel)))
                {
                    _items.Add(new ExpressionTree(lowLevel));
                }

                var propertyNode = _items.Single(i => i.Name.Equals(lowLevel));
                propertyNode.AddProperty(secondPart);
            }
        }

        public void AddCollection(string property)
        {
            if (property.IndexOf('/') == -1)
            {
                _items.Add(new CollectionTree(property));
            }
            else
            {
                var twoParts = property.Split("/".ToCharArray(), 2);
                var lowLevel = twoParts[0];
                var secondPart = twoParts[1];

                if (!_items.Any(i => i.Name.Equals(lowLevel)))
                {
                    _items.Add(new CollectionTree(lowLevel));
                }

                var propertyNode = _items.Single(i => i.Name.Equals(lowLevel));
                propertyNode.AddCollection(secondPart);
            }
        }

        public virtual void Bind(Type type, int depth = 1)
        {
            if (Items.Count() == 0)
            {
                if (type.Namespace == "System")
                {
                    ElementType = type;
                }
                else if (type.IsEnum)
                {
                    ElementType = type;
                }
                else
                {
                    if(depth < 0)
                    {
                        Ignore = true;
                        return;
                    }
                    this.AddChildElements(type, depth - 1);
                }
            }
            else
            {
                if (depth < 0) return;
                this.BindChildElements(type, depth);
            }
        }

        protected bool Ignore { get; set; }

        public virtual void BuildType()
        {
            if (Items.Count() == 0)
            {
                QueryType = new KeyValuePair<string, Type>(ElementType.Name, ElementType);
            }
            else
            {
                var fields = new Dictionary<string, Type>();

                foreach (var item in Items.Where(i=>i.ElementType != typeof(void)))
                {
                    item.BuildType();
                    fields.Add(item.Name, item.QueryType.Value);
                }

                QueryType = FlyWeightTypeFactory.New(fields);
            }
        }

        public KeyValuePair<string, Type> QueryType { get; protected set; }

        protected void BindChildElements(Type type, int depth)
        {
            foreach (var item in this.Items)
            {
                var property = type.GetProperty(item.Name);
                item.Bind(property != null ? type.GetProperty(item.Name).PropertyType : typeof(object), depth);
            }
        }

        protected void AddChildElements(Type type, int depth)
        {
            foreach (var propertyInfo in type.GetProperties())
            {
                if (!propertyInfo.PropertyType.Namespace.StartsWith("System.Collection"))
                {
                    var item = new ExpressionTree(propertyInfo.Name);
                    item.Bind(propertyInfo.PropertyType, depth);
                    if (!item.Ignore) this._items.Add(item);
                }
            }
        }

        #endregion

        public ExpressionTree(string name)
        {
            Name = name;
        }

        public ExpressionTree()
            : this(String.Empty)
        {

        }
    }
}
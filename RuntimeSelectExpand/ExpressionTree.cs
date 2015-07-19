namespace RuntimeSelectExpand
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

        public virtual void Bind(Type type)
        {
            if (Items.Count() == 0)
            {
                if (type.Namespace == "System")
                {
                    ElementType = type;
                }
                else
                {
                    this.AddChildElements(type);
                }
            }
            else
            {
                this.BindChildElements(type);
            }
        }

        public virtual void BuildType()
        {
            if (Items.Count() == 0)
            {
                QueryType = ElementType;
            }
            else
            {
                var fields = new Dictionary<string, Type>();

                foreach (var item in Items.Where(i=>i.ElementType != typeof(void)))
                {
                    item.BuildType();
                    fields.Add(item.Name, item.QueryType);
                }

                QueryType = FlyWeightTypeFactory.New(fields);
            }
        }

        public Type QueryType { get; protected set; }

        protected void BindChildElements(Type type)
        {
            foreach (var item in this.Items)
            {
                var property = type.GetProperty(item.Name);
                item.Bind(property != null ? type.GetProperty(item.Name).PropertyType : typeof(object));
            }
        }

        protected void AddChildElements(Type type)
        {
            foreach (var propertyInfo in type.GetProperties())
            {
                if (!propertyInfo.PropertyType.Namespace.StartsWith("System.Collection"))
                {
                    var item = new ExpressionTree(propertyInfo.Name);
                    this._items.Add(item);
                    item.Bind(propertyInfo.PropertyType);
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
namespace ODataSelectForWebAPI1
{
    using System;
    using System.Collections.Generic;

    public class SystemTreeLeaf : IExpressionTree
    {
        #region Implementation of IExpressionTree

        public string Name { get; private set; }

        public IEnumerable<IExpressionTree> Items
        {
            get
            {
                return new List<IExpressionTree>();
            }
        }

        public Type ElementType { get; private set; }

        public void AddProperty(string property)
        {
            throw new NotImplementedException();
        }

        public void AddCollection(string property)
        {
            throw new NotImplementedException();
        }

        public void Bind(Type type, int depth = 1)
        {
            ElementType = type;
        }

        public void BuildType()
        {
            QueryType = ElementType;
        }

        public Type QueryType { get; protected set; }

        #endregion
    }
}
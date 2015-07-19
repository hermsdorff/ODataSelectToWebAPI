﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuntimeSelectExpand
{
    public interface IExpressionTree
    {
        string Name { get; }
        IEnumerable<IExpressionTree> Items { get; }
        Type ElementType { get; }
        void AddProperty(string property);
        void AddCollection(string property);
        void Bind(Type type);
        void BuildType();
        Type QueryType { get; }
    }
}
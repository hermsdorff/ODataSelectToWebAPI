﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Testes.Fakes
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<Product> Products { get; set; }
        public List<string> Tags { get; set; } 
    }
}
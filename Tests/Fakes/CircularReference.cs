using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests.Fakes
{
    public class CircularReference
    {
        public string Name { get; set; }
        public CircularReference Parent { get; set; }
    }
}

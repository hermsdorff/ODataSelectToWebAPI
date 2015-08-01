using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests.Fakes
{
    public class Company
    {
        public int Id { get; set; }
        public Person Owner { get; set; }
        public string Name { get; set; }
    }
}

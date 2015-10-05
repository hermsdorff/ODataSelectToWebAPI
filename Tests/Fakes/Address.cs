using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests.Fakes
{
    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public Person Person { get; set; }
    }
}

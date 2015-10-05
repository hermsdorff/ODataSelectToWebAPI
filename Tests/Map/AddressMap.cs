using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests.Map
{
    using System.Data.Entity.ModelConfiguration;

    using Tests.Fakes;

    public  class AddressMap : EntityTypeConfiguration<Address>
    {
        public AddressMap()
        {
            HasKey(a => a.Id);
            Property(a => a.Street);
            Property(a => a.City);
        }
    }
}

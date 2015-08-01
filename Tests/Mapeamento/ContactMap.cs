using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using Tests.Fakes;

namespace Tests.Mapeamento
{
    class ContactMap: EntityTypeConfiguration<Contact>
    {
        public ContactMap()
        {
            HasKey(c => c.Id);
            Property(c => c.Email);
            Property(c => c.PhoneNumber);
        }
    }
}

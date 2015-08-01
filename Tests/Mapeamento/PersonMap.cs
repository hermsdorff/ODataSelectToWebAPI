using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using Tests.Fakes;

namespace Tests.Mapeamento
{
    public class PersonMap : EntityTypeConfiguration<Person>
    {
        public PersonMap()
        {
            HasKey(p => p.Id);
            Property(p => p.Name);
            HasOptional(p => p.Contact);
        }
    }
}

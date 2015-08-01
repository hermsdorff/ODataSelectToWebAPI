using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using Tests.Fakes;

namespace Tests.Mapeamento
{
    public class CompanyMap : EntityTypeConfiguration<Company>
    {
        public CompanyMap()
        {
            HasKey(p => p.Id);
            Property(c => c.Name);
            HasRequired(c => c.Owner);
        }
    }
}

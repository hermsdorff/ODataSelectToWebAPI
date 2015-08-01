using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using Tests.Fakes;

namespace Tests.Mapeamento
{
    class CategoryMap : EntityTypeConfiguration<Category>
    {
        public CategoryMap()
        {
            HasKey(c => c.Id);

            Property(c => c.Name);
            Property(c => c.Status);
            
            HasMany(c => c.Products);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using Tests.Fakes;

namespace Tests.Mapeamento
{
    class ProductMap : EntityTypeConfiguration<Product>
    {
        public ProductMap()
        {
            HasKey(p => p.Id);

            Property(p => p.Name);

            HasOptional(p => p.Manufacturer);
            HasMany(p => p.Models);
        }
    }
}
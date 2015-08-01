using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using Tests.Fakes;

namespace Tests.Mapeamento
{
    class ModelMap : EntityTypeConfiguration<Model>
    {
        public ModelMap()
        {
            HasKey(m => m.Id);
            Property(m => m.Name);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using Tests.Fakes;

namespace Tests.Mapeamento
{
    public class TestDataContext : DbContext
    {
        public TestDataContext(DbConnection connection) : base(connection, true)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new CategoryMap());
            modelBuilder.Configurations.Add(new ProductMap());
            modelBuilder.Configurations.Add(new ManufacturerMap());
            modelBuilder.Configurations.Add(new ModelMap());
            modelBuilder.Configurations.Add(new CompanyMap());
            modelBuilder.Configurations.Add(new PersonMap());
            modelBuilder.Configurations.Add(new ContactMap());
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Contact> Contacts { get; set; }
    }
}

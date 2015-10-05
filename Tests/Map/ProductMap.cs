namespace Tests.Map
{
    using System.Data.Entity.ModelConfiguration;

    using Tests.Fakes;

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
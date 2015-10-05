namespace Tests.Map
{
    using System.Data.Entity.ModelConfiguration;

    using Tests.Fakes;

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

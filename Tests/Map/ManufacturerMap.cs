namespace Tests.Map
{
    using System.Data.Entity.ModelConfiguration;

    using Tests.Fakes;

    class ManufacturerMap:EntityTypeConfiguration<Manufacturer>
    {
        public ManufacturerMap()
        {
            HasKey(m => m.Id);
            Property(m => m.Name);
        }
    }
}

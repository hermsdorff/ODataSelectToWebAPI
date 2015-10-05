namespace Tests.Map
{
    using System.Data.Entity.ModelConfiguration;

    using Tests.Fakes;

    class ModelMap : EntityTypeConfiguration<Model>
    {
        public ModelMap()
        {
            HasKey(m => m.Id);
            Property(m => m.Name);
        }
    }
}

namespace Tests.Map
{
    using System.Data.Entity.ModelConfiguration;

    using Tests.Fakes;

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

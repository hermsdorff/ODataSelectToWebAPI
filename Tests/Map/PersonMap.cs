namespace Tests.Map
{
    using System.Data.Entity.ModelConfiguration;

    using Tests.Fakes;

    public class PersonMap : EntityTypeConfiguration<Person>
    {
        public PersonMap()
        {
            HasKey(p => p.Id);
            Property(p => p.Name);
            HasOptional(p => p.Contact);
            HasMany(p => p.Addresses)
                .WithRequired(a => a.Person);
        }
    }
}

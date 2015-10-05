namespace Tests.Map
{
    using System.Data.Entity.ModelConfiguration;

    using Tests.Fakes;

    class ContactMap: EntityTypeConfiguration<Contact>
    {
        public ContactMap()
        {
            HasKey(c => c.Id);
            Property(c => c.Email);
            Property(c => c.PhoneNumber);
        }
    }
}

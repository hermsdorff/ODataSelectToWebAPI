namespace Tests.Fakes
{
    using System.Runtime.Serialization;

    public class Manufacturer
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [IgnoreDataMember]
        public bool NotSerializableProperty { get; set; }

    }
}
namespace ExamManager.App.Entities
{
    // Positional record
    public record Address(
        string City,
        string Zip,
        string Street
        )
    {
        public string FullAddress => $"{Zip} {City}, {Street}";
    }
    //public class Address : IEquatable<Address?>
    //{
    //    public Address(string city, string zip, string street)
    //    {
    //        City = city;
    //        Zip = zip;
    //        Street = street;
    //    }

    //    public string City { get; }
    //    public string Zip { get; }
    //    public string Street { get;  }

    //    public override bool Equals(object? obj)
    //    {
    //        return Equals(obj as Address);
    //    }

    //    public bool Equals(Address? other)
    //    {
    //        return other != null &&
    //               City == other.City &&
    //               Zip == other.Zip &&
    //               Street == other.Street;
    //    }

    //    public override int GetHashCode()
    //    {
    //        return HashCode.Combine(City, Zip, Street);
    //    }
    //}
}

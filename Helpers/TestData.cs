namespace TestAssignment;

public static class TestData
{
    /// <summary>Thread-safe shared instance for parallel test runs.</summary>
    private static Random Rnd => Random.Shared;

    public static string Email()
    {
        string prefix = PickOne(new[] { "test", "auto", "qa", "reg", "play", "demo", "hr" });
        string unique = (Rnd.Next(1000000, 9999999) + DateTime.Now.Ticks % 100000).ToString();
        return $"{prefix}{unique}@testmail.hr";
    }

    public static string Password()
    {
        string basePass = "Test" + Rnd.Next(100000, 999999);
        string extra = PickOne(new[] { "!", "@", "#", "$", "%", "^", "&", "*" });
        return basePass + extra + PickOne(new[] { "Hr", "Zg", "St" });
    }

    public static string FirstName()
    {
        string[] patterns = { "Iv", "Mar", "Luk", "Nik", "Jos", "Ant", "Tom", "Pet", "Jur", "Stj", "Fil", "Dav", "Le", "No", "An", "Kat", "Luc", "Sar", "Nik", "Ema" };
        return patterns[Rnd.Next(patterns.Length)] + PickOne(new[] { "an", "ko", "a", "o", "ica", "eta", "in", "na", "ela", "ija" });
    }

    public static string LastName()
    {
        string[] roots = { "Hor", "Kov", "Nov", "Juri", "Mark", "Petrov", "Radi", "Babi", "Pavl", "Sim", "Lovr", "Perk", "Boz", "Knez", "Bar", "Gal", "Vid", "Tom", "Fran", "Dragi" };
        return roots[Rnd.Next(roots.Length)] + PickOne(new[] { "ić", "ović", "ević", "ić", "ak", "ec", "in", "ović", "ić" });
    }

    public static string City()
    {
        string[] parts = { "Gornja", "Donja", "Nova", "Stara", "Srednja", "Mala", "Velika", "Sveti", "Sveta", "" };
        string[] roots = { "Nedelja", "Petar", "Marko", "Luka", "Ivan", "Juraj", "Ante", "Martin", "Klara", "Jela", "Rok", "Križ", "Katarina" };
        string[] ends = { "ac", "evo", "ica", "polje", "grad", "selo", "brdo", "dol", "luka", "voda", "gora" };

        var p = PickOne(parts);
        var r = PickOne(roots);
        var e = PickOne(ends);

        return string.IsNullOrEmpty(p) ? r + e : $"{p} {r}{e}";
    }

    public static string Street()
    {
        int broj = Rnd.Next(1, 180);
        string[] ulice = { "Ulica", "Cesta", "Trg", "Avenija", "Put", "Ulica kralja", "Ulica kneza", "Obala" };
        string[] imena = { "Tomislava", "Zvonimira", "Krešimira", "Trpimira", "Branimira", "Petra", "Ante", "Jelačića", "Radića", "Starčevića", "Gupca", "Hebranga" };

        return $"{PickOne(ulice)} {PickOne(imena)} {broj}";
    }

    public static string Phone()
    {
        int operater = Rnd.Next(0, 10) < 7 ? Rnd.Next(91, 99) : Rnd.Next(95, 98);
        int dio1 = Rnd.Next(100, 999);
        int dio2 = Rnd.Next(100, 999);
        return $"+385 {operater} {dio1} {dio2}";
    }

    public static string Zip()
    {
        return Rnd.Next(10000, 53297).ToString("00000");
    }

    private static string PickOne(string[] arr) => arr[Rnd.Next(arr.Length)];

    public static RegistrationData Generate()
    {
        return new RegistrationData
        {
            FirstName = FirstName(),
            LastName = LastName(),
            Email = Email(),
            Password = Password(),
            Phone = Phone(),
            StreetAddress = Street(),
            City = City(),
            ZipCode = Zip()
        };
    }

    public static string GenerateRandomLocalPart(int length)
    {
        var localPart = Guid.NewGuid().ToString("N");

        while (localPart.Length < length)
            localPart += Guid.NewGuid().ToString("N");

        return $"{localPart[..length]}@test.com";
    }
}

public record RegistrationData
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string Phone { get; init; }
    public required string StreetAddress { get; init; }
    public required string City { get; init; }
    public required string ZipCode { get; init; }
}
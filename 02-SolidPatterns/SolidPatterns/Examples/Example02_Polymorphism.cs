namespace DotnetBackendLab.SolidPatterns.Examples.Polymorphism;

public sealed class Example02_Polymorphism : IExample
{
    public int Number => 2;
    public string Title => "Polymorphism: abstract Consignment, override (runtime) vs overload (compile-time)";

    public Task RunAsync()
    {
        // 1) Override: hangi metodun çalışacağına ÇALIŞMA ZAMANINDA nesnenin gerçek tipine bakılır.
        Consignment[] items = [new Parcel("TR100", 4), new Document("TR200")];
        Console.WriteLine("1) Override (runtime polymorphism):");
        foreach (var c in items)
        {
            Console.WriteLine($"   {c.Describe(),-28} -> fiyat {c.Price(),6:N2} TL");
        }
        Console.WriteLine();

        // 2) Overload: hangi metodun çağrılacağına DERLEME ZAMANINDA değişkenin tipine bakılır.
        Consignment asBase = new Parcel("TR300", 2);   // gerçek nesne Parcel, değişkenin tipi Consignment
        Parcel asParcel = new Parcel("TR301", 2);
        Console.WriteLine("2) Overload (compile-time):");
        Console.WriteLine($"   Label(asParcel)              -> {LabelPrinter.Label(asParcel)}");
        Console.WriteLine($"   Label(asBase)  [nesne Parcel] -> {LabelPrinter.Label(asBase)}  (değişken tipi seçti)");
        Console.WriteLine($"   asBase.Price()               -> {asBase.Price():N2} TL      (override: nesne tipi seçti)");

        return Task.CompletedTask;
    }
}

public abstract class Consignment(string trackingNo)
{
    public string TrackingNo { get; } = trackingNo;

    public abstract decimal Price();                                         // alt sınıf yazmak zorunda
    public virtual string Describe() => $"{GetType().Name} {TrackingNo}";    // alt sınıf isterse değiştirir
}

public sealed class Parcel(string trackingNo, decimal kg) : Consignment(trackingNo)
{
    public decimal Kg { get; } = kg;
    public override decimal Price() => 30m + Kg * 12.5m;
    public override string Describe() => $"Koli {TrackingNo} ({Kg} kg)";
}

public sealed class Document(string trackingNo) : Consignment(trackingNo)
{
    public override decimal Price() => 25m;   // Describe override edilmedi -> temel sınıftaki çalışır
}

public static class LabelPrinter
{
    public static string Label(Consignment c) => $"GENEL etiket: {c.TrackingNo}";
    public static string Label(Parcel p) => $"KOLİ etiketi: {p.TrackingNo}, {p.Kg} kg";
}

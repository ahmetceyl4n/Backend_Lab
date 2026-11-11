namespace DotnetBackendLab.CSharpFundamentals.Examples;

public sealed class Example06_InterfaceVsAbstract : IExample
{
    public int Number => 6;
    public string Title => "InterfaceVsAbstract: kargo fiyatı; sözleşme vs ortak kod, default interface method";

    public Task RunAsync()
    {
        const decimal kg = 3m;

        // Interface: sadece "ne yapılacağını" söyler. Birbirine benzemeyen tipler aynı sözleşmeyi uygular.
        IPriceCalculator[] calculators = [new StandardCarrier(), new ExpressCarrier(), new DronePartner()];

        Console.WriteLine($"{kg} kg kargo için fiyatlar:");
        foreach (var calc in calculators)
        {
            Console.WriteLine($"   {calc.Describe(kg)}");
        }
        Console.WriteLine();

        Console.WriteLine("Fark:");
        Console.WriteLine("   CarrierBase (abstract) : ortak kod + state (BaseFee, Calculate akışı) taşır; tek sınıftan miras alınır.");
        Console.WriteLine("   IPriceCalculator       : sadece sözleşme; bir sınıf birden fazla interface uygulayabilir.");
        Console.WriteLine("   Describe()             : default interface method. DronePartner yazmadı, interface'teki çalıştı;");
        Console.WriteLine("                            sadece interface üzerinden çağrılabilir (new DronePartner().Describe(...) derlenmez).");

        return Task.CompletedTask;
    }
}

public interface IPriceCalculator
{
    string Name { get; }
    decimal Calculate(decimal kg);

    // Default interface method (C# 8+): interface'e yeni metot eklerken eski uygulayıcılar kırılmaz.
    string Describe(decimal kg) => $"{Name,-16}: {Calculate(kg),7:N2} TL";
}

// Abstract class: ortak akışı sabitler, değişen kısmı (PerKg) alt sınıfa bırakır (template method).
public abstract class CarrierBase : IPriceCalculator
{
    protected abstract decimal BaseFee { get; }
    protected abstract decimal PerKg { get; }
    public abstract string Name { get; }

    public decimal Calculate(decimal kg) => BaseFee + kg * PerKg;
}

public sealed class StandardCarrier : CarrierBase
{
    protected override decimal BaseFee => 30m;
    protected override decimal PerKg => 10m;
    public override string Name => "Standart";
}

public sealed class ExpressCarrier : CarrierBase
{
    protected override decimal BaseFee => 60m;
    protected override decimal PerKg => 15m;
    public override string Name => "Ekspres";
}

// CarrierBase'den gelmiyor: tamamen farklı hesap, sadece sözleşmeyi uyguluyor.
public sealed class DronePartner : IPriceCalculator
{
    public string Name => "Drone (partner)";
    public decimal Calculate(decimal kg) => kg <= 5m ? 120m : throw new ArgumentOutOfRangeException(nameof(kg), "Drone 5 kg üstünü taşımaz");
}

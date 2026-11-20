namespace DotnetBackendLab.SolidPatterns.Examples.Composition;

public sealed class Example03_CompositionOverInheritance : IExample
{
    public int Number => 3;
    public string Title => "CompositionOverInheritance: şişen sınıf hiyerarşisi vs bileşenlerden kurulan kargo";

    public Task RunAsync()
    {
        // KÖTÜ: her özellik kombinasyonu için ayrı sınıf. 2 hız x 2 sigorta = 4 sınıf;
        // "soğuk zincir" gelirse 8, "kapıda ödeme" gelirse 16 sınıf olur.
        ShipmentBase[] hierarchy = [new StandardShipment(), new InsuredStandardShipment(), new ExpressShipment(), new InsuredExpressShipment()];
        Console.WriteLine("KÖTÜ (kalıtım hiyerarşisi):");
        foreach (var s in hierarchy)
            Console.WriteLine($"   {s.GetType().Name,-24}: {s.Total(5),7:N2} TL");
        Console.WriteLine("   Yeni bir özellik = mevcut sınıf sayısı x 2. Sigorta kuralı 2 sınıfta tekrar ediyor.");
        Console.WriteLine();

        // İYİ: Shipment "bir fiyatlandırmaya SAHİP" ve "bir sigortaya SAHİP" (has-a). Parçalar takılıp çıkarılır.
        IPriceCalculator[] speeds = [new StandardPrice(), new ExpressPrice()];
        IInsurance[] insurances = [new NoInsurance(), new ValueInsurance(declaredValue: 2_000m)];
        Console.WriteLine("İYİ (composition: 2 + 2 küçük sınıf, 4 kombinasyon):");
        foreach (var speed in speeds)
            foreach (var insurance in insurances)
            {
                var shipment = new Shipment(speed, insurance);
                Console.WriteLine($"   {shipment.Describe(),-36}: {shipment.Total(5),7:N2} TL");
            }
        Console.WriteLine("   Yeni özellik = yeni küçük bir sınıf; mevcutlara dokunulmaz, çalışırken de değiştirilebilir.");

        return Task.CompletedTask;
    }
}

// --- Kötü: kalıtımla şişen hiyerarşi ---
public abstract class ShipmentBase { public abstract decimal Total(decimal kg); }
public class StandardShipment : ShipmentBase { public override decimal Total(decimal kg) => 30m + kg * 10m; }
public class ExpressShipment : ShipmentBase { public override decimal Total(decimal kg) => 60m + kg * 15m; }
public sealed class InsuredStandardShipment : StandardShipment { public override decimal Total(decimal kg) => base.Total(kg) + 2_000m * 0.01m; }
public sealed class InsuredExpressShipment : ExpressShipment { public override decimal Total(decimal kg) => base.Total(kg) + 2_000m * 0.01m; }

// --- İyi: küçük bileşenler ---
public interface IPriceCalculator { string Name { get; } decimal Calculate(decimal kg); }
public interface IInsurance { string Name { get; } decimal Premium(); }

public sealed class StandardPrice : IPriceCalculator { public string Name => "Standart"; public decimal Calculate(decimal kg) => 30m + kg * 10m; }
public sealed class ExpressPrice : IPriceCalculator { public string Name => "Ekspres"; public decimal Calculate(decimal kg) => 60m + kg * 15m; }
public sealed class NoInsurance : IInsurance { public string Name => "sigortasız"; public decimal Premium() => 0m; }
public sealed class ValueInsurance(decimal declaredValue) : IInsurance
{
    public string Name => $"değer sigortalı ({declaredValue:N0} TL)";
    public decimal Premium() => declaredValue * 0.01m;
}

public sealed class Shipment(IPriceCalculator price, IInsurance insurance)
{
    public decimal Total(decimal kg) => price.Calculate(kg) + insurance.Premium();
    public string Describe() => $"{price.Name}, {insurance.Name}";
}

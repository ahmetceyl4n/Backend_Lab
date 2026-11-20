namespace DotnetBackendLab.SolidPatterns.Examples.OpenClosed;

public sealed class Example05_OpenClosed : IExample
{
    public int Number => 5;
    public string Title => "OpenClosed: switch ile fiyat vs IPriceCalculator ile genişletme";

    public Task RunAsync()
    {
        const decimal kg = 3m;

        // KÖTÜ: yeni kargo türü = bu metodu açıp değiştirmek (ve aynı switch'in kopyalandığı her yeri).
        Console.WriteLine("KÖTÜ (switch):");
        foreach (var type in new[] { "standart", "ekspres", "drone" })
        {
            try { Console.WriteLine($"   {type,-9}: {SwitchPricing.Price(type, kg),7:N2} TL"); }
            catch (ArgumentException ex) { Console.WriteLine($"   {type,-9}: HATA -> {ex.Message}"); }
        }
        Console.WriteLine();

        // İYİ: PricingService değişime KAPALI, yeni hesaplayıcılarla genişlemeye AÇIK.
        var service = new PricingService([new StandardCalculator(), new ExpressCalculator()]);
        Console.WriteLine("İYİ (IPriceCalculator):");
        Console.WriteLine($"   standart : {service.Price("standart", kg),7:N2} TL");

        // Drone eklemek için PricingService'e dokunmadan YENİ bir sınıf yazıp listeye eklemek yeterli.
        var extended = new PricingService([new StandardCalculator(), new ExpressCalculator(), new DroneCalculator()]);
        Console.WriteLine($"   drone    : {extended.Price("drone", kg),7:N2} TL  (PricingService'te tek satır değişmedi)");
        Console.WriteLine($"   Bilinen türler: {string.Join(", ", extended.KnownTypes)}");

        return Task.CompletedTask;
    }
}

public static class SwitchPricing
{
    public static decimal Price(string type, decimal kg) => type switch
    {
        "standart" => 30m + kg * 10m,
        "ekspres" => 60m + kg * 15m,
        _ => throw new ArgumentException($"Bilinmeyen tür '{type}'. Eklemek için bu switch değişmeli.", nameof(type)),
    };
}

public interface IPriceCalculator
{
    string Type { get; }
    decimal Calculate(decimal kg);
}

public sealed class StandardCalculator : IPriceCalculator { public string Type => "standart"; public decimal Calculate(decimal kg) => 30m + kg * 10m; }
public sealed class ExpressCalculator : IPriceCalculator { public string Type => "ekspres"; public decimal Calculate(decimal kg) => 60m + kg * 15m; }
public sealed class DroneCalculator : IPriceCalculator { public string Type => "drone"; public decimal Calculate(decimal kg) => 120m; }

/// <summary>Hangi hesaplayıcıların var olduğunu bilmez; kendisine verilenleri kullanır.</summary>
public sealed class PricingService(IEnumerable<IPriceCalculator> calculators)
{
    private readonly Dictionary<string, IPriceCalculator> _byType = calculators.ToDictionary(c => c.Type);

    public IEnumerable<string> KnownTypes => _byType.Keys;

    public decimal Price(string type, decimal kg) =>
        _byType.TryGetValue(type, out var calculator)
            ? calculator.Calculate(kg)
            : throw new ArgumentException($"Bilinmeyen tür '{type}'.", nameof(type));
}

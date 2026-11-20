namespace DotnetBackendLab.SolidPatterns.Examples.Strategy;

public sealed class Example11_Strategy : IExample
{
    public int Number => 11;
    public string Title => "Strategy: çalışma zamanında Dictionary ile seçilen fiyat stratejisi";

    public Task RunAsync()
    {
        // Stratejiler bir kez kaydedilir; hangisinin kullanılacağı ÇALIŞIRKEN gelen veriye göre seçilir.
        var strategies = new Dictionary<string, IPricingStrategy>
        {
            ["bireysel"] = new RegularPricing(),
            ["kampanya"] = new CampaignPricing(discountRate: 0.20m),
            ["kurumsal"] = new CorporatePricing(fixedDiscount: 15m),
        };

        const decimal basePrice = 100m;
        string[] incomingCustomerTypes = ["bireysel", "kurumsal", "kampanya", "bireysel"];   // ör. istekten gelen veri

        var checkout = new Checkout(strategies["bireysel"]);
        Console.WriteLine($"Taban fiyat {basePrice:N2} TL; müşteri tipine göre strateji çalışırken değişiyor:");
        foreach (var customerType in incomingCustomerTypes)
        {
            checkout.SetStrategy(strategies[customerType]);   // if/else yok: Dictionary'den seçim
            Console.WriteLine($"   {customerType,-9} -> {checkout.Strategy.Name,-22} {checkout.Total(basePrice),7:N2} TL");
        }
        Console.WriteLine();
        Console.WriteLine("Yeni strateji = yeni sınıf + Dictionary'ye bir satır. Checkout değişmez.");
        Console.WriteLine("Factory 'hangi nesneyi oluşturayım?', Strategy 'hangi algoritmayı kullanayım?' sorusunu cevaplar;");
        Console.WriteLine("Strategy'de algoritma nesne yaşarken değiştirilebilir.");

        return Task.CompletedTask;
    }
}

public interface IPricingStrategy
{
    string Name { get; }
    decimal Apply(decimal basePrice);
}

public sealed class RegularPricing : IPricingStrategy
{
    public string Name => "Normal fiyat";
    public decimal Apply(decimal basePrice) => basePrice;
}

public sealed class CampaignPricing(decimal discountRate) : IPricingStrategy
{
    public string Name => $"Kampanya (%{discountRate * 100:N0})";
    public decimal Apply(decimal basePrice) => basePrice * (1 - discountRate);
}

public sealed class CorporatePricing(decimal fixedDiscount) : IPricingStrategy
{
    public string Name => $"Kurumsal (-{fixedDiscount:N0} TL)";
    public decimal Apply(decimal basePrice) => Math.Max(0, basePrice - fixedDiscount);
}

/// <summary>Context: hesaplamayı kendisi yapmaz, elindeki stratejiye devreder.</summary>
public sealed class Checkout(IPricingStrategy strategy)
{
    public IPricingStrategy Strategy { get; private set; } = strategy;

    public void SetStrategy(IPricingStrategy strategy) => Strategy = strategy;

    public decimal Total(decimal basePrice) => Strategy.Apply(basePrice);
}

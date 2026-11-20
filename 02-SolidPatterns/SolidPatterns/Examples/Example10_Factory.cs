namespace DotnetBackendLab.SolidPatterns.Examples.Factory;

public sealed class Example10_Factory : IExample
{
    public int Number => 10;
    public string Title => "Factory: kargo türüne göre IPriceCalculator üreten factory";

    public Task RunAsync()
    {
        // Factory olmadan: her kullanan yer "hangi türse şu sınıfı new'le" kararını kendisi verir.
        // Bu karar kodun 10 yerine dağılırsa, yeni tür eklemek 10 yeri değiştirmek demektir.
        // Factory ile: nesne oluşturma kararı TEK yerde toplanır; kullanan sadece interface'i bilir.

        var orders = new (ShipmentType Type, decimal Kg)[]
        {
            (ShipmentType.Standard, 3),
            (ShipmentType.Express, 3),
            (ShipmentType.ColdChain, 3),
        };

        Console.WriteLine("Siparişler (istemci kod sadece factory'yi ve IPriceCalculator'ı biliyor):");
        foreach (var (type, kg) in orders)
        {
            IPriceCalculator calculator = PriceCalculatorFactory.Create(type);
            Console.WriteLine($"   {type,-10} -> {calculator.GetType().Name,-20} {calculator.Calculate(kg),7:N2} TL");
        }
        Console.WriteLine();

        try
        {
            PriceCalculatorFactory.Create((ShipmentType)99);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"Bilinmeyen tür: {ex.ParamName} = 99 -> hata tek yerden, anlaşılır şekilde fırlatılıyor.");
        }

        Console.WriteLine("Factory 'HANGİ nesneyi oluşturayım?' sorusunu cevaplar. ASP.NET Core'da bu işi çoğu zaman DI konteyneri yapar.");
        return Task.CompletedTask;
    }
}

public enum ShipmentType { Standard, Express, ColdChain }

public interface IPriceCalculator
{
    decimal Calculate(decimal kg);
}

public sealed class StandardCalculator : IPriceCalculator { public decimal Calculate(decimal kg) => 30m + kg * 10m; }
public sealed class ExpressCalculator : IPriceCalculator { public decimal Calculate(decimal kg) => 60m + kg * 15m; }

// Factory'nin faydası: oluşturma karmaşıksa (bağımlılık, ayar) bu karmaşıklık da burada saklı kalır.
public sealed class ColdChainCalculator(decimal iceBoxFee) : IPriceCalculator
{
    public decimal Calculate(decimal kg) => 80m + kg * 20m + iceBoxFee;
}

public static class PriceCalculatorFactory
{
    public static IPriceCalculator Create(ShipmentType type) => type switch
    {
        ShipmentType.Standard => new StandardCalculator(),
        ShipmentType.Express => new ExpressCalculator(),
        ShipmentType.ColdChain => new ColdChainCalculator(iceBoxFee: 25m),
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Bilinmeyen kargo türü"),
    };
}

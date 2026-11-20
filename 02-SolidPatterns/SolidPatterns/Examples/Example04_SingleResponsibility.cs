namespace DotnetBackendLab.SolidPatterns.Examples.SingleResponsibility;

public sealed class Example04_SingleResponsibility : IExample
{
    public int Number => 4;
    public string Title => "SingleResponsibility: hesaplayan + kaydeden + e-posta atan sınıfın bölünmesi";

    public Task RunAsync()
    {
        Console.WriteLine("KÖTÜ (tek sınıf, üç iş):");
        new ShipmentManager().Create("TR100", 4, "ayse@ornek.com");
        Console.WriteLine("   Bu sınıfın değişmesi için 3 ayrı sebep var: fiyat kuralı, veritabanı, e-posta sağlayıcısı.");
        Console.WriteLine("   Fiyatı test etmek isteyen, kayıt ve e-postayı da çalıştırmak zorunda.");
        Console.WriteLine();

        Console.WriteLine("İYİ (her sınıfın tek bir değişme sebebi var):");
        var service = new ShipmentService(new PriceCalculator(), new ShipmentStore(), new EmailNotifier());
        service.Create("TR200", 4, "mehmet@ornek.com");
        Console.WriteLine("   Fiyat kuralı değişirse sadece PriceCalculator, e-posta sağlayıcısı değişirse sadece EmailNotifier değişir.");
        Console.WriteLine($"   Fiyat tek başına test edilebilir: PriceCalculator.Calculate(4) = {new PriceCalculator().Calculate(4):N2} TL");

        return Task.CompletedTask;
    }
}

// KÖTÜ: hesaplama, kalıcılık ve bildirim aynı sınıfta.
public sealed class ShipmentManager
{
    private readonly List<string> _db = [];

    public void Create(string trackingNo, decimal kg, string email)
    {
        decimal price = 30m + kg * 12.5m;                                   // 1) iş kuralı
        _db.Add($"{trackingNo};{price}");                                   // 2) kalıcılık
        Console.WriteLine($"   [DB] {trackingNo} kaydedildi ({price:N2} TL)");
        Console.WriteLine($"   [SMTP] {email} adresine 'Kargonuz oluşturuldu' gönderildi");  // 3) bildirim
    }
}

// İYİ: her iş kendi sınıfında; ShipmentService sadece akışı yönetir.
public sealed class PriceCalculator
{
    public decimal Calculate(decimal kg) => 30m + kg * 12.5m;
}

public sealed class ShipmentStore
{
    private readonly List<(string TrackingNo, decimal Price)> _rows = [];

    public void Save(string trackingNo, decimal price)
    {
        _rows.Add((trackingNo, price));
        Console.WriteLine($"   [ShipmentStore] {trackingNo} kaydedildi ({price:N2} TL)");
    }
}

public sealed class EmailNotifier
{
    public void Send(string email, string message) => Console.WriteLine($"   [EmailNotifier] {email}: {message}");
}

public sealed class ShipmentService(PriceCalculator calculator, ShipmentStore store, EmailNotifier notifier)
{
    public void Create(string trackingNo, decimal kg, string email)
    {
        var price = calculator.Calculate(kg);
        store.Save(trackingNo, price);
        notifier.Send(email, "Kargonuz oluşturuldu");
    }
}

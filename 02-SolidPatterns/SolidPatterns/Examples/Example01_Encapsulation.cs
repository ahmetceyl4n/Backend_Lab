namespace DotnetBackendLab.SolidPatterns.Examples.Encapsulation;

public sealed class Example01_Encapsulation : IExample
{
    public int Number => 1;
    public string Title => "Encapsulation: public alanlı kargo vs private set + davranış metotları";

    public Task RunAsync()
    {
        // KÖTÜ: herkes alanları istediği gibi değiştirebilir; geçersiz durumlar mümkün.
        var bad = new OpenShipment { TrackingNo = "TR100", Status = "Teslim edildi" };
        bad.Status = "Hazırlanıyor";      // teslim edilmiş kargo geri hazırlanıyor?
        bad.Status = "Teslim edldi";      // yazım hatası da kabul ediliyor
        Console.WriteLine("KÖTÜ (public alanlar):");
        Console.WriteLine($"   Status = '{bad.Status}', DeliveredAt = {bad.DeliveredAt?.ToString() ?? "null"}  -> tutarsız durum");
        Console.WriteLine();

        // İYİ: durum sadece kurallı metotlarla değişir.
        var good = new Shipment("TR200");
        Console.WriteLine("İYİ (private set + Dispatch / Deliver):");
        Console.WriteLine($"   Başlangıç : {good.Status}");
        try
        {
            good.Deliver();               // yola çıkmadan teslim edilemez
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"   Deliver() : engellendi -> {ex.Message}");
        }
        good.Dispatch();
        good.Deliver();
        Console.WriteLine($"   Dispatch + Deliver sonrası: {good.Status}, DeliveredAt dolu mu? {good.DeliveredAt is not null}");
        // good.Status = ShipmentStatus.Preparing;   // derlenmez: CS0272, set private
        Console.WriteLine("   good.Status = ... dışarıdan derlenmez (private set).");

        return Task.CompletedTask;
    }
}

public sealed class OpenShipment
{
    public string TrackingNo = "";
    public string Status = "";
    public DateTime? DeliveredAt;
}

public enum ShipmentStatus { Preparing, InTransit, Delivered }

public sealed class Shipment(string trackingNo)
{
    public string TrackingNo { get; } = trackingNo;
    public ShipmentStatus Status { get; private set; } = ShipmentStatus.Preparing;
    public DateTime? DeliveredAt { get; private set; }

    public void Dispatch()
    {
        if (Status != ShipmentStatus.Preparing)
            throw new InvalidOperationException($"{Status} durumundaki kargo yola çıkarılamaz.");
        Status = ShipmentStatus.InTransit;
    }

    public void Deliver()
    {
        if (Status != ShipmentStatus.InTransit)
            throw new InvalidOperationException($"{Status} durumundaki kargo teslim edilemez.");
        Status = ShipmentStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;   // durum ve tarih her zaman birlikte değişir
    }
}

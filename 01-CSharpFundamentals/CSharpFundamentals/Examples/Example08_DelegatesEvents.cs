namespace DotnetBackendLab.CSharpFundamentals.Examples;

public sealed class Example08_DelegatesEvents : IExample
{
    public int Number => 8;
    public string Title => "DelegatesEvents: Func / Action / Predicate, teslimat event'i, event vs düz delegate";

    public Task RunAsync()
    {
        // 1) Hazır delegate tipleri: metodu değişken gibi taşımak.
        Func<int, decimal> priceOf = kg => 30m + kg * 10m;   // girdi alır, değer döndürür
        Action<string> log = msg => Console.WriteLine($"   [log] {msg}"); // değer döndürmez
        Predicate<int> isHeavy = kg => kg > 20;              // bool döndürür (Func<int,bool> ile aynı iş)

        Console.WriteLine("1) Func / Action / Predicate:");
        log($"8 kg fiyat = {priceOf(8)} TL, 25 kg ağır mı? {isHeavy(25)}");
        var weights = new List<int> { 3, 25, 12, 40 };
        log($"Ağır paketler (FindAll(Predicate)): {string.Join(", ", weights.FindAll(isHeavy))}");
        Console.WriteLine();

        // 2) event: kurye teslim edince abonelere haber verir.
        var courier = new DeliveryCourier();
        courier.Delivered += (_, e) => Console.WriteLine($"   SMS servisi   : {e.TrackingNo} teslim edildi");
        courier.Delivered += (_, e) => Console.WriteLine($"   Fatura servisi: {e.TrackingNo} için fatura kesildi");
        Console.WriteLine("2) event ile 2 abone:");
        courier.Deliver("TR100");
        Console.WriteLine();

        // 3) Düz delegate alanı: dışarıdan '=' ile ezilebilir ve dışarıdan çağrılabilir.
        var rawCourier = new DeliveryCourier();
        rawCourier.OnDeliveredRaw += no => Console.WriteLine($"   SMS servisi   : {no} teslim edildi");
        rawCourier.OnDeliveredRaw += no => Console.WriteLine($"   Fatura servisi: {no} için fatura kesildi");
        rawCourier.OnDeliveredRaw = no => Console.WriteLine($"   Yeni abone    : {no}");  // '=' -> önceki 2 abone SİLİNDİ
        Console.WriteLine("3) Düz delegate alanı, biri += yerine = yazınca (SMS ve Fatura kayboldu):");
        rawCourier.Deliver("TR200");
        Console.WriteLine("   Dışarıdan sahte tetikleme: rawCourier.OnDeliveredRaw(\"SAHTE\")");
        rawCourier.OnDeliveredRaw("SAHTE");   // kurye teslim etmediği halde abone tetiklendi
        Console.WriteLine();

        // event'te ikisi de derleme hatası:
        // courier.Delivered = null;                 // CS0070: sınıf dışında sadece += ve -= kullanılabilir
        // courier.Delivered(null, new("SAHTE"));    // CS0070: event'i sadece sahibi tetikleyebilir
        Console.WriteLine("event = delegate + koruma: dışarıdan sadece += / -=; tetiklemek yalnızca sahibine ait.");
        return Task.CompletedTask;
    }
}

public sealed class DeliveredEventArgs(string trackingNo) : EventArgs
{
    public string TrackingNo { get; } = trackingNo;
}

public sealed class DeliveryCourier
{
    public event EventHandler<DeliveredEventArgs>? Delivered;
    public Action<string>? OnDeliveredRaw;   // kötü örnek: korumasız public delegate alanı

    public void Deliver(string trackingNo)
    {
        Delivered?.Invoke(this, new DeliveredEventArgs(trackingNo));
        OnDeliveredRaw?.Invoke(trackingNo);
    }
}

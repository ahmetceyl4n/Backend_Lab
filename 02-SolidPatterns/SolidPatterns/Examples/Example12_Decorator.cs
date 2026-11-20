namespace DotnetBackendLab.SolidPatterns.Examples.Decorator;

public sealed class Example12_Decorator : IExample
{
    public int Number => 12;
    public string Title => "Decorator: IShipmentRepository'ye dokunmadan loglama ve cache eklemek";

    public Task RunAsync()
    {
        var db = new InMemoryShipmentRepository();
        var log = new List<string>();

        // Katmanlar dıştan içe: Logging -> Caching -> InMemory. Hepsi aynı interface'i uygular.
        IShipmentRepository repository = new LoggingShipmentRepository(new CachingShipmentRepository(db), log.Add);

        Console.WriteLine("Aynı takip numarası 3 kez okunuyor:");
        for (int i = 1; i <= 3; i++)
        {
            var shipment = repository.GetByTrackingNo("TR100");
            Console.WriteLine($"   {i}. okuma: {shipment}");
        }
        Console.WriteLine();
        Console.WriteLine($"Gerçek repository'ye gidilen sayı: {db.CallCount}  (1. okumadan sonra cache cevap verdi)");
        Console.WriteLine("Log kayıtları:");
        log.ForEach(l => Console.WriteLine($"   {l}"));
        Console.WriteLine();
        Console.WriteLine("InMemoryShipmentRepository'de tek satır değişmedi; özellikler sarmalayarak eklendi.");
        Console.WriteLine("Katmanların sırası da seçilebilir: Caching(Logging(db)) olsaydı cache'ten gelen okumalar loglanmazdı.");

        return Task.CompletedTask;
    }
}

public sealed record ShipmentInfo(string TrackingNo, string Status);

public interface IShipmentRepository
{
    ShipmentInfo? GetByTrackingNo(string trackingNo);
}

/// <summary>Asıl veri kaynağı (veritabanını temsil eder).</summary>
public sealed class InMemoryShipmentRepository : IShipmentRepository
{
    private readonly Dictionary<string, ShipmentInfo> _rows = new() { ["TR100"] = new("TR100", "Dağıtımda") };
    public int CallCount { get; private set; }

    public ShipmentInfo? GetByTrackingNo(string trackingNo)
    {
        CallCount++;
        return _rows.GetValueOrDefault(trackingNo);
    }
}

/// <summary>Decorator: çağrıyı içteki repository'ye iletir, öncesinde ve sonrasında log yazar.</summary>
public sealed class LoggingShipmentRepository(IShipmentRepository inner, Action<string> log) : IShipmentRepository
{
    public ShipmentInfo? GetByTrackingNo(string trackingNo)
    {
        log($"GetByTrackingNo('{trackingNo}') çağrıldı");
        var result = inner.GetByTrackingNo(trackingNo);
        log($"GetByTrackingNo('{trackingNo}') -> {(result is null ? "bulunamadı" : "bulundu")}");
        return result;
    }
}

/// <summary>Decorator: ilk sonuçtan sonra içteki repository'yi tekrar çağırmaz.</summary>
public sealed class CachingShipmentRepository(IShipmentRepository inner) : IShipmentRepository
{
    private readonly Dictionary<string, ShipmentInfo?> _cache = [];

    public ShipmentInfo? GetByTrackingNo(string trackingNo)
    {
        if (!_cache.TryGetValue(trackingNo, out var cached))
        {
            cached = inner.GetByTrackingNo(trackingNo);
            _cache[trackingNo] = cached;
        }
        return cached;
    }
}

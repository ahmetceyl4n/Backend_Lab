namespace DotnetBackendLab.SolidPatterns.Examples.UnitOfWork;

public sealed class Example13_RepositoryUnitOfWork : IExample
{
    public int Number => 13;
    public string Title => "RepositoryUnitOfWork: iki repository tek SaveChanges; hata olursa hiçbiri kaydedilmez";

    public Task RunAsync()
    {
        // KÖTÜ: her repository anında kaydediyor. İkinci kayıt patlarsa ilki yarım kalır.
        var db1 = new Database();
        try
        {
            new ImmediateShipmentStore(db1).Add(new Shipment("TR100", CourierId: 7));
            new ImmediateCourierStore(db1).Add(new Courier(7, Name: ""));   // geçersiz -> exception
        }
        catch (InvalidOperationException ex) { Console.WriteLine($"KÖTÜ: hata -> {ex.Message}"); }
        Console.WriteLine($"   Veritabanı: {db1.Shipments.Count} kargo, {db1.Couriers.Count} kurye  -> kargo var, kuryesi yok (tutarsız!)");
        Console.WriteLine();

        // İYİ: repository'ler değişikliği bekletir; UnitOfWork hepsini birlikte kaydeder ya da hiçbirini.
        var db2 = new Database();
        var uow = new UnitOfWork(db2);
        uow.Shipments.Add(new Shipment("TR200", CourierId: 8));
        uow.Couriers.Add(new Courier(8, Name: ""));                         // geçersiz
        try { uow.SaveChanges(); }
        catch (InvalidOperationException ex) { Console.WriteLine($"İYİ: hata -> {ex.Message}"); }
        Console.WriteLine($"   Veritabanı: {db2.Shipments.Count} kargo, {db2.Couriers.Count} kurye  -> hiçbiri yazılmadı (tutarlı)");

        uow.Shipments.Add(new Shipment("TR200", CourierId: 8));
        uow.Couriers.Add(new Courier(8, Name: "Ayşe"));
        uow.SaveChanges();
        Console.WriteLine($"   Düzeltip tekrar: {db2.Shipments.Count} kargo, {db2.Couriers.Count} kurye  -> ikisi birlikte yazıldı");
        Console.WriteLine();
        Console.WriteLine("EF Core'da DbContext zaten bir Unit of Work'tür: DbSet'ler repository, SaveChanges tek transaction.");

        return Task.CompletedTask;
    }
}

public sealed record Shipment(string TrackingNo, int CourierId);
public sealed record Courier(int Id, string Name);

/// <summary>Kalıcı veriyi temsil eder.</summary>
public sealed class Database
{
    public List<Shipment> Shipments { get; } = [];
    public List<Courier> Couriers { get; } = [];
}

public static class Rules
{
    public static void Validate(Shipment s) { if (string.IsNullOrWhiteSpace(s.TrackingNo)) throw new InvalidOperationException("Takip no boş olamaz"); }
    public static void Validate(Courier c) { if (string.IsNullOrWhiteSpace(c.Name)) throw new InvalidOperationException($"Kurye #{c.Id} adı boş olamaz"); }
}

// --- Kötü: anında kaydeden repository'ler ---
public sealed class ImmediateShipmentStore(Database db) { public void Add(Shipment s) { Rules.Validate(s); db.Shipments.Add(s); } }
public sealed class ImmediateCourierStore(Database db) { public void Add(Courier c) { Rules.Validate(c); db.Couriers.Add(c); } }

// --- İyi: bekleten repository'ler + UnitOfWork ---
public interface IRepository<T> { void Add(T item); }

public interface IUnitOfWork
{
    IRepository<Shipment> Shipments { get; }
    IRepository<Courier> Couriers { get; }
    void SaveChanges();
}

public sealed class PendingRepository<T> : IRepository<T>
{
    public List<T> Pending { get; } = [];
    public void Add(T item) => Pending.Add(item);   // veritabanına yazmaz, sadece not alır
}

public sealed class UnitOfWork(Database db) : IUnitOfWork
{
    private readonly PendingRepository<Shipment> _shipments = new();
    private readonly PendingRepository<Courier> _couriers = new();

    public IRepository<Shipment> Shipments => _shipments;
    public IRepository<Courier> Couriers => _couriers;

    public void SaveChanges()
    {
        try
        {
            // 1) Önce HEPSİNİ doğrula; biri bile geçersizse hiçbir şey yazılmaz.
            _shipments.Pending.ForEach(Rules.Validate);
            _couriers.Pending.ForEach(Rules.Validate);

            // 2) Hepsi geçerliyse birlikte yaz (gerçek veritabanında bu adım tek transaction olur).
            db.Shipments.AddRange(_shipments.Pending);
            db.Couriers.AddRange(_couriers.Pending);
        }
        finally
        {
            _shipments.Pending.Clear();   // başarılı da olsa başarısız da olsa bekleyenler temizlenir
            _couriers.Pending.Clear();
        }
    }
}

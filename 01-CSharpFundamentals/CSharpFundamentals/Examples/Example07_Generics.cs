namespace DotnetBackendLab.CSharpFundamentals.Examples;

public sealed class Example07_Generics : IExample
{
    public int Number => 7;
    public string Title => "Generics: generic repository ve cache, constraint'ler (where T : class, new())";

    public Task RunAsync()
    {
        // 1) Tek repository kodu, farklı tipler için tip güvenli çalışır. object + cast yok.
        var branches = new InMemoryRepository<Branch>();
        branches.Add(new Branch { Id = 1, Name = "Kadıköy" });
        branches.Add(new Branch { Id = 2, Name = "Çankaya" });

        var couriers = new InMemoryRepository<Courier>();
        couriers.Add(new Courier { Id = 7, Name = "Ayşe" });

        Console.WriteLine("1) InMemoryRepository<T> where T : class, IEntity");
        Console.WriteLine($"   Şube #2   : {branches.GetById(2)?.Name}");
        Console.WriteLine($"   Kurye #7  : {couriers.GetById(7)?.Name}");
        Console.WriteLine($"   Şube #99  : {branches.GetById(99)?.Name ?? "(yok -> null)"}");
        Console.WriteLine();

        // 2) new() kısıtı sayesinde cache, eksik anahtar için T'yi kendisi oluşturabilir.
        var stats = new DefaultingCache<string, BranchStats>();
        stats.GetOrCreate("Kadıköy").Delivered += 3;
        stats.GetOrCreate("Kadıköy").Delivered += 2;
        stats.GetOrCreate("Çankaya").Delivered += 1;
        Console.WriteLine("2) DefaultingCache<TKey, TValue> where TValue : class, new()");
        Console.WriteLine($"   Kadıköy teslim: {stats.GetOrCreate("Kadıköy").Delivered}, Çankaya teslim: {stats.GetOrCreate("Çankaya").Delivered}, kayıt sayısı: {stats.Count}");
        Console.WriteLine();

        // Constraint'ler derleme zamanında kontrol edilir:
        // new InMemoryRepository<string>();        // CS0311: string IEntity uygulamıyor
        // new DefaultingCache<string, int>();      // CS0452: int bir class değil
        // new DefaultingCache<string, Branch2>();  // CS0310: parametresiz public ctor yoksa new() sağlanmaz
        Console.WriteLine("3) Kısıtı sağlamayan tip (ör. DefaultingCache<string, int>) derleme hatası verir, çalışma zamanına kalmaz.");

        return Task.CompletedTask;
    }
}

public interface IEntity
{
    int Id { get; }
}

public sealed class InMemoryRepository<T> where T : class, IEntity
{
    private readonly Dictionary<int, T> _items = [];

    public void Add(T item) => _items[item.Id] = item;     // IEntity kısıtı sayesinde item.Id erişilebilir
    public T? GetById(int id) => _items.GetValueOrDefault(id); // class kısıtı sayesinde T? = null anlamına gelir
}

public sealed class DefaultingCache<TKey, TValue>
    where TKey : notnull
    where TValue : class, new()
{
    private readonly Dictionary<TKey, TValue> _items = [];
    public int Count => _items.Count;

    public TValue GetOrCreate(TKey key)
    {
        if (!_items.TryGetValue(key, out var value))
        {
            value = new TValue();   // new() kısıtı olmadan bu satır derlenmez
            _items[key] = value;
        }
        return value;
    }
}

public sealed class Branch : IEntity { public int Id { get; init; } public string Name { get; init; } = ""; }
public sealed class Courier : IEntity { public int Id { get; init; } public string Name { get; init; } = ""; }
public sealed class BranchStats { public int Delivered { get; set; } }

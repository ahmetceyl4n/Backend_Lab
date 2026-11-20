namespace DotnetBackendLab.SolidPatterns.Examples.Singleton;

public sealed class Example09_Singleton : IExample
{
    public int Number => 9;
    public string Title => "Singleton: thread-safe olmayan, lock'lu ve Lazy<T> sürümleri";

    public Task RunAsync()
    {
        const int threads = 8;
        Console.WriteLine($"{threads} thread aynı anda Instance istiyor (hepsi aynı anda başlasın diye kapıda bekletiliyor):");

        UnsafeTrackingNoGenerator.ResetForDemo();
        var unsafeResult = SingletonProbe.Run(() => UnsafeTrackingNoGenerator.Instance, threads);
        Console.WriteLine($"   Korumasız  : farklı örnek = {unsafeResult}, constructor çağrısı = {UnsafeTrackingNoGenerator.CreatedCount}  (tek olmalıydı!)");

        var lockedResult = SingletonProbe.Run(() => LockedTrackingNoGenerator.Instance, threads);
        Console.WriteLine($"   lock       : farklı örnek = {lockedResult}, constructor çağrısı = {LockedTrackingNoGenerator.CreatedCount}");

        var lazyResult = SingletonProbe.Run(() => LazyTrackingNoGenerator.Instance, threads);
        Console.WriteLine($"   Lazy<T>    : farklı örnek = {lazyResult}, constructor çağrısı = {LazyTrackingNoGenerator.CreatedCount}");
        Console.WriteLine();
        Console.WriteLine($"Sonraki takip no: {LazyTrackingNoGenerator.Instance.Next()}, {LazyTrackingNoGenerator.Instance.Next()} (tek sayaç, herkes paylaşıyor)");
        Console.WriteLine("ASP.NET Core'da elle singleton yazmak yerine: builder.Services.AddSingleton<ITrackingNoGenerator, ...>();");

        return Task.CompletedTask;
    }
}

public static class SingletonProbe
{
    /// <summary>Verilen sayıda thread'i aynı anda başlatıp kaç farklı örnek döndüğünü sayar.</summary>
    public static int Run(Func<object> getInstance, int threadCount)
    {
        using var gate = new ManualResetEventSlim(false);
        var results = new object[threadCount];
        var workers = Enumerable.Range(0, threadCount)
            .Select(i => new Thread(() => { gate.Wait(); results[i] = getInstance(); }))
            .ToList();
        workers.ForEach(t => t.Start());
        gate.Set();                         // hepsini aynı anda serbest bırak
        workers.ForEach(t => t.Join());
        return results.Distinct(ReferenceEqualityComparer.Instance).Count();
    }
}

// KÖTÜ: iki thread aynı anda "_instance null" görürse ikisi de new yapar.
public sealed class UnsafeTrackingNoGenerator
{
    private static UnsafeTrackingNoGenerator? _instance;
    public static int CreatedCount;

    private UnsafeTrackingNoGenerator() => Interlocked.Increment(ref CreatedCount);

    public static UnsafeTrackingNoGenerator Instance
    {
        get
        {
            if (_instance is null)
            {
                Thread.Sleep(20);   // yarış penceresini genişlet (gerçekte constructor'ın pahalı olması gibi)
                _instance = new UnsafeTrackingNoGenerator();
            }
            return _instance;
        }
    }

    public static void ResetForDemo() { _instance = null; CreatedCount = 0; }   // sadece gösterim için
}

// lock: kontrol + oluşturma tek kritik bölgede.
public sealed class LockedTrackingNoGenerator
{
    private static readonly object Gate = new();
    private static LockedTrackingNoGenerator? _instance;
    public static int CreatedCount;

    private LockedTrackingNoGenerator() { Thread.Sleep(20); Interlocked.Increment(ref CreatedCount); }

    public static LockedTrackingNoGenerator Instance
    {
        get { lock (Gate) return _instance ??= new LockedTrackingNoGenerator(); }
    }
}

// Lazy<T>: thread güvenliğini .NET üstlenir (varsayılan mod: ExecutionAndPublication).
public sealed class LazyTrackingNoGenerator
{
    private static readonly Lazy<LazyTrackingNoGenerator> LazyInstance = new(() => new LazyTrackingNoGenerator());
    public static int CreatedCount;
    private int _counter;

    private LazyTrackingNoGenerator() { Thread.Sleep(20); Interlocked.Increment(ref CreatedCount); }

    public static LazyTrackingNoGenerator Instance => LazyInstance.Value;

    public string Next() => $"TR{Interlocked.Increment(ref _counter):D6}";
}

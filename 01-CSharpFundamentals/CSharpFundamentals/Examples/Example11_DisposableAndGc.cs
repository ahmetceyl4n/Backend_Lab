using System.Runtime.CompilerServices;

namespace DotnetBackendLab.CSharpFundamentals.Examples;

public sealed class Example11_DisposableAndGc : IExample
{
    public int Number => 11;
    public string Title => "DisposableAndGc: using, using var, finalizer zamanlaması, GC nesilleri";

    public Task RunAsync()
    {
        var log = new List<string>();

        // 1) using bloğu: blok bitince Dispose çağrılır (hata olsa bile; derleyici try/finally üretir).
        using (var printer = new LabelPrinter("Yazıcı-1", log))
        {
            printer.Print("TR100");
        }
        log.Add("using bloğundan sonraki satır");

        // 2) using var: Dispose, değişkenin bulunduğu kapsam (burada PrintBatch metodu) bitince çağrılır.
        PrintBatch(log);
        log.Add("PrintBatch döndü");

        Console.WriteLine("1-2) using / using var sırası:");
        log.ForEach(l => Console.WriteLine($"   {l}"));
        Console.WriteLine();

        // 3) Finalizer: Dispose edilmeyen nesneyi GC ne zaman toplarsa o zaman çalışır. Zamanı belli değildir.
        CreateAndForget();
        Console.WriteLine("3) Finalizer:");
        Console.WriteLine("   Nesne bırakıldı. GC henüz çalışmadı -> finalizer çıktısı yok.");
        GC.Collect();                    // sadece gösterim için zorluyoruz; gerçek kodda yapılmaz
        GC.WaitForPendingFinalizers();
        Console.WriteLine("   GC.Collect + WaitForPendingFinalizers sonrası ^ (finalizer başka bir thread'de çalıştı)");
        Console.WriteLine();

        // 4) GC nesilleri: hayatta kalan nesne her toplamadan sonra bir üst nesle terfi eder (0 -> 1 -> 2).
        var cache = new byte[1024];
        Console.Write($"4) GC nesli: yeni = {GC.GetGeneration(cache)}");
        GC.Collect();
        Console.Write($", 1. Collect = {GC.GetGeneration(cache)}");
        GC.Collect();
        Console.WriteLine($", 2. Collect = {GC.GetGeneration(cache)}  (uzun yaşayanlar gen2'ye yerleşir)");
        Console.WriteLine($"   85.000 byte üstü dizi doğrudan LOH'a gider: GetGeneration = {GC.GetGeneration(new byte[100_000])}");

        return Task.CompletedTask;
    }

    private static void PrintBatch(List<string> log)
    {
        using var printer = new LabelPrinter("Yazıcı-2", log);
        printer.Print("TR200");
        printer.Print("TR201");
        log.Add("PrintBatch'in son satırı");
    } // <- Dispose tam burada

    // Ayrı metot + NoInlining: Debug'da yerel değişken metot sonuna kadar canlı tutulmasın.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void CreateAndForget() => _ = new LeakyConnection();
}

public sealed class LabelPrinter(string name, List<string> log) : IDisposable
{
    public bool IsDisposed { get; private set; }

    public void Print(string trackingNo) => log.Add($"{name}: {trackingNo} etiketi basıldı");

    public void Dispose()
    {
        if (IsDisposed) return;      // iki kez çağrılsa da güvenli
        IsDisposed = true;
        log.Add($"{name}: Dispose -> bağlantı kapatıldı");
    }
}

public sealed class LeakyConnection
{
    // Finalizer: yönetilmeyen kaynak için son güvenlik ağı. Ne zaman çalışacağı GC'ye bağlı.
    ~LeakyConnection() => Console.WriteLine("   ~LeakyConnection: finalizer çalıştı (Dispose çağrılmamıştı)");
}

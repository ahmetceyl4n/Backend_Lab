namespace DotnetBackendLab.CSharpFundamentals.Examples;

public sealed class Example09_LinqDeferred : IExample
{
    public int Number => 9;
    public string Title => "LinqDeferred: sorgu enumerate edilince çalışır, ToList, iki kez enumerate, yield";

    public Task RunAsync()
    {
        var weights = new List<int> { 3, 15, 8, 22 };
        var counter = new CallCounter();

        // 1) Tanımlamak çalıştırmaz: Where sadece "nasıl filtreleneceğini" saklar.
        var heavy = BuildHeavyQuery(weights, counter);
        Console.WriteLine($"1) Sorgu tanımlandı           -> filtre çağrı sayısı: {counter.Count}");

        // 2) Listeye sonradan eklenen eleman da sorguya girer (sorgu henüz çalışmadı).
        weights.Add(30);
        Console.WriteLine($"2) Listeye 30 eklendi, enumerate: [{string.Join(", ", heavy)}]  -> çağrı: {counter.Count}");

        // 3) Aynı sorguyu tekrar enumerate etmek filtreyi BAŞTAN çalıştırır.
        int howMany = heavy.Count();
        Console.WriteLine($"3) heavy.Count() = {howMany}                  -> çağrı: {counter.Count}  (iki kez çalıştı)");

        // 4) ToList: sorguyu hemen bir kez çalıştırır, sonucu dondurur.
        counter.Count = 0;
        var snapshot = heavy.ToList();
        weights.Add(50);
        Console.WriteLine($"4) ToList sonra 50 eklendi    -> snapshot: [{string.Join(", ", snapshot)}], sorgu: [{string.Join(", ", heavy)}]");
        Console.WriteLine($"   snapshot 50'yi görmez; sorgu görür. Çağrı: {counter.Count} (ToList 5 + tekrar enumerate 6)");
        Console.WriteLine();

        // 5) yield return ile kendi iterator'ın: her eleman istendiği anda üretilir.
        var log = new List<string>();
        var scans = ReadScanEvents(log);
        Console.WriteLine($"5) ReadScanEvents çağrıldı    -> log: {log.Count} satır (metodun içine hiç girilmedi)");
        foreach (var scan in scans.Take(2))
        {
            Console.WriteLine($"   tüketildi: {scan}");
        }
        Console.WriteLine($"   Take(2) sonrası log: [{string.Join(" | ", log)}]  -> 3. olay hiç üretilmedi");

        return Task.CompletedTask;
    }

    /// <summary>Her filtre çağrısını sayan, ertelenmiş (deferred) LINQ sorgusu.</summary>
    public static IEnumerable<int> BuildHeavyQuery(List<int> weights, CallCounter counter) =>
        weights.Where(w =>
        {
            counter.Count++;
            return w > 10;
        });

    /// <summary>yield ile tarama olaylarını tek tek üretir; ürettiği her olayı log'a yazar.</summary>
    public static IEnumerable<string> ReadScanEvents(List<string> log)
    {
        string[] events = ["TR100 şubeye girdi", "TR100 dağıtımda", "TR100 teslim edildi"];
        foreach (var e in events)
        {
            log.Add($"üretildi: {e}");
            yield return e;   // burada durur, sonraki eleman istenince kaldığı yerden devam eder
        }
    }
}

public sealed class CallCounter
{
    public int Count { get; set; }
}

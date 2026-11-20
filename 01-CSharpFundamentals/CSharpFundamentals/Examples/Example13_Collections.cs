using System.Diagnostics;

namespace DotnetBackendLab.CSharpFundamentals.Examples;

public sealed class Example13_Collections : IExample
{
    public int Number => 13;
    public string Title => "Collections: dizi, List, Dictionary, HashSet, Queue, Stack ve arama/ekleme süreleri";

    public Task RunAsync()
    {
        // 1) Kısa tanıtım: her koleksiyon ne işe yarar?
        string[] branches = ["Kadıköy", "Çankaya", "Konak"];               // sabit boyut
        var queue = new Queue<string>(["TR1", "TR2", "TR3"]);               // FIFO: teslimat sırası
        var stack = new Stack<string>(["koli-alt", "koli-orta", "koli-üst"]); // LIFO: üst üste koliler
        Console.WriteLine("1) Tanıtım:");
        Console.WriteLine($"   Dizi    : {branches.Length} şube, branches[1] = {branches[1]} (boyut sabit)");
        Console.WriteLine($"   Queue   : Dequeue() = {queue.Dequeue()} (ilk giren ilk çıkar)");
        Console.WriteLine($"   Stack   : Pop()     = {stack.Pop()} (son giren ilk çıkar)");
        Console.WriteLine();

        // 2) 100.000 takip numarası; listenin SONUNDAKİ numaralar 1.000 kez aranıyor (en kötü durum).
        const int count = 100_000;
        const int lookups = 1_000;
        var list = Enumerable.Range(0, count).Select(i => $"TR{i}").ToList();
        var set = new HashSet<string>(list);
        var dict = list.ToDictionary(no => no, no => new Parcel(no, 1));
        var parcels = dict.Values.ToList();
        var targets = Enumerable.Range(count - lookups, lookups).Select(i => $"TR{i}").ToArray();

        Console.WriteLine($"2) {count:N0} eleman, {lookups:N0} arama:");
        Measure("List.Contains            (O(n))", () => targets.Count(list.Contains));
        Measure("HashSet.Contains         (O(1))", () => targets.Count(set.Contains));
        Measure("List + FirstOrDefault    (O(n))", () => targets.Count(t => parcels.FirstOrDefault(p => p.TrackingNo == t) is not null));
        Measure("Dictionary.TryGetValue   (O(1))", () => targets.Count(t => dict.TryGetValue(t, out _)));
        Console.WriteLine();

        // 3) List'e başa ekleme her seferinde bütün elemanları bir sağa kaydırır.
        const int inserts = 50_000;
        Console.WriteLine($"3) List'e {inserts:N0} eleman ekleme:");
        Measure("Add (sona)               (O(1))", () => { var l = new List<int>(); for (int i = 0; i < inserts; i++) l.Add(i); return l.Count; });
        Measure("Insert(0, ...) (başa)    (O(n))", () => { var l = new List<int>(); for (int i = 0; i < inserts; i++) l.Insert(0, i); return l.Count; });

        return Task.CompletedTask;
    }

    private static void Measure(string label, Func<int> work)
    {
        var sw = Stopwatch.StartNew();
        int result = work();
        sw.Stop();
        Console.WriteLine($"   {label}: {sw.Elapsed.TotalMilliseconds,8:N1} ms (sonuç: {result:N0})");
    }
}

using System.Collections;
using System.Diagnostics;

namespace DotnetBackendLab.CSharpFundamentals.Examples;

public sealed class Example04_Boxing : IExample
{
    public int Number => 4;
    public string Title => "Boxing: int -> object, ArrayList vs List<int> süre ve bellek farkı";

    public Task RunAsync()
    {
        // 1) Boxing: int heap'te yeni bir object kutusuna kopyalanır.
        int weight = 42;
        object boxed = weight;      // boxing (kopya)
        weight = 99;
        int unboxed = (int)boxed;   // unboxing (tipi tam tutmalı; (long)boxed InvalidCastException verir)
        Console.WriteLine("1) object boxed = weight; weight = 99;");
        Console.WriteLine($"   weight = {weight}, boxed = {boxed}, unboxed = {unboxed}  -> kutu ayrı bir kopya");
        object boxedAgain = 42;     // aynı değer, YENİ bir kutu
        Console.WriteLine($"   boxed.GetType() = {boxed.GetType().Name}");
        Console.WriteLine($"   boxed == boxedAgain : {boxed == boxedAgain}  (object == referans karşılaştırır: iki ayrı kutu)");
        Console.WriteLine($"   boxed.Equals(boxedAgain): {boxed.Equals(boxedAgain)}  (Equals içerideki int değerini karşılaştırır)");
        Console.WriteLine();

        // 2) 1.000.000 kargo ağırlığı ekle ve topla.
        const int count = 1_000_000;
        Console.WriteLine($"2) {count:N0} ağırlık ekle + topla:");
        Measure("ArrayList (her eleman box) ", () =>
        {
            var list = new ArrayList();
            for (int i = 0; i < count; i++) list.Add(i % 50);  // Add(object) -> boxing
            long sum = 0;
            foreach (object o in list) sum += (int)o;          // unboxing
            return sum;
        });
        Measure("List<int> (boxing yok)     ", () =>
        {
            var list = new List<int>();
            for (int i = 0; i < count; i++) list.Add(i % 50);  // Add(int) -> doğrudan diziye
            long sum = 0;
            foreach (int w in list) sum += w;
            return sum;
        });

        Console.WriteLine();
        Console.WriteLine("ArrayList her int için heap'te ~24 byte'lık kutu açar; List<int> int'leri dizide yan yana tutar.");
        return Task.CompletedTask;
    }

    private static void Measure(string label, Func<long> work)
    {
        work(); // ısınma (JIT)
        GC.Collect();
        long before = GC.GetAllocatedBytesForCurrentThread();
        var sw = Stopwatch.StartNew();
        long sum = work();
        sw.Stop();
        long allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        Console.WriteLine($"   {label}: {sw.ElapsedMilliseconds,4} ms, ayrılan bellek ~{allocated / 1024 / 1024,3} MB (toplam={sum:N0})");
    }
}

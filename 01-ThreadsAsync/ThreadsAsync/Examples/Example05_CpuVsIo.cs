using System.Diagnostics;

namespace DotnetBackendLab.ThreadsAsync.Examples;

public sealed class Example05_CpuVsIo : IExample
{
    public int Number => 5;
    public string Title => "CpuVsIo: Parallel.For (CPU) kazancı, I/O'yu Task.Run'a sarmak kazandırmaz";

    public async Task RunAsync()
    {
        const int cpuItems = 24;

        // CPU işi (hacimsel ağırlık hesabı): Parallel.For çekirdeklere böler -> hızlanır.
        var sw = Stopwatch.StartNew();
        long seq = 0;
        for (int i = 0; i < cpuItems; i++) seq += HeavyCompute(i);
        sw.Stop();
        Console.WriteLine($"CPU sıralı for         : {sw.ElapsedMilliseconds,5} ms");

        sw.Restart();
        long par = 0;
        Parallel.For(0, cpuItems, () => 0L,
            (i, _, local) => local + HeavyCompute(i),
            local => Interlocked.Add(ref par, local));
        sw.Stop();
        Console.WriteLine($"CPU Parallel.For       : {sw.ElapsedMilliseconds,5} ms (çekirdekler bölüştü, aynı sonuç: {seq == par})");
        Console.WriteLine();

        // I/O işi zaten beklemedir. Paralel kazanç WhenAll'dan gelir; Task.Run'a sarmak
        // ekstra hız getirmez, sadece gereksiz havuz thread'i harcar.
        const int ioItems = 20;
        sw.Restart();
        for (int i = 0; i < ioItems; i++) await FetchAsync(i);
        sw.Stop();
        Console.WriteLine($"I/O sıralı await       : {sw.ElapsedMilliseconds,5} ms");

        sw.Restart();
        await Task.WhenAll(Enumerable.Range(0, ioItems).Select(FetchAsync));
        sw.Stop();
        Console.WriteLine($"I/O WhenAll (doğrudan) : {sw.ElapsedMilliseconds,5} ms");

        sw.Restart();
        await Task.WhenAll(Enumerable.Range(0, ioItems).Select(i => Task.Run(() => FetchAsync(i))));
        sw.Stop();
        Console.WriteLine($"I/O WhenAll + Task.Run : {sw.ElapsedMilliseconds,5} ms (aynı süre; Task.Run kazanç getirmez)");
    }

    private static long HeavyCompute(int seed)
    {
        long sum = 0;
        for (int i = 1; i < 5_000_000; i++) sum += (seed + i) % 7;
        return sum;
    }

    private static async Task FetchAsync(int index) => await Task.Delay(50); // dış servis gecikmesi
}

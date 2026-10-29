using System.Diagnostics;

namespace DotnetBackendLab.ThreadsAsync.Examples;

public sealed class Example06_LimitedConcurrency : IExample
{
    public int Number => 6;
    public string Title => "LimitedConcurrency: WhenAll / SemaphoreSlim(10) / Parallel.ForEachAsync(10)";

    public async Task RunAsync()
    {
        const int total = 1000;
        Console.WriteLine($"{total} sahte dış API çağrısı. Sahte API en yüksek eşzamanlı istek sayısını ölçüyor.");
        Console.WriteLine();

        var sw = Stopwatch.StartNew();
        int unlimited = await RunUnlimitedAsync(total);
        sw.Stop();
        Console.WriteLine($"Sınırsız WhenAll          : en yüksek eşzamanlı = {unlimited,4} ({sw.ElapsedMilliseconds} ms)");

        sw.Restart();
        int semaphore = await RunWithSemaphoreAsync(total, 10);
        sw.Stop();
        Console.WriteLine($"SemaphoreSlim(10)         : en yüksek eşzamanlı = {semaphore,4} ({sw.ElapsedMilliseconds} ms)");

        sw.Restart();
        int forEach = await RunWithParallelForEachAsync(total, 10);
        sw.Stop();
        Console.WriteLine($"Parallel.ForEachAsync(10) : en yüksek eşzamanlı = {forEach,4} ({sw.ElapsedMilliseconds} ms)");
    }

    // Sınır yok: 1000 çağrı aynı anda başlar, eşzamanlılık tavan yapar.
    public static async Task<int> RunUnlimitedAsync(int total)
    {
        var api = new FakeExternalApi();
        await Task.WhenAll(Enumerable.Range(0, total).Select(_ => api.CallAsync()));
        return api.MaxConcurrency;
    }

    // SemaphoreSlim(limit): aynı anda en fazla 'limit' çağrı içeri girebilir.
    public static async Task<int> RunWithSemaphoreAsync(int total, int limit)
    {
        var api = new FakeExternalApi();
        using var gate = new SemaphoreSlim(limit);
        var tasks = Enumerable.Range(0, total).Select(async _ =>
        {
            await gate.WaitAsync();
            try { await api.CallAsync(); }
            finally { gate.Release(); }
        });
        await Task.WhenAll(tasks);
        return api.MaxConcurrency;
    }

    // Parallel.ForEachAsync: MaxDegreeOfParallelism eşzamanlılığı doğrudan sınırlar.
    public static async Task<int> RunWithParallelForEachAsync(int total, int limit)
    {
        var api = new FakeExternalApi();
        var options = new ParallelOptions { MaxDegreeOfParallelism = limit };
        await Parallel.ForEachAsync(Enumerable.Range(0, total), options, async (_, _) => await api.CallAsync());
        return api.MaxConcurrency;
    }
}

/// <summary>Sahte dış API: aynı anda kaç isteğin işlendiğini (tepe değer) ölçer.</summary>
public sealed class FakeExternalApi
{
    private int _current;
    private int _max;

    public int MaxConcurrency => Volatile.Read(ref _max);

    public async Task CallAsync()
    {
        int now = Interlocked.Increment(ref _current);
        int snapshot;
        while (now > (snapshot = Volatile.Read(ref _max)))
        {
            Interlocked.CompareExchange(ref _max, now, snapshot);
        }
        await Task.Delay(5); // dış API gecikmesi
        Interlocked.Decrement(ref _current);
    }
}

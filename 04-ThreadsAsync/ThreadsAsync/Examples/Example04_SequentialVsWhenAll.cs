using System.Diagnostics;

namespace DotnetBackendLab.ThreadsAsync.Examples;

public sealed class Example04_SequentialVsWhenAll : IExample
{
    public int Number => 4;
    public string Title => "SequentialVsWhenAll: 3 servis çağrısı sırayla vs Task.WhenAll";

    public async Task RunAsync()
    {
        // Sırayla await: her çağrı bir öncekini bekler -> süreler toplanır (~900 ms).
        var sw = Stopwatch.StartNew();
        var a = await QueryBranchAsync("İstanbul", 300);
        var b = await QueryBranchAsync("Ankara", 300);
        var c = await QueryBranchAsync("İzmir", 300);
        sw.Stop();
        Console.WriteLine($"Sırayla : {a}, {b}, {c}");
        Console.WriteLine($"   Süre: {sw.ElapsedMilliseconds} ms (~900, toplanır)");
        Console.WriteLine();

        // WhenAll: üçü aynı anda başlar -> en yavaş olan kadar sürer (~300 ms).
        sw.Restart();
        var results = await Task.WhenAll(
            QueryBranchAsync("İstanbul", 300),
            QueryBranchAsync("Ankara", 300),
            QueryBranchAsync("İzmir", 300));
        sw.Stop();
        Console.WriteLine($"WhenAll : {string.Join(", ", results)}");
        Console.WriteLine($"   Süre: {sw.ElapsedMilliseconds} ms (~300, paralel)");
    }

    private static async Task<string> QueryBranchAsync(string city, int ms)
    {
        await Task.Delay(ms); // şube servisine çağrı simülasyonu
        return $"{city}:{Random.Shared.Next(10, 99)} paket";
    }
}

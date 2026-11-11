using System.Diagnostics;

namespace DotnetBackendLab.ThreadsAsync.Examples;

public sealed class Example03_AwaitThreads : IExample
{
    public int Number => 3;
    public string Title => "AwaitThreads: await öncesi/sonrası thread, Thread.Sleep vs Task.Delay";

    public async Task RunAsync()
    {
        Console.WriteLine($"await ÖNCESİ thread#: {Environment.CurrentManagedThreadId}");
        await Task.Delay(100);
        Console.WriteLine($"await SONRASI thread#: {Environment.CurrentManagedThreadId} (değişmiş olabilir)");
        Console.WriteLine();

        const int jobs = 200;

        // Thread.Sleep bloklar: Task.Run ile havuza atılsa bile havuz thread'leri tükenir,
        // işler gruplar hâlinde ilerler -> toplam süre uzar.
        var sw = Stopwatch.StartNew();
        await Task.WhenAll(Enumerable.Range(0, jobs).Select(_ => Task.Run(() => Thread.Sleep(50))));
        sw.Stop();
        Console.WriteLine($"Thread.Sleep x{jobs}: {sw.ElapsedMilliseconds,5} ms (thread'ler bloklandı)");

        // Task.Delay bloklamaz: thread'i serbest bırakır -> işler neredeyse tümüyle paralel.
        sw.Restart();
        await Task.WhenAll(Enumerable.Range(0, jobs).Select(_ => Task.Delay(50)));
        sw.Stop();
        Console.WriteLine($"Task.Delay  x{jobs}: {sw.ElapsedMilliseconds,5} ms (thread bloklanmadı)");
    }
}

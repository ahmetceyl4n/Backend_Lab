using System.Diagnostics;
using DotnetBackendLab.ThreadsAsync.Examples;

namespace DotnetBackendLab.ThreadsAsync;

/// <summary>Örnekleri kaydeder, menüyü basar ve seçilen örneği süresini ölçerek çalıştırır.</summary>
public sealed class ExampleRunner
{
    private readonly IReadOnlyList<IExample> _examples;

    public ExampleRunner()
    {
        _examples =
        [
            new Example01_ThreadPoolReuse(),
            new Example02_ThreadVsTask(),
            new Example03_AwaitThreads(),
            new Example04_SequentialVsWhenAll(),
            new Example05_CpuVsIo(),
            new Example06_LimitedConcurrency(),
            new Example07_WhenAllWhenAny(),
            new Example08_RaceCondition(),
            new Example09_BankAccount(),
            new Example10_LockOrderDeadlock(),
            new Example11_Cancellation(),
        ];
    }

    public bool Has(int number) => _examples.Any(e => e.Number == number);

    public void PrintMenu()
    {
        Console.WriteLine();
        Console.WriteLine("=== 01-ThreadsAsync Örnekleri ===");
        foreach (var e in _examples)
        {
            Console.WriteLine($"  {e.Number,2}. {e.Title}");
        }
    }

    public async Task RunAsync(int number)
    {
        var example = _examples.First(e => e.Number == number);
        Console.WriteLine();
        Console.WriteLine($"--- {example.Number}. {example.Title} ---");

        var sw = Stopwatch.StartNew();
        await example.RunAsync();
        sw.Stop();

        Console.WriteLine($"--- Bitti: toplam {sw.ElapsedMilliseconds} ms ---");
    }
}

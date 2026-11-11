using System.Diagnostics;
using DotnetBackendLab.CSharpFundamentals.Examples;

namespace DotnetBackendLab.CSharpFundamentals;

/// <summary>Örnekleri kaydeder, menüyü basar ve seçilen örneği süresini ölçerek çalıştırır.</summary>
public sealed class ExampleRunner
{
    private readonly IReadOnlyList<IExample> _examples;

    public ExampleRunner()
    {
        _examples =
        [
            new Example01_ValueVsReference(),
            new Example02_ParameterPassing(),
            new Example03_StructInsideClass(),
            new Example04_Boxing(),
            new Example05_ClassStructRecord(),
            new Example06_InterfaceVsAbstract(),
            new Example07_Generics(),
            new Example08_DelegatesEvents(),
            new Example09_LinqDeferred(),
            new Example10_ExceptionHandling(),
            new Example11_DisposableAndGc(),
            new Example12_StringAndEquality(),
        ];
    }

    public bool Has(int number) => _examples.Any(e => e.Number == number);

    public void PrintMenu()
    {
        Console.WriteLine();
        Console.WriteLine("=== 01-CSharpFundamentals Örnekleri ===");
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

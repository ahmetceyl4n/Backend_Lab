using System.Diagnostics;
// Her örneğin tipleri kendi alt namespace'inde (ör. iki örnekte de IPriceCalculator var, çakışmasınlar).
using DotnetBackendLab.SolidPatterns.Examples.Composition;
using DotnetBackendLab.SolidPatterns.Examples.Decorator;
using DotnetBackendLab.SolidPatterns.Examples.DependencyInversion;
using DotnetBackendLab.SolidPatterns.Examples.Encapsulation;
using DotnetBackendLab.SolidPatterns.Examples.Factory;
using DotnetBackendLab.SolidPatterns.Examples.InterfaceSegregation;
using DotnetBackendLab.SolidPatterns.Examples.LiskovSubstitution;
using DotnetBackendLab.SolidPatterns.Examples.OpenClosed;
using DotnetBackendLab.SolidPatterns.Examples.Polymorphism;
using DotnetBackendLab.SolidPatterns.Examples.SingleResponsibility;
using DotnetBackendLab.SolidPatterns.Examples.Singleton;
using DotnetBackendLab.SolidPatterns.Examples.Strategy;
using DotnetBackendLab.SolidPatterns.Examples.UnitOfWork;

namespace DotnetBackendLab.SolidPatterns;

/// <summary>Örnekleri kaydeder, menüyü basar ve seçilen örneği süresini ölçerek çalıştırır.</summary>
public sealed class ExampleRunner
{
    private readonly IReadOnlyList<IExample> _examples;

    public ExampleRunner()
    {
        _examples =
        [
            new Example01_Encapsulation(),
            new Example02_Polymorphism(),
            new Example03_CompositionOverInheritance(),
            new Example04_SingleResponsibility(),
            new Example05_OpenClosed(),
            new Example06_LiskovSubstitution(),
            new Example07_InterfaceSegregation(),
            new Example08_DependencyInversion(),
            new Example09_Singleton(),
            new Example10_Factory(),
            new Example11_Strategy(),
            new Example12_Decorator(),
            new Example13_RepositoryUnitOfWork(),
        ];
    }

    public bool Has(int number) => _examples.Any(e => e.Number == number);

    public void PrintMenu()
    {
        Console.WriteLine();
        Console.WriteLine("=== 02-SolidPatterns Örnekleri ===");
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

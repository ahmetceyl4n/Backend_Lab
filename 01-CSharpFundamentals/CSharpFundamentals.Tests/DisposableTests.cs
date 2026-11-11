using DotnetBackendLab.CSharpFundamentals.Examples;

namespace DotnetBackendLab.CSharpFundamentals.Tests;

public class DisposableTests
{
    // using bloğu bitince Dispose çağrılır.
    [Fact]
    public void UsingBlock_CallsDispose()
    {
        var log = new List<string>();
        LabelPrinter printer;

        using (printer = new LabelPrinter("P", log))
        {
            Assert.False(printer.IsDisposed);
        }

        Assert.True(printer.IsDisposed);
    }

    // using var: kapsam (metot) bitince Dispose çağrılır.
    [Fact]
    public void UsingVar_CallsDispose_WhenScopeEnds()
    {
        var log = new List<string>();
        var printer = UseWithUsingVar(log);

        Assert.True(printer.IsDisposed);
        Assert.Equal("P: Dispose -> bağlantı kapatıldı", log[^1]);
    }

    // Blok içinde exception olsa bile Dispose çağrılır (using = try/finally).
    [Fact]
    public void UsingBlock_CallsDispose_EvenWhenExceptionThrown()
    {
        var log = new List<string>();
        LabelPrinter? printer = null;

        void PrintAndJam()
        {
            using (printer = new LabelPrinter("P", log))
            {
                throw new InvalidOperationException("yazıcı sıkıştı");
            }
        }

        Assert.Throws<InvalidOperationException>(PrintAndJam);

        Assert.NotNull(printer);
        Assert.True(printer.IsDisposed);
    }

    private static LabelPrinter UseWithUsingVar(List<string> log)
    {
        using var printer = new LabelPrinter("P", log);
        printer.Print("TR1");
        return printer;
    }
}

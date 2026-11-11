using System.Text;
using DotnetBackendLab.ThreadsAsync;

// Türkçe karakterlerin konsolda düzgün görünmesi için.
Console.OutputEncoding = Encoding.UTF8;

var runner = new ExampleRunner();

// "dotnet run -- 3" gibi numarayla doğrudan çalıştırma.
if (args.Length > 0)
{
    if (int.TryParse(args[0], out var direct) && runner.Has(direct))
    {
        await runner.RunAsync(direct);
    }
    else
    {
        Console.WriteLine($"Geçersiz örnek numarası: '{args[0]}'");
        runner.PrintMenu();
    }
    return;
}

// Argümansız: menüyle etkileşimli çalışma.
while (true)
{
    runner.PrintMenu();
    Console.Write("Örnek numarası (çıkmak için boş Enter): ");
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
    {
        break;
    }

    if (int.TryParse(input, out var selection) && runner.Has(selection))
    {
        await runner.RunAsync(selection);
    }
    else
    {
        Console.WriteLine("Geçersiz seçim, tekrar deneyin.");
    }
}

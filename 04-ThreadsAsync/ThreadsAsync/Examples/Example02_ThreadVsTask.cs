namespace DotnetBackendLab.ThreadsAsync.Examples;

public sealed class Example02_ThreadVsTask : IExample
{
    public int Number => 2;
    public string Title => "ThreadVsTask: sonuç döndürme ve exception yakalama";

    public async Task RunAsync()
    {
        // Task ile: sonuç doğrudan döner, hata await noktasında yakalanır.
        Console.WriteLine("Task ile takip numarası üretimi:");
        try
        {
            string ok = await Task.Run(() => GenerateTracking(fail: false));
            Console.WriteLine($"   Sonuç: {ok}");

            string bad = await Task.Run(() => GenerateTracking(fail: true));
            Console.WriteLine($"   Sonuç: {bad}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"   Hata yakalandı (Task): {ex.Message}");
        }
        Console.WriteLine();

        // Thread ile: sonucu ve hatayı dışarı taşımak için elle alan tutmak gerekir.
        Console.WriteLine("Thread ile aynı iş (elle sonuç + hata taşıma):");
        string? result = null;
        Exception? error = null;
        var t = new Thread(() =>
        {
            // Burada yakalamazsan istisna thread'i çökertir, dışarıya ulaşamaz.
            try { result = GenerateTracking(fail: true); }
            catch (Exception ex) { error = ex; }
        });
        t.Start();
        t.Join();

        if (error is not null)
            Console.WriteLine($"   Hata yakalandı (Thread): {error.Message}");
        else
            Console.WriteLine($"   Sonuç: {result}");
    }

    private static string GenerateTracking(bool fail)
    {
        Thread.Sleep(30);
        if (fail) throw new InvalidOperationException("Şube servisi yanıt vermedi");
        return $"TR{Random.Shared.Next(100_000, 999_999)}";
    }
}

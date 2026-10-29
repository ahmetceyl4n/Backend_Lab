namespace DotnetBackendLab.ThreadsAsync.Examples;

public sealed class Example07_WhenAllWhenAny : IExample
{
    public int Number => 7;
    public string Title => "WhenAllWhenAny: WhenAll çoklu hata (InnerExceptions) ve WhenAny zaman aşımı";

    public async Task RunAsync()
    {
        // İki kargo işi de hata veriyor. await EDİLEN WhenAll yalnızca İLK hatayı fırlatır.
        var t1 = FailAsync("Şube-A kapalı", 50);
        var t2 = FailAsync("Şube-B dolu", 80);
        var all = Task.WhenAll(t1, t2);
        try
        {
            await all;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"await'in fırlattığı tek hata: {ex.Message}");
        }

        // Hataların tamamı Task'in Exception.InnerExceptions koleksiyonunda durur.
        Console.WriteLine($"Toplam hata sayısı (InnerExceptions): {all.Exception?.InnerExceptions.Count}");
        foreach (var inner in all.Exception!.InnerExceptions)
        {
            Console.WriteLine($"   - {inner.Message}");
        }
        Console.WriteLine();

        // WhenAny ile zaman aşımı: iş mi önce biter, zaman aşımı mı?
        var work = TrackAsync(500);
        var timeout = Task.Delay(200);
        var winner = await Task.WhenAny(work, timeout);
        if (winner == timeout)
            Console.WriteLine("Zaman aşımı! Takip sorgusu 200 ms içinde yanıt vermedi.");
        else
            Console.WriteLine($"Sonuç: {await work}");
    }

    private static async Task FailAsync(string message, int ms)
    {
        await Task.Delay(ms);
        throw new InvalidOperationException(message);
    }

    private static async Task<string> TrackAsync(int ms)
    {
        await Task.Delay(ms);
        return "Teslim edildi";
    }
}

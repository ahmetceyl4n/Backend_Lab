namespace DotnetBackendLab.ThreadsAsync.Examples;

public sealed class Example11_Cancellation : IExample
{
    public int Number => 11;
    public string Title => "Cancellation: CTS zaman aşımı, token geçişi, ThrowIfCancellationRequested";

    public async Task RunAsync()
    {
        // 300 ms sonra iptal. İşin tamamı 10 kargo x 100 ms = ~1000 ms sürerdi.
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(300));
        try
        {
            await ProcessShipmentsAsync(10, cts.Token);
            Console.WriteLine("Tüm kargolar işlendi.");
        }
        catch (OperationCanceledException)
        {
            // İptal normal bir akıştır; gerçek hatalardan ayrı ele alınır.
            Console.WriteLine("İşlem iptal edildi (zaman aşımı). Bu bir hata değil, beklenen durum.");
        }
    }

    // Token alt metoda geçirilir; her adımın başında iptal kontrol edilir.
    public static async Task ProcessShipmentsAsync(int count, CancellationToken token)
    {
        for (int i = 1; i <= count; i++)
        {
            token.ThrowIfCancellationRequested(); // iptal edilmişse burada OperationCanceledException fırlar
            await ScanAsync(token);
            Console.WriteLine($"   Kargo {i} işlendi (thread#{Environment.CurrentManagedThreadId})");
        }
    }

    private static async Task ScanAsync(CancellationToken token)
    {
        await Task.Delay(100, token); // Task.Delay de token'ı dinler, iptalde hemen fırlatır
    }
}

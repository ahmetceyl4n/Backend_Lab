using System.Runtime.CompilerServices;

namespace DotnetBackendLab.CSharpFundamentals.Examples;

public sealed class Example10_ExceptionHandling : IExample
{
    public int Number => 10;
    public string Title => "ExceptionHandling: try/catch/finally sırası, throw vs throw ex, when filtresi, özel exception";

    public Task RunAsync()
    {
        // 1) Çalışma sırası: try -> (hata) catch -> finally. finally her durumda çalışır.
        Console.WriteLine("1) Sıra:");
        try
        {
            Console.WriteLine("   try     : etiket basılıyor");
            throw new CarrierApiException(503, "Kargo API geçici olarak kapalı");
        }
        catch (CarrierApiException ex) when (ex.StatusCode == 404)   // filtre tutmaz -> bu blok atlanır
        {
            Console.WriteLine("   catch 404 (çalışmamalı)");
        }
        catch (CarrierApiException ex) when (ex.StatusCode == 503)   // filtre tutar -> buraya girer
        {
            Console.WriteLine($"   catch   : when (StatusCode == 503) yakaladı -> '{ex.Message}', tekrar denenecek");
        }
        finally
        {
            Console.WriteLine("   finally : yazıcı bağlantısı kapatıldı");
        }
        Console.WriteLine();

        // 2) throw vs throw ex: stack trace'te hatanın ÇIKTIĞI metot kalıyor mu?
        Console.WriteLine("2) Stack trace'in ilk satırı:");
        Console.WriteLine($"   throw    -> {FirstFrame(Capture(RethrowWithThrow))}");
        Console.WriteLine($"   throw ex -> {FirstFrame(Capture(RethrowWithThrowEx))}");
        Console.WriteLine("   'throw ex' stack trace'i catch satırından yeniden başlatır; asıl hatanın yeri kaybolur.");

        return Task.CompletedTask;
    }

    // Hatanın asıl çıktığı yer. NoInlining: Release'te derleyici bu metodu satır içine gömmesin.
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void CallCarrierApi() => throw new CarrierApiException(500, "Kargo API hata verdi");

    public static void RethrowWithThrow()
    {
        try { CallCarrierApi(); }
        catch (CarrierApiException) { throw; }       // orijinal stack trace korunur
    }

    public static void RethrowWithThrowEx()
    {
        try { CallCarrierApi(); }
#pragma warning disable CA2200 // Bilerek: farkı göstermek için 'throw ex' kullanılıyor.
        catch (CarrierApiException ex) { throw ex; } // stack trace burada sıfırlanır
#pragma warning restore CA2200
    }

    /// <summary>Verilen işi çalıştırır ve fırlattığı exception'ı döndürür.</summary>
    public static Exception Capture(Action action)
    {
        try { action(); }
        catch (Exception ex) { return ex; }
        throw new InvalidOperationException("Exception bekleniyordu");
    }

    private static string FirstFrame(Exception ex) =>
        ex.StackTrace?.Split(Environment.NewLine)[0].Trim().Split(" in ")[0] ?? "(yok)";
}

/// <summary>Özel exception: HTTP durum kodunu taşır, böylece 'when' ile filtrelenebilir.</summary>
public sealed class CarrierApiException(int statusCode, string message) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}

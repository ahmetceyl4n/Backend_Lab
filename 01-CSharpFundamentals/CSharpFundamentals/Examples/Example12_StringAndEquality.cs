using System.Diagnostics;
using System.Text;

namespace DotnetBackendLab.CSharpFundamentals.Examples;

public sealed class Example12_StringAndEquality : IExample
{
    public int Number => 12;
    public string Title => "StringAndEquality: string değişmezliği, string + vs StringBuilder, const vs readonly, nullable";

    // const: derleme zamanında sabit; kullanan her yere DEĞER olarak gömülür.
    public const decimal VatRate = 0.20m;
    // static readonly: çalışma zamanında bir kez atanır (ör. DateTime, new nesne olabilir).
    public static readonly DateTime StartedAt = DateTime.Now;
    // readonly örnek alanı: sadece tanımda veya constructor'da atanabilir.
    private readonly string _branch = "Kadıköy";

    public Task RunAsync()
    {
        // 1) string değişmezdir: "değiştiren" her işlem YENİ string üretir.
        string original = "tr100";
        string alias = original;
        original += "-A";
        string upper = alias.ToUpper();
        Console.WriteLine("1) Değişmezlik:");
        Console.WriteLine($"   original = {original}, alias = {alias}, upper = {upper}  -> alias hiç değişmedi");
        string built = new StringBuilder("tr").Append("100").ToString();
        Console.WriteLine($"   alias == built: {alias == built}, ReferenceEquals: {ReferenceEquals(alias, built)}  (== değeri karşılaştırır)");
        Console.WriteLine();

        // 2) Döngüde birleştirme: string + her adımda tüm metni kopyalar (O(n^2)); StringBuilder büyüyen tampon kullanır.
        const int n = 20_000;
        var sw = Stopwatch.StartNew();
        string manifest = "";
        for (int i = 0; i < n; i++) manifest += "TR" + i + ";";
        long concatMs = sw.ElapsedMilliseconds;

        sw.Restart();
        var sb = new StringBuilder();
        for (int i = 0; i < n; i++) sb.Append("TR").Append(i).Append(';');
        string manifest2 = sb.ToString();
        long sbMs = sw.ElapsedMilliseconds;
        Console.WriteLine($"2) {n:N0} takip no birleştirme: string + = {concatMs} ms, StringBuilder = {sbMs} ms (aynı sonuç: {manifest == manifest2})");
        Console.WriteLine();

        // 3) const vs readonly
        Console.WriteLine("3) const vs readonly:");
        Console.WriteLine($"   const VatRate = {VatRate} (derleme zamanı; başka assembly kullanıyorsa değer oraya kopyalanır, değişince o da yeniden derlenmeli)");
        Console.WriteLine($"   static readonly StartedAt = {StartedAt:HH:mm:ss} (çalışma zamanında hesaplandı; const olamazdı)");
        Console.WriteLine($"   readonly _branch = {_branch} (constructor dışında atanamaz: CS0191)");
        Console.WriteLine();

        // 4) Nullable reference types: derleyici null olabilecek değeri takip eder.
        string? note = FindDeliveryNote("TR100");      // string? -> null olabilir
        // Console.WriteLine(note.Length);              // açarsan CS8602: null olabilecek referansı kullanma uyarısı
        Console.WriteLine("4) Nullable:");
        Console.WriteLine($"   note?.Length          = {note?.Length.ToString() ?? "null"}  (?. null ise durur)");
        Console.WriteLine($"   note ?? \"(not yok)\"   = {note ?? "(not yok)"}");
        string? surely = FindDeliveryNote("TR200");
        Console.WriteLine($"   surely!.Length        = {surely!.Length}  (! 'eminim null değil' der; yanılırsan NullReferenceException)");

        return Task.CompletedTask;
    }

    private static string? FindDeliveryNote(string trackingNo) => trackingNo == "TR200" ? "Kapıya bırakın" : null;
}

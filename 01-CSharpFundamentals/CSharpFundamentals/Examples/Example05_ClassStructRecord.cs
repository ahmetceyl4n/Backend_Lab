namespace DotnetBackendLab.CSharpFundamentals.Examples;

public sealed class Example05_ClassStructRecord : IExample
{
    public int Number => 5;
    public string Title => "ClassStructRecord: == / Equals davranışı, record'da with ve değişmezlik";

    public Task RunAsync()
    {
        // Üçünde de aynı veri: TR100, 5 kg.
        var c1 = new LabelClass("TR100", 5);
        var c2 = new LabelClass("TR100", 5);
        var s1 = new LabelStruct("TR100", 5);
        var s2 = new LabelStruct("TR100", 5);
        var r1 = new LabelRecord("TR100", 5);
        var r2 = new LabelRecord("TR100", 5);

        Console.WriteLine("Aynı verili iki nesne karşılaştırılıyor:");
        Console.WriteLine($"   class : ==  {c1 == c2,-5}  Equals {c1.Equals(c2),-5}  (referans karşılaştırır: iki ayrı nesne)");
        Console.WriteLine($"   struct: ==  ---    Equals {s1.Equals(s2),-5}  (alanları karşılaştırır; == tanımlı değil, yazarsan CS0019)");
        Console.WriteLine($"   record: ==  {r1 == r2,-5}  Equals {r1.Equals(r2),-5}  (derleyici değer eşitliği üretir)");
        Console.WriteLine($"   record ReferenceEquals(r1, r2) = {ReferenceEquals(r1, r2)}  (yine de iki ayrı nesne)");
        Console.WriteLine();

        // with: kopyasını alıp bazı alanları değiştirir. Orijinal değişmez.
        var r3 = r1 with { Weight = 8 };
        Console.WriteLine("var r3 = r1 with { Weight = 8 };");
        Console.WriteLine($"   r1 = {r1}");
        Console.WriteLine($"   r3 = {r3}");
        Console.WriteLine($"   r1 == r3 : {r1 == r3}  |  ReferenceEquals: {ReferenceEquals(r1, r3)}");
        Console.WriteLine();

        // Değişmezlik: positional record'un özellikleri 'init' -> oluşturulduktan sonra atanamaz.
        // r1.Weight = 10;   // açarsan CS8852: init-only özellik sadece nesne oluşturulurken atanabilir
        Console.WriteLine("r1.Weight = 10 derlenmez (init-only). Değiştirmek için 'with' ile yeni nesne üretilir.");
        Console.WriteLine($"ToString de hazır: {r1}");

        return Task.CompletedTask;
    }
}

public sealed class LabelClass(string trackingNo, int weight)
{
    public string TrackingNo { get; } = trackingNo;
    public int Weight { get; } = weight;
}

public readonly struct LabelStruct(string trackingNo, int weight)
{
    public string TrackingNo { get; } = trackingNo;
    public int Weight { get; } = weight;
}

/// <summary>Değer eşitliği, ToString ve with desteği derleyici tarafından üretilir.</summary>
public sealed record LabelRecord(string TrackingNo, int Weight);

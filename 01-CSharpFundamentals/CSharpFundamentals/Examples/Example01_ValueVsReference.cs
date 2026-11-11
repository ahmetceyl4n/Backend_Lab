namespace DotnetBackendLab.CSharpFundamentals.Examples;

public sealed class Example01_ValueVsReference : IExample
{
    public int Number => 1;
    public string Title => "ValueVsReference: int ataması kopyalar, class ataması aynı nesneyi gösterir";

    public Task RunAsync()
    {
        // 1) Değer tipi (int): atama DEĞERİ kopyalar. İki ayrı kutu olur.
        int weightA = 5;
        int weightB = weightA;
        weightB = 12;
        Console.WriteLine("1) int (değer tipi):");
        Console.WriteLine($"   weightA = {weightA}, weightB = {weightB}  -> weightB değişti, weightA etkilenmedi");
        Console.WriteLine();

        // 2) Referans tipi (class): atama nesneyi değil, nesnenin ADRESİNİ kopyalar.
        var parcelA = new Parcel("TR100", 5);
        var parcelB = parcelA;          // aynı nesneyi gösteren ikinci değişken
        parcelB.Weight = 12;
        Console.WriteLine("2) class (referans tipi):");
        Console.WriteLine($"   parcelA.Weight = {parcelA.Weight}, parcelB.Weight = {parcelB.Weight}  -> ikisi de değişti");
        Console.WriteLine($"   ReferenceEquals(parcelA, parcelB) = {ReferenceEquals(parcelA, parcelB)}  (tek nesne, iki değişken)");
        Console.WriteLine();

        // 3) Değişkene yeni nesne atamak sadece O değişkeni başka yere yönlendirir.
        parcelB = new Parcel("TR200", 1);
        Console.WriteLine("3) parcelB = new Parcel(...) sonrası:");
        Console.WriteLine($"   parcelA = {parcelA}, parcelB = {parcelB}  -> artık farklı nesneler");
        Console.WriteLine($"   ReferenceEquals(parcelA, parcelB) = {ReferenceEquals(parcelA, parcelB)}");

        return Task.CompletedTask;
    }
}

/// <summary>Örneklerde ve testlerde kullanılan basit kargo paketi (referans tipi).</summary>
public sealed class Parcel(string trackingNo, int weight)
{
    public string TrackingNo { get; set; } = trackingNo;
    public int Weight { get; set; } = weight;

    public override string ToString() => $"{TrackingNo}({Weight} kg)";
}

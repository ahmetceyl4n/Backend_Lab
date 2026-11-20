namespace DotnetBackendLab.CSharpFundamentals.Examples;

public sealed class Example14_HashCodeEquality : IExample
{
    public int Number => 14;
    public string Title => "HashCodeEquality: Equals var GetHashCode yok, doğru hali, eklendikten sonra değişen anahtar";

    public Task RunAsync()
    {
        // 1) Sadece Equals override edildi: "eşit" iki kargo farklı kovalara düşer.
        var badSet = new HashSet<ShipmentEqualsOnly>
        {
            new("TR100"),
            new("TR100"),
        };
        var b1 = new ShipmentEqualsOnly("TR100");
        var b2 = new ShipmentEqualsOnly("TR100");
        Console.WriteLine("1) Sadece Equals override edilmiş:");
        Console.WriteLine($"   b1.Equals(b2) = {b1.Equals(b2)}, hash eşit mi? {b1.GetHashCode() == b2.GetHashCode()}");
        Console.WriteLine($"   Aynı takip no iki kez eklendi -> Count = {badSet.Count}  (HashSet tekrarı yakalayamadı)");
        Console.WriteLine();

        // 2) Equals + GetHashCode birlikte: aynı kova, Equals ile eşit bulunur.
        var goodSet = new HashSet<ShipmentWithHash>();
        bool first = goodSet.Add(new ShipmentWithHash("TR100"));
        bool second = goodSet.Add(new ShipmentWithHash("TR100"));
        Console.WriteLine("2) Equals + GetHashCode override edilmiş:");
        Console.WriteLine($"   1. Add = {first}, 2. Add = {second} -> Count = {goodSet.Count}");
        Console.WriteLine();

        // 3) Tuzak: eklendikten sonra hash'i belirleyen alan değişirse nesne eski kovada kalır.
        var parcel = new MutableKeyShipment { TrackingNo = "TR200" };
        var set = new HashSet<MutableKeyShipment> { parcel };
        parcel.TrackingNo = "TR201";   // hash değişti, ama nesne hâlâ "TR200" kovasında
        Console.WriteLine("3) Eklendikten sonra TrackingNo değişti:");
        Console.WriteLine($"   set.Count = {set.Count}, set.Contains(parcel) = {set.Contains(parcel)}  (aynı nesne, bulunamıyor!)");
        Console.WriteLine($"   tek tek gezince var mı? {set.Any(x => ReferenceEquals(x, parcel))}  (nesne sette duruyor ama yanlış kovada)");
        Console.WriteLine("   Çözüm: anahtar alanlar değişmez olmalı (init / readonly), ör. ShipmentWithHash.");

        return Task.CompletedTask;
    }
}

// Kötü örnek: Equals var, GetHashCode yok -> hash hâlâ referansa göre üretilir.
#pragma warning disable CS0659 // Bilerek: GetHashCode'u override etmemenin sonucunu göstermek için.
public sealed class ShipmentEqualsOnly(string trackingNo)
{
    public string TrackingNo { get; } = trackingNo;
    public override bool Equals(object? obj) => obj is ShipmentEqualsOnly s && s.TrackingNo == TrackingNo;
}
#pragma warning restore CS0659

// Doğru örnek: eşit nesnelerin hash'i de eşit. Anahtar alan değişmez (get-only).
public sealed class ShipmentWithHash(string trackingNo)
{
    public string TrackingNo { get; } = trackingNo;
    public override bool Equals(object? obj) => obj is ShipmentWithHash s && s.TrackingNo == TrackingNo;
    public override int GetHashCode() => TrackingNo.GetHashCode();
}

// Tuzak örneği: hash'i belirleyen alan sonradan değiştirilebiliyor.
public sealed class MutableKeyShipment
{
    public string TrackingNo { get; set; } = "";
    public override bool Equals(object? obj) => obj is MutableKeyShipment s && s.TrackingNo == TrackingNo;

    // string.GetHashCode her çalıştırmada farklı değer üretir (güvenlik için rastgeledir).
    // Gösterimin her seferinde aynı sonucu vermesi için burada sabit bir formül kullanılıyor.
    public override int GetHashCode() => TrackingNo.Aggregate(17, (h, c) => h * 31 + c);
}

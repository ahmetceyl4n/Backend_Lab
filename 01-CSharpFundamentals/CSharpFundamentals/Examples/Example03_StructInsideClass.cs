namespace DotnetBackendLab.CSharpFundamentals.Examples;

public sealed class Example03_StructInsideClass : IExample
{
    public int Number => 3;
    public string Title => "StructInsideClass: class içindeki struct alanı; class ataması vs struct kopyası";

    public Task RunAsync()
    {
        var shipmentA = new Shipment { TrackingNo = "TR100" };
        shipmentA.Destination = new Address("İstanbul", "34000");
        Console.WriteLine($"Başlangıç            : shipmentA -> {shipmentA.Destination}");
        Console.WriteLine();

        // 1) class'ı başka değişkene atamak: aynı nesne. İçindeki struct da tek bir yerde durur.
        var shipmentB = shipmentA;
        shipmentB.Destination.City = "Ankara";
        Console.WriteLine("1) shipmentB = shipmentA; shipmentB.Destination.City = \"Ankara\"");
        Console.WriteLine($"   shipmentA -> {shipmentA.Destination}  (yansıdı: aynı Shipment nesnesi)");
        Console.WriteLine($"   shipmentB -> {shipmentB.Destination}");
        Console.WriteLine();

        // 2) struct'ı dışarı almak: TÜM struct kopyalanır. Kopya bağımsızdır.
        var copy = shipmentA.Destination;
        copy.City = "İzmir";
        Console.WriteLine("2) var copy = shipmentA.Destination; copy.City = \"İzmir\"");
        Console.WriteLine($"   copy      -> {copy}");
        Console.WriteLine($"   shipmentA -> {shipmentA.Destination}  (yansımadı: struct kopyalandı)");
        Console.WriteLine();

        // 3) Kopyayı geri yazınca değişiklik nesneye geçer.
        shipmentA.Destination = copy;
        Console.WriteLine("3) shipmentA.Destination = copy;");
        Console.WriteLine($"   shipmentA -> {shipmentA.Destination}, shipmentB -> {shipmentB.Destination}  (ikisi aynı nesne)");

        // Not: Destination alan değil PROPERTY olsaydı, 'shipmentA.Destination.City = ...' derlenmezdi (CS1612):
        // property getter bir kopya döndürür, kopyayı değiştirmek anlamsız olduğu için derleyici engeller.
        return Task.CompletedTask;
    }

    /// <summary>Struct'ı kopyalayıp değiştirir, orijinali ve kopyayı döndürür (test için).</summary>
    public static (Address Original, Address Copy) CopyAndModify(Shipment shipment, string newCity)
    {
        var copy = shipment.Destination;
        copy.City = newCity;
        return (shipment.Destination, copy);
    }
}

/// <summary>Değer tipi adres. Atandığında tamamı kopyalanır.</summary>
public struct Address(string city, string postalCode)
{
    public string City = city;
    public string PostalCode = postalCode;

    public override readonly string ToString() => $"{City}/{PostalCode}";
}

/// <summary>Referans tipi gönderi; Destination bir ALAN (field), property değil.</summary>
public sealed class Shipment
{
    public string TrackingNo = "";
    public Address Destination;
}

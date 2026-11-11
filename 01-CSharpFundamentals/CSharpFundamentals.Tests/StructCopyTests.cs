using DotnetBackendLab.CSharpFundamentals.Examples;

namespace DotnetBackendLab.CSharpFundamentals.Tests;

public class StructCopyTests
{
    // class ataması aynı nesneyi gösterir: birinden yapılan değişiklik diğerinde görünür.
    [Fact]
    public void ClassAssignment_SharesSameObject()
    {
        var a = new Parcel("TR100", 5);
        var b = a;
        b.Weight = 12;

        Assert.Same(a, b);
        Assert.Equal(12, a.Weight);
    }

    // class içindeki struct alanı dışarı alınınca kopyalanır; kopyayı değiştirmek orijinali etkilemez.
    [Fact]
    public void StructCopy_DoesNotAffectOriginal()
    {
        var shipment = new Shipment { Destination = new Address("İstanbul", "34000") };

        var (original, copy) = Example03_StructInsideClass.CopyAndModify(shipment, "İzmir");

        Assert.Equal("İzmir", copy.City);
        Assert.Equal("İstanbul", original.City);
        Assert.Equal("İstanbul", shipment.Destination.City);
    }

    // Struct alanı doğrudan nesne üzerinden değiştirilirse (field olduğu için) asıl değer değişir.
    [Fact]
    public void StructFieldModifiedInPlace_ChangesOwner()
    {
        var shipment = new Shipment { Destination = new Address("İstanbul", "34000") };
        var sameShipment = shipment;

        sameShipment.Destination.City = "Ankara";

        Assert.Equal("Ankara", shipment.Destination.City);
    }
}

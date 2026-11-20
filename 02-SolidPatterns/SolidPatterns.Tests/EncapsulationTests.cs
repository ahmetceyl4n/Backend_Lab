using DotnetBackendLab.SolidPatterns.Examples.Encapsulation;

namespace DotnetBackendLab.SolidPatterns.Tests;

public class EncapsulationTests
{
    // Yola çıkmamış kargo teslim edilemez.
    [Fact]
    public void Deliver_BeforeDispatch_Throws()
    {
        var shipment = new Shipment("TR1");

        Assert.Throws<InvalidOperationException>(shipment.Deliver);
        Assert.Equal(ShipmentStatus.Preparing, shipment.Status);
    }

    // Teslim edilmiş kargo tekrar yola çıkarılamaz.
    [Fact]
    public void Dispatch_AfterDelivered_Throws()
    {
        var shipment = new Shipment("TR1");
        shipment.Dispatch();
        shipment.Deliver();

        Assert.Throws<InvalidOperationException>(shipment.Dispatch);
    }

    // Geçerli sırayla durum ilerler ve teslim tarihi durumla birlikte atanır.
    [Fact]
    public void ValidTransitions_SetStatusAndDeliveredAt()
    {
        var shipment = new Shipment("TR1");

        shipment.Dispatch();
        Assert.Equal(ShipmentStatus.InTransit, shipment.Status);
        Assert.Null(shipment.DeliveredAt);

        shipment.Deliver();
        Assert.Equal(ShipmentStatus.Delivered, shipment.Status);
        Assert.NotNull(shipment.DeliveredAt);
    }
}

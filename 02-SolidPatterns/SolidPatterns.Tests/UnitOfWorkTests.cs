using DotnetBackendLab.SolidPatterns.Examples.UnitOfWork;

namespace DotnetBackendLab.SolidPatterns.Tests;

public class UnitOfWorkTests
{
    // Bekleyen kayıtlardan biri geçersizse SaveChanges hata verir ve HİÇBİR kayıt veritabanına yazılmaz.
    [Fact]
    public void SaveChanges_WhenOneItemInvalid_PersistsNothing()
    {
        var db = new Database();
        var uow = new UnitOfWork(db);
        uow.Shipments.Add(new Shipment("TR1", CourierId: 1));
        uow.Couriers.Add(new Courier(1, Name: ""));

        Assert.Throws<InvalidOperationException>(uow.SaveChanges);

        Assert.Empty(db.Shipments);
        Assert.Empty(db.Couriers);
    }

    // Hepsi geçerliyse iki repository'nin kayıtları tek SaveChanges ile birlikte yazılır.
    [Fact]
    public void SaveChanges_WhenAllValid_PersistsBoth()
    {
        var db = new Database();
        var uow = new UnitOfWork(db);
        uow.Shipments.Add(new Shipment("TR1", CourierId: 1));
        uow.Couriers.Add(new Courier(1, Name: "Ayşe"));

        uow.SaveChanges();

        Assert.Single(db.Shipments);
        Assert.Single(db.Couriers);
    }

    // SaveChanges çağrılmadan hiçbir şey yazılmaz; başarısız denemeden kalan kayıtlar sonrakine sızmaz.
    [Fact]
    public void FailedSave_DoesNotLeakIntoNextSave()
    {
        var db = new Database();
        var uow = new UnitOfWork(db);
        uow.Couriers.Add(new Courier(1, Name: ""));
        Assert.Empty(db.Couriers);
        Assert.Throws<InvalidOperationException>(uow.SaveChanges);

        uow.Couriers.Add(new Courier(2, Name: "Mehmet"));
        uow.SaveChanges();

        Assert.Equal([new Courier(2, "Mehmet")], db.Couriers);
    }
}

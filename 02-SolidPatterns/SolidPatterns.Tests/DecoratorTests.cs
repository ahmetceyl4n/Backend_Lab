using DotnetBackendLab.SolidPatterns.Examples.Decorator;

namespace DotnetBackendLab.SolidPatterns.Tests;

public class DecoratorTests
{
    // Logging decorator çağrıyı iç repository'ye iletir ve sonucu değiştirmeden döndürür.
    [Fact]
    public void LoggingDecorator_CallsInnerRepository_AndLogs()
    {
        var inner = new InMemoryShipmentRepository();
        var log = new List<string>();
        var repository = new LoggingShipmentRepository(inner, log.Add);

        var result = repository.GetByTrackingNo("TR100");

        Assert.Equal(1, inner.CallCount);
        Assert.Equal("TR100", result?.TrackingNo);
        Assert.Equal(2, log.Count);
    }

    // Cache decorator aynı anahtar için iç repository'yi yalnızca bir kez çağırır.
    [Fact]
    public void CachingDecorator_CallsInnerOnce_ForRepeatedReads()
    {
        var inner = new InMemoryShipmentRepository();
        var repository = new CachingShipmentRepository(inner);

        var first = repository.GetByTrackingNo("TR100");
        var second = repository.GetByTrackingNo("TR100");

        Assert.Equal(1, inner.CallCount);
        Assert.Same(first, second);
    }

    // Bulunamayan sonuç da cache'lenir; tekrar tekrar veritabanına gidilmez.
    [Fact]
    public void CachingDecorator_CachesMissingResults()
    {
        var inner = new InMemoryShipmentRepository();
        var repository = new CachingShipmentRepository(inner);

        Assert.Null(repository.GetByTrackingNo("YOK"));
        Assert.Null(repository.GetByTrackingNo("YOK"));
        Assert.Equal(1, inner.CallCount);
    }
}

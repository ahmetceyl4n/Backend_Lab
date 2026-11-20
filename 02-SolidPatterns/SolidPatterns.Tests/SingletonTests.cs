using DotnetBackendLab.SolidPatterns.Examples.Singleton;

namespace DotnetBackendLab.SolidPatterns.Tests;

public class SingletonTests
{
    // Lazy<T> singleton: aynı anda başlayan thread'lerin hepsi tek örneği alır, constructor bir kez çalışır.
    [Fact]
    public void LazySingleton_ProducesSingleInstance_UnderParallelAccess()
    {
        int distinct = SingletonProbe.Run(() => LazyTrackingNoGenerator.Instance, threadCount: 16);

        Assert.Equal(1, distinct);
        Assert.Equal(1, LazyTrackingNoGenerator.CreatedCount);
    }

    // lock'lu singleton: aynı garanti.
    [Fact]
    public void LockedSingleton_ProducesSingleInstance_UnderParallelAccess()
    {
        int distinct = SingletonProbe.Run(() => LockedTrackingNoGenerator.Instance, threadCount: 16);

        Assert.Equal(1, distinct);
        Assert.Equal(1, LockedTrackingNoGenerator.CreatedCount);
    }

    // Not: korumasız singleton test edilmez; kaç örnek oluşacağı zamanlamaya bağlı (kararsız test).
}

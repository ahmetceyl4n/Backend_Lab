using DotnetBackendLab.ThreadsAsync.Examples;

namespace DotnetBackendLab.ThreadsAsync.Tests;

public class SemaphoreConcurrencyTests
{
    // SemaphoreSlim(10) ile aynı anda en fazla 10 istek işlenir, 10'u asla aşmaz.
    [Fact]
    public async Task Semaphore_LimitsMaxConcurrencyTo10()
    {
        int maxObserved = await Example06_LimitedConcurrency.RunWithSemaphoreAsync(total: 1000, limit: 10);

        Assert.True(maxObserved <= 10, $"Eşzamanlı istek sayısı 10'u aştı: {maxObserved}");
    }
}

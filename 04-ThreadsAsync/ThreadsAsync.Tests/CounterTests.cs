using DotnetBackendLab.ThreadsAsync.Examples;

namespace DotnetBackendLab.ThreadsAsync.Tests;

public class CounterTests
{
    // lock'lu sayaç her koşuda tam beklenen değeri verir (deterministik).
    [Fact]
    public void LockCounter_AlwaysEqualsExpected()
    {
        const int threads = 8;
        const int perThread = 50_000;

        int result = Example08_RaceCondition.RunWithLock(threads, perThread);

        Assert.Equal(threads * perThread, result);
    }

    // Interlocked'lı sayaç da her koşuda tam beklenen değeri verir.
    [Fact]
    public void InterlockedCounter_AlwaysEqualsExpected()
    {
        const int threads = 8;
        const int perThread = 50_000;

        int result = Example08_RaceCondition.RunWithInterlocked(threads, perThread);

        Assert.Equal(threads * perThread, result);
    }
}

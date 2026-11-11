using DotnetBackendLab.CSharpFundamentals.Examples;

namespace DotnetBackendLab.CSharpFundamentals.Tests;

public class LinqDeferredTests
{
    // Sorguyu tanımlamak filtreyi çalıştırmaz.
    [Fact]
    public void Query_DoesNotRun_UntilEnumerated()
    {
        var counter = new CallCounter();
        _ = Example09_LinqDeferred.BuildHeavyQuery([3, 15, 8, 22], counter);

        Assert.Equal(0, counter.Count);
    }

    // Her enumerate filtreyi baştan çalıştırır.
    [Fact]
    public void Query_RunsAgain_OnEachEnumeration()
    {
        var counter = new CallCounter();
        var query = Example09_LinqDeferred.BuildHeavyQuery([3, 15, 8, 22], counter);

        _ = query.Count();
        Assert.Equal(4, counter.Count);

        _ = query.Count();
        Assert.Equal(8, counter.Count);
    }

    // Tanımdan sonra kaynağa eklenen eleman sorgu sonucuna girer.
    [Fact]
    public void Query_SeesItemsAddedAfterDefinition()
    {
        var weights = new List<int> { 3, 15 };
        var query = Example09_LinqDeferred.BuildHeavyQuery(weights, new CallCounter());

        weights.Add(30);

        Assert.Equal([15, 30], query);
    }

    // ToList sorguyu bir kez çalıştırır; sonraki değişiklikleri görmez.
    [Fact]
    public void ToList_RunsOnce_AndFreezesResult()
    {
        var weights = new List<int> { 3, 15 };
        var counter = new CallCounter();
        var snapshot = Example09_LinqDeferred.BuildHeavyQuery(weights, counter).ToList();

        weights.Add(30);
        _ = snapshot.Count;
        _ = snapshot.Count;

        Assert.Equal([15], snapshot);
        Assert.Equal(2, counter.Count);
    }

    // yield'li metot çağrılınca gövdesi çalışmaz; sadece istenen kadar eleman üretilir.
    [Fact]
    public void YieldIterator_ProducesItemsLazily()
    {
        var log = new List<string>();
        var events = Example09_LinqDeferred.ReadScanEvents(log);
        Assert.Empty(log);

        _ = events.Take(2).ToList();

        Assert.Equal(2, log.Count);
    }
}

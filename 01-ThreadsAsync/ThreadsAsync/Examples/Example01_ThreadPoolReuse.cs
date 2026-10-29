namespace DotnetBackendLab.ThreadsAsync.Examples;

public sealed class Example01_ThreadPoolReuse : IExample
{
    public int Number => 1;
    public string Title => "ThreadPoolReuse: new Thread vs ThreadPool (thread tekrar kullanımı)";

    public async Task RunAsync()
    {
        const int count = 5;
        Console.WriteLine($"Ana thread#: {Environment.CurrentManagedThreadId}");
        Console.WriteLine();

        // 1) Her kargo için 'new Thread': her biri ayrı bir thread -> numaralar hep farklı.
        Console.WriteLine("1) new Thread — her kargo için yeni thread:");
        var newThreadIds = new int[count];
        var threads = new List<Thread>();
        for (int i = 0; i < count; i++)
        {
            int index = i;
            var t = new Thread(() =>
            {
                newThreadIds[index] = Environment.CurrentManagedThreadId;
                Thread.Sleep(50); // kargo işleniyor
            });
            threads.Add(t);
            t.Start();
        }
        foreach (var t in threads) t.Join();
        Console.WriteLine($"   Thread#: {string.Join(", ", newThreadIds)}");
        Console.WriteLine($"   Farklı thread sayısı: {newThreadIds.Distinct().Count()} (hepsi farklı)");
        Console.WriteLine();

        // 2) ThreadPool: işler sırayla gelince havuz aynı thread'i boşaltıp tekrar kullanır.
        Console.WriteLine("2) ThreadPool — işler sırayla, havuz thread'leri tekrar kullanır:");
        var poolIds = new List<int>();
        for (int round = 0; round < count; round++)
        {
            using var done = new ManualResetEventSlim();
            ThreadPool.QueueUserWorkItem(_ =>
            {
                lock (poolIds) poolIds.Add(Environment.CurrentManagedThreadId);
                Thread.Sleep(50);
                done.Set();
            });
            done.Wait(); // sıradaki işi başlatmadan önce bitmesini bekle -> thread serbest kalır
        }
        Console.WriteLine($"   Thread#: {string.Join(", ", poolIds)}");
        Console.WriteLine($"   Farklı thread sayısı: {poolIds.Distinct().Count()} (tekrar kullanıldı)");

        await Task.CompletedTask;
    }
}

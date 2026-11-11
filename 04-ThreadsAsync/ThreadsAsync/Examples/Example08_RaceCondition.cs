namespace DotnetBackendLab.ThreadsAsync.Examples;

public sealed class Example08_RaceCondition : IExample
{
    public int Number => 8;
    public string Title => "RaceCondition: sayaç++ kayıp güncelleme, lock ve Interlocked çözümü";

    public async Task RunAsync()
    {
        const int threads = 8;
        const int perThread = 100_000;
        int expected = threads * perThread;

        Console.WriteLine($"{threads} thread, her biri {perThread:N0} paket tarıyor. Beklenen toplam: {expected:N0}");
        Console.WriteLine();

        Console.WriteLine($"Korumasız (counter++): {RunUnsafe(threads, perThread),10:N0}  (güncellemeler kaybolabilir)");
        Console.WriteLine($"lock ile             : {RunWithLock(threads, perThread),10:N0}");
        Console.WriteLine($"Interlocked ile      : {RunWithInterlocked(threads, perThread),10:N0}");

        await Task.CompletedTask;
    }

    // counter++ aslında üç adım: oku, artır, yaz. Araya başka thread girince artışlar kaybolur.
    public static int RunUnsafe(int threads, int perThread)
    {
        int counter = 0;
        Parallel.For(0, threads, _ =>
        {
            for (int i = 0; i < perThread; i++) counter++;
        });
        return counter;
    }

    // lock: artırma bölgesine aynı anda tek thread girer -> hiçbir güncelleme kaybolmaz.
    public static int RunWithLock(int threads, int perThread)
    {
        int counter = 0;
        var gate = new object();
        Parallel.For(0, threads, _ =>
        {
            for (int i = 0; i < perThread; i++)
                lock (gate) counter++;
        });
        return counter;
    }

    // Interlocked.Increment: tek değişkenin artırılmasını tek atomik işlem yapar.
    public static int RunWithInterlocked(int threads, int perThread)
    {
        int counter = 0;
        Parallel.For(0, threads, _ =>
        {
            for (int i = 0; i < perThread; i++) Interlocked.Increment(ref counter);
        });
        return counter;
    }
}

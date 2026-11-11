namespace DotnetBackendLab.ThreadsAsync.Examples;

public sealed class Example10_LockOrderDeadlock : IExample
{
    public int Number => 10;
    public string Title => "LockOrderDeadlock: ters kilit sırası deadlock, TryEnter ile tespit";

    private readonly object _branchA = new();
    private readonly object _branchB = new();

    public async Task RunAsync()
    {
        Console.WriteLine("İki şube kilidi var. Transferde kilitler hangi sırada alınıyor önemli.");
        Console.WriteLine();

        // Ters sıra: bir thread A->B, diğeri B->A alır. Klasik deadlock kurulumu.
        bool deadlock = await TryTransferAsync(reverse: true);
        Console.WriteLine(deadlock
            ? "Ters sıra : KİLİTLENME tespit edildi (TryEnter zaman aşımına uğradı), program donmadı."
            : "Ters sıra : bu sefer şanslıydık (nadir).");
        Console.WriteLine();

        // Aynı sıra: iki thread de A->B alır -> deadlock oluşamaz.
        bool sameOrderDeadlock = await TryTransferAsync(reverse: false);
        Console.WriteLine(sameOrderDeadlock
            ? "Aynı sıra : beklenmedik kilitlenme."
            : "Aynı sıra : sorun yok, iki transfer de tamamlandı.");
    }

    private async Task<bool> TryTransferAsync(bool reverse)
    {
        int failures = 0;

        var t1 = Task.Run(() =>
        {
            if (!AcquireBoth(_branchA, _branchB)) Interlocked.Increment(ref failures);
        });
        var t2 = Task.Run(() =>
        {
            var (first, second) = reverse ? (_branchB, _branchA) : (_branchA, _branchB);
            if (!AcquireBoth(first, second)) Interlocked.Increment(ref failures);
        });

        await Task.WhenAll(t1, t2);
        return failures > 0;
    }

    // Monitor.TryEnter + zaman aşımı: kilit alınamazsa sonsuza kadar beklemek yerine vazgeçer.
    private static bool AcquireBoth(object first, object second)
    {
        if (!Monitor.TryEnter(first, TimeSpan.FromMilliseconds(1000))) return false;
        try
        {
            Thread.Sleep(100); // diğer thread'in ilk kilidini almasına fırsat ver (çakışmayı tetikle)
            if (!Monitor.TryEnter(second, TimeSpan.FromMilliseconds(1000))) return false;
            try
            {
                Thread.Sleep(20); // transfer işi
                return true;
            }
            finally { Monitor.Exit(second); }
        }
        finally { Monitor.Exit(first); }
    }
}

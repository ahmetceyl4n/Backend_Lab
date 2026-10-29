namespace DotnetBackendLab.ThreadsAsync.Examples;

public sealed class Example09_BankAccount : IExample
{
    public int Number => 9;
    public string Title => "BankAccount: ParaCek check-then-act — korumasız / Interlocked / lock";

    public async Task RunAsync()
    {
        Console.WriteLine("Hesapta 100 TL var; 200 kurye aynı anda 1 TL çekmeye çalışıyor.");
        Console.WriteLine("Doğru davranış: en fazla 100 başarılı çekim, bakiye asla < 0.");
        Console.WriteLine();

        Console.WriteLine($"Korumasız  : {Simulate(new UnsafeBankAccount(100))}");
        Console.WriteLine($"Interlocked: {Simulate(new InterlockedBankAccount(100))}  (hâlâ check-then-act hatalı)");
        Console.WriteLine($"lock       : {Simulate(new LockedBankAccount(100))}  (doğru)");

        await Task.CompletedTask;
    }

    private static string Simulate(IBankAccount account)
    {
        int success = 0;
        Parallel.For(0, 200, _ =>
        {
            if (account.Withdraw(1)) Interlocked.Increment(ref success);
        });
        return $"başarılı çekim={success,3}, kalan bakiye={account.Balance,5}";
    }
}

public interface IBankAccount
{
    decimal Balance { get; }
    bool Withdraw(decimal amount);
}

// Korumasız: kontrol ve çekme iki ayrı adım. Araya başka thread girer -> bakiye eksiye düşer.
public sealed class UnsafeBankAccount(decimal initial) : IBankAccount
{
    private decimal _balance = initial;
    public decimal Balance => _balance;

    public bool Withdraw(decimal amount)
    {
        if (_balance >= amount)   // kontrol
        {
            Thread.SpinWait(200); // yarış penceresini genişlet
            _balance -= amount;   // eylem
            return true;
        }
        return false;
    }
}

// Interlocked tek bir değişkeni atomik yapar ama 'önce kontrol, sonra çek' bütününü korumaz.
public sealed class InterlockedBankAccount(decimal initial) : IBankAccount
{
    private long _cents = (long)(initial * 100);
    public decimal Balance => _cents / 100m;

    public bool Withdraw(decimal amount)
    {
        long cents = (long)(amount * 100);
        if (Interlocked.Read(ref _cents) >= cents)  // kontrol (atomik ama ayrı adım)
        {
            Thread.SpinWait(200);
            Interlocked.Add(ref _cents, -cents);     // eylem (atomik ama ayrı adım)
            return true;
        }
        return false;
    }
}

// lock: kontrol + eylem tek kritik bölgede; araya kimse giremez -> doğru.
public sealed class LockedBankAccount(decimal initial) : IBankAccount
{
    private decimal _balance = initial;
    private readonly object _gate = new();

    public decimal Balance { get { lock (_gate) return _balance; } }

    public bool Withdraw(decimal amount)
    {
        lock (_gate)
        {
            if (_balance >= amount)
            {
                Thread.SpinWait(200);
                _balance -= amount;
                return true;
            }
            return false;
        }
    }
}

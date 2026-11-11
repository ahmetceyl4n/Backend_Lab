using DotnetBackendLab.ThreadsAsync.Examples;

namespace DotnetBackendLab.ThreadsAsync.Tests;

public class BankAccountTests
{
    // lock'lu hesap paralel çekimlerde asla eksiye düşmez ve tam 100 çekime izin verir.
    [Fact]
    public void LockedAccount_NeverGoesNegative_UnderParallelWithdrawals()
    {
        var account = new LockedBankAccount(100m);
        int success = 0;

        Parallel.For(0, 500, _ =>
        {
            if (account.Withdraw(1m)) Interlocked.Increment(ref success);
        });

        Assert.True(account.Balance >= 0m, $"Bakiye eksiye düştü: {account.Balance}");
        Assert.Equal(100, success);
        Assert.Equal(0m, account.Balance);
    }
}

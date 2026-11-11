using DotnetBackendLab.ThreadsAsync.Examples;

namespace DotnetBackendLab.ThreadsAsync.Tests;

public class CancellationTests
{
    // İptal edilmiş token ile başlayan işlem OperationCanceledException fırlatır.
    [Fact]
    public async Task CancelledOperation_ThrowsOperationCanceledException()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => Example11_Cancellation.ProcessShipmentsAsync(10, cts.Token));
    }
}

using DotnetBackendLab.CSharpFundamentals.Examples;

namespace DotnetBackendLab.CSharpFundamentals.Tests;

public class ExceptionRethrowTests
{
    // 'throw;' orijinal stack trace'i korur: hatanın çıktığı metot (CallCarrierApi) görünür.
    [Fact]
    public void Throw_PreservesOriginalStackTrace()
    {
        var ex = Example10_ExceptionHandling.Capture(Example10_ExceptionHandling.RethrowWithThrow);

        Assert.IsType<CarrierApiException>(ex);
        Assert.Contains(nameof(Example10_ExceptionHandling.CallCarrierApi), ex.StackTrace);
    }

    // 'throw ex;' stack trace'i yeniden başlatır: CallCarrierApi kaybolur, sadece catch eden metot kalır.
    [Fact]
    public void ThrowEx_ResetsStackTrace()
    {
        var ex = Example10_ExceptionHandling.Capture(Example10_ExceptionHandling.RethrowWithThrowEx);

        Assert.IsType<CarrierApiException>(ex);
        Assert.DoesNotContain(nameof(Example10_ExceptionHandling.CallCarrierApi), ex.StackTrace);
        Assert.Contains(nameof(Example10_ExceptionHandling.RethrowWithThrowEx), ex.StackTrace);
    }
}

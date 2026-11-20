using DotnetBackendLab.SolidPatterns.Examples.OpenClosed;

namespace DotnetBackendLab.SolidPatterns.Tests;

public class OpenClosedTests
{
    // Testin içinde yazılan, örnek projede hiç olmayan yeni bir tür.
    private sealed class CargoBikeCalculator : IPriceCalculator
    {
        public string Type => "bisiklet";
        public decimal Calculate(decimal kg) => 15m + kg;
    }

    // PricingService'e dokunmadan yeni hesaplayıcı eklenebilir (genişlemeye açık, değişime kapalı).
    [Fact]
    public void NewCalculator_WorksWithoutChangingPricingService()
    {
        var service = new PricingService([new StandardCalculator(), new CargoBikeCalculator()]);

        Assert.Equal(18m, service.Price("bisiklet", 3m));
        Assert.Equal(60m, service.Price("standart", 3m));
    }

    // Kayıtlı olmayan tür açık bir hatayla reddedilir.
    [Fact]
    public void UnknownType_Throws()
    {
        var service = new PricingService([new StandardCalculator()]);

        Assert.Throws<ArgumentException>(() => service.Price("drone", 1m));
    }
}

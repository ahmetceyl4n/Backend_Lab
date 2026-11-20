using DotnetBackendLab.SolidPatterns.Examples.DependencyInversion;

namespace DotnetBackendLab.SolidPatterns.Tests;

public class DependencyInversionTests
{
    // Testin kendi sahte uygulaması: gerçek e-posta/SMS göndermez, gelen mesajları saklar.
    private sealed class FakeNotifier : INotifier
    {
        public List<string> Messages { get; } = [];
        public void Send(string message) => Messages.Add(message);
    }

    // DeliveryService interface'e bağlı olduğu için testte sahte notifier verilebilir.
    [Fact]
    public void Deliver_SendsNotification_ThroughInjectedNotifier()
    {
        var fake = new FakeNotifier();
        var service = new DeliveryService(fake);

        service.Deliver("TR100");

        Assert.Equal(["TR100 teslim edildi"], fake.Messages);
    }
}

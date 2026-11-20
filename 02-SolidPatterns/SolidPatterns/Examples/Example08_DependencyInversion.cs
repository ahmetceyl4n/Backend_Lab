namespace DotnetBackendLab.SolidPatterns.Examples.DependencyInversion;

public sealed class Example08_DependencyInversion : IExample
{
    public int Number => 8;
    public string Title => "DependencyInversion: new ile somut sınıfa bağlılık vs interface + constructor injection";

    public Task RunAsync()
    {
        // KÖTÜ: servis bağımlılığını kendisi new'liyor. Testte bile gerçek SMTP'ye gider; değiştirmek için kodu açmak gerekir.
        Console.WriteLine("KÖTÜ (new SmtpEmailSender() içeride):");
        new DeliveryServiceBad().Deliver("TR100");
        Console.WriteLine();

        // İYİ: servis sadece INotifier'ı bilir; hangi uygulamanın verileceğine dışarısı karar verir.
        Console.WriteLine("İYİ (INotifier constructor'dan geliyor):");
        new DeliveryService(new SmtpNotifier()).Deliver("TR200");
        new DeliveryService(new SmsNotifier()).Deliver("TR201");

        // Testte sahte uygulama: hiçbir yere bağlanmaz, gönderilenleri sadece hatırlar.
        var fake = new RecordingNotifier();
        new DeliveryService(fake).Deliver("TR202");
        Console.WriteLine($"   Sahte notifier'a gelen mesajlar: [{string.Join(" | ", fake.Sent)}]  (gerçek e-posta gitmedi)");
        Console.WriteLine();
        Console.WriteLine("Üst seviye (DeliveryService) alt seviyeye (SMTP) değil, ikisi de soyutlamaya (INotifier) bağlı.");
        Console.WriteLine("ASP.NET Core'da bağlama DI konteynerinde yapılır: builder.Services.AddScoped<INotifier, SmtpNotifier>();");

        return Task.CompletedTask;
    }
}

// --- Kötü ---
public sealed class SmtpEmailSender
{
    public void Send(string message) => Console.WriteLine($"   [SMTP] smtp.sirket.com'a bağlanıldı, gönderildi: {message}");
}

public sealed class DeliveryServiceBad
{
    private readonly SmtpEmailSender _sender = new();   // somut sınıfa sıkı bağlılık

    public void Deliver(string trackingNo) => _sender.Send($"{trackingNo} teslim edildi");
}

// --- İyi ---
public interface INotifier
{
    void Send(string message);
}

public sealed class SmtpNotifier : INotifier
{
    public void Send(string message) => Console.WriteLine($"   [SMTP] {message}");
}

public sealed class SmsNotifier : INotifier
{
    public void Send(string message) => Console.WriteLine($"   [SMS ] {message}");
}

public sealed class RecordingNotifier : INotifier
{
    public List<string> Sent { get; } = [];
    public void Send(string message) => Sent.Add(message);
}

public sealed class DeliveryService(INotifier notifier)
{
    public void Deliver(string trackingNo)
    {
        // ... teslim iş kuralları ...
        notifier.Send($"{trackingNo} teslim edildi");
    }
}

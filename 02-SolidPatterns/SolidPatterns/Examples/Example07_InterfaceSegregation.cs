namespace DotnetBackendLab.SolidPatterns.Examples.InterfaceSegregation;

public sealed class Example07_InterfaceSegregation : IExample
{
    public int Number => 7;
    public string Title => "InterfaceSegregation: her şeyi içeren IShipmentOperations vs küçük interface'ler";

    public Task RunAsync()
    {
        // KÖTÜ: kurye uygulaması fatura ve iade yapmadığı halde bu metotları "uygulamak" zorunda.
        IShipmentOperations courierBad = new CourierAppBad();
        Console.WriteLine("KÖTÜ (şişman interface):");
        Console.WriteLine($"   {courierBad.Track("TR100")}");
        try { courierBad.CreateInvoice("TR100"); }
        catch (NotImplementedException) { Console.WriteLine("   CreateInvoice -> NotImplementedException (interface 'yapabilir' diyordu!)"); }
        Console.WriteLine();

        // İYİ: her istemci sadece ihtiyacı olan interface'i görür.
        var courier = new CourierApp();
        var accounting = new AccountingService();
        Console.WriteLine("İYİ (küçük interface'ler):");
        Console.WriteLine($"   {courier.Track("TR200")}");
        Console.WriteLine($"   {courier.Deliver("TR200")}");
        Console.WriteLine($"   {accounting.CreateInvoice("TR200")}");

        IInvoiceable[] invoicers = [accounting];   // CourierApp buraya konamaz: derleme zamanında engellenir
        Console.WriteLine($"   Faturalayabilenler: {string.Join(", ", invoicers.Select(i => i.GetType().Name))}");

        return Task.CompletedTask;
    }
}

public interface IShipmentOperations
{
    string Track(string trackingNo);
    string Deliver(string trackingNo);
    string CreateInvoice(string trackingNo);
    string Refund(string trackingNo);
}

public sealed class CourierAppBad : IShipmentOperations
{
    public string Track(string trackingNo) => $"{trackingNo}: dağıtımda";
    public string Deliver(string trackingNo) => $"{trackingNo}: teslim edildi";
    public string CreateInvoice(string trackingNo) => throw new NotImplementedException();
    public string Refund(string trackingNo) => throw new NotImplementedException();
}

public interface ITrackable { string Track(string trackingNo); }
public interface IDeliverable { string Deliver(string trackingNo); }
public interface IInvoiceable { string CreateInvoice(string trackingNo); }
public interface IRefundable { string Refund(string trackingNo); }

public sealed class CourierApp : ITrackable, IDeliverable
{
    public string Track(string trackingNo) => $"[Kurye] {trackingNo}: dağıtımda";
    public string Deliver(string trackingNo) => $"[Kurye] {trackingNo}: teslim edildi";
}

public sealed class AccountingService : IInvoiceable, IRefundable
{
    public string CreateInvoice(string trackingNo) => $"[Muhasebe] {trackingNo}: fatura kesildi";
    public string Refund(string trackingNo) => $"[Muhasebe] {trackingNo}: iade yapıldı";
}

namespace DotnetBackendLab.SolidPatterns.Examples.LiskovSubstitution;

public sealed class Example06_LiskovSubstitution : IExample
{
    public int Number => 6;
    public string Title => "LiskovSubstitution: sözleşmeyi bozan override, dikdörtgen-kare örneği";

    public Task RunAsync()
    {
        // 1) Sözleşme: "Price her kargo için negatif olmayan bir sayı döndürür."
        Console.WriteLine("1) Kargo fiyatları toplanıyor:");
        ShipmentPricing[] bad = [new ShipmentPricing(), new FreeShipmentBad()];
        try { Console.WriteLine($"   KÖTÜ: toplam = {bad.Sum(p => p.Price(5))}"); }
        catch (NotSupportedException ex) { Console.WriteLine($"   KÖTÜ: döngü patladı -> {ex.Message}"); }

        ShipmentPricing[] good = [new ShipmentPricing(), new FreeShipment()];
        Console.WriteLine($"   İYİ : toplam = {good.Sum(p => p.Price(5)):N2} TL  (ücretsiz kargo 0 döndürür, sözleşme korunur)");
        Console.WriteLine();

        // 2) Klasik örnek: matematikte kare bir dikdörtgendir, ama kodda Square : Rectangle LSP'yi bozar.
        Console.WriteLine("2) Dikdörtgen-kare:");
        Console.WriteLine($"   Rectangle: genişlik 5, yükseklik 4 -> alan {ResizeAndGetArea(new Rectangle())} (beklenen 20)");
        Console.WriteLine($"   Square   : genişlik 5, yükseklik 4 -> alan {ResizeAndGetArea(new Square())} (beklenen 20!)");
        Console.WriteLine("   Square, Height atanınca Width'i de değiştiriyor; Rectangle bekleyen kod yanlış sonuç alıyor.");

        IShape[] shapes = [new FixedRectangle(5, 4), new FixedSquare(4)];
        Console.WriteLine($"   İYİ: ayrı değişmez tipler + IShape -> alanlar: {string.Join(", ", shapes.Select(s => s.Area))}");

        return Task.CompletedTask;
    }

    // Rectangle ile çalıştığını sanan istemci kodu.
    private static int ResizeAndGetArea(Rectangle r)
    {
        r.Width = 5;
        r.Height = 4;
        return r.Area;
    }
}

public class ShipmentPricing
{
    public virtual decimal Price(decimal kg) => 30m + kg * 10m;
}

// KÖTÜ: temel sınıfın yerine konulamıyor; çağıran her yer "ya bu FreeShipmentBad ise?" diye kontrol etmek zorunda.
public sealed class FreeShipmentBad : ShipmentPricing
{
    public override decimal Price(decimal kg) => throw new NotSupportedException("Ücretsiz kargonun fiyatı sorulamaz");
}

// İYİ: sözleşmeye uyar (negatif olmayan fiyat döndürür).
public sealed class FreeShipment : ShipmentPricing
{
    public override decimal Price(decimal kg) => 0m;
}

public class Rectangle
{
    public virtual int Width { get; set; }
    public virtual int Height { get; set; }
    public int Area => Width * Height;
}

public sealed class Square : Rectangle
{
    public override int Width { set { base.Width = value; base.Height = value; } }
    public override int Height { set { base.Width = value; base.Height = value; } }
}

public interface IShape { int Area { get; } }
public sealed record FixedRectangle(int Width, int Height) : IShape { public int Area => Width * Height; }
public sealed record FixedSquare(int Side) : IShape { public int Area => Side * Side; }

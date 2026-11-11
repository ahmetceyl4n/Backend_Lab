namespace DotnetBackendLab.CSharpFundamentals.Examples;

public sealed class Example02_ParameterPassing : IExample
{
    public int Number => 2;
    public string Title => "ParameterPassing: int / class parametre, alan değiştirme, new atama, ref";

    public Task RunAsync()
    {
        int price = 100;
        ApplyDiscount(price);
        Console.WriteLine($"1) int geçildi, içeride 50 yapıldı      -> dışarıda price = {price}  (kopya değişti)");

        ApplyDiscountByRef(ref price);
        Console.WriteLine($"2) ref int geçildi, içeride 50 yapıldı  -> dışarıda price = {price}  (asıl değişken değişti)");
        Console.WriteLine();

        var parcel = new Parcel("TR100", 5);
        ChangeWeight(parcel);
        Console.WriteLine($"3) class geçildi, içeride Weight=99     -> dışarıda {parcel}  (aynı nesne, alan değişti)");

        ReplaceParcel(parcel);
        Console.WriteLine($"4) class geçildi, içeride p = new(...)  -> dışarıda {parcel}  (sadece yerel kopya yönlendi)");

        ReplaceParcelByRef(ref parcel);
        Console.WriteLine($"5) ref class geçildi, içeride p = new() -> dışarıda {parcel}  (dışarıdaki değişken yönlendi)");
        Console.WriteLine();
        Console.WriteLine("Özet: C#'ta varsayılan geçiş 'değerle'dir. class için kopyalanan değer, nesnenin adresidir.");

        return Task.CompletedTask;
    }

    // Parametre 'price'ın bir kopyası. Kopyayı değiştirmek çağıranı etkilemez.
    public static void ApplyDiscount(int price) => price = 50;

    // ref: çağıranın değişkeninin kendisi geçer.
    public static void ApplyDiscountByRef(ref int price) => price = 50;

    // Adresin kopyası geldi ama aynı nesneyi gösteriyor -> alan değişikliği dışarıda görünür.
    public static void ChangeWeight(Parcel p) => p.Weight = 99;

    // Yerel 'p' başka nesneye yönlenir; çağıranın değişkeni eski nesneyi göstermeye devam eder.
    public static void ReplaceParcel(Parcel p) => p = new Parcel("YENI", 1);

    // ref ile çağıranın değişkeni yeni nesneye yönlenir.
    public static void ReplaceParcelByRef(ref Parcel p) => p = new Parcel("YENI", 1);
}

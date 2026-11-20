# 02 - SolidPatterns

OOP temelleri (encapsulation, polymorphism, composition), SOLID prensipleri ve backend'de en sık
kullanılan tasarım desenlerini (Singleton, Factory, Strategy, Decorator, Repository + Unit of Work)
çalıştırılabilir örneklerle gösterir. Örneklerin çoğunda **kötü** ve **iyi** hal yan yana durur.
Örnekler kargo alanından seçilmiştir.

## Çalıştırma

```bash
# Menüyle:
dotnet run --project SolidPatterns

# Doğrudan bir örnek (ör. 9. örnek):
dotnet run --project SolidPatterns -- 9
```

## Örnekler

| # | Örnek | Kötü → İyi |
|---|-------|------------|
| 1 | Encapsulation | Public alanlı kargo (teslim edilen kargo geri "Hazırlanıyor" olabiliyor) → `private set` + `Dispatch()` / `Deliver()`, geçersiz geçişte exception |
| 2 | Polymorphism | Abstract `Consignment` → `Parcel` / `Document` override; overload derleme zamanında değişken tipine, override çalışma zamanında nesne tipine göre seçilir |
| 3 | CompositionOverInheritance | Her kombinasyon için ayrı sınıf (`InsuredExpressShipment`...) → `IPriceCalculator` + `IInsurance` bileşenlerinden kurulan tek `Shipment` |
| 4 | SingleResponsibility | Hesaplayan + kaydeden + e-posta atan `ShipmentManager` → `PriceCalculator`, `ShipmentStore`, `EmailNotifier` |
| 5 | OpenClosed | `switch` ile fiyat (yeni tür = kodu değiştirmek) → `IPriceCalculator` listesi alan `PricingService` (yeni tür = yeni sınıf) |
| 6 | LiskovSubstitution | Exception fırlatan `FreeShipmentBad` override'ı döngüyü patlatır → sözleşmeye uyan `FreeShipment`; dikdörtgen-kare (20 yerine 16) |
| 7 | InterfaceSegregation | Kuryenin `NotImplementedException` attığı şişman `IShipmentOperations` → `ITrackable`, `IDeliverable`, `IInvoiceable`, `IRefundable` |
| 8 | DependencyInversion | İçinde `new SmtpEmailSender()` olan servis → constructor'dan `INotifier` alan servis; sahte notifier ile test |
| 9 | Singleton | Korumasız singleton (8 thread → 8 örnek) → `lock` → `Lazy<T>` (tek örnek) |
| 10 | Factory | Türe göre `IPriceCalculator` üreten `PriceCalculatorFactory`; oluşturma kararı tek yerde |
| 11 | Strategy | `Dictionary<string, IPricingStrategy>` ile çalışırken seçilen ve değiştirilen fiyat stratejisi |
| 12 | Decorator | `IShipmentRepository`'ye dokunmadan `LoggingShipmentRepository` ve `CachingShipmentRepository` |
| 13 | RepositoryUnitOfWork | Anında kaydeden repository'ler (yarım kayıt) → değişiklikleri bekleten repository'ler + tek `SaveChanges` (ya hepsi ya hiçbiri) |

Her örneğin tipleri kendi alt namespace'indedir (`...Examples.OpenClosed`, `...Examples.Factory`), böylece
`IPriceCalculator` gibi aynı adı taşıyan tipler örnekler arasında çakışmaz.

## Örnek çıktı

```
--- 2. Polymorphism ---
2) Overload (compile-time):
   Label(asParcel)              -> KOLİ etiketi: TR301, 2 kg
   Label(asBase)  [nesne Parcel] -> GENEL etiket: TR300  (değişken tipi seçti)
   asBase.Price()               -> 55,00 TL      (override: nesne tipi seçti)
```

```
--- 9. Singleton ---
8 thread aynı anda Instance istiyor:
   Korumasız  : farklı örnek = 8, constructor çağrısı = 8  (tek olmalıydı!)
   lock       : farklı örnek = 1, constructor çağrısı = 1
   Lazy<T>    : farklı örnek = 1, constructor çağrısı = 1
```

```
--- 13. RepositoryUnitOfWork ---
KÖTÜ: hata -> Kurye #7 adı boş olamaz
   Veritabanı: 1 kargo, 0 kurye  -> kargo var, kuryesi yok (tutarsız!)
İYİ: hata -> Kurye #8 adı boş olamaz
   Veritabanı: 0 kargo, 0 kurye  -> hiçbiri yazılmadı (tutarlı)
```

## Testler

```bash
dotnet test SolidPatterns.Tests
```

- `EncapsulationTests`: yola çıkmamış kargo teslim edilemez, teslim edilmiş kargo tekrar yola çıkamaz, geçerli sıra durumu ve tarihi birlikte günceller.
- `OpenClosedTests`: testin içinde yazılan yeni hesaplayıcı `PricingService` değiştirilmeden çalışır; bilinmeyen tür reddedilir.
- `SingletonTests`: `Lazy<T>` ve `lock`'lu singleton 16 paralel thread'de tek örnek üretir.
- `DecoratorTests`: logging decorator iç repository'yi çağırır; cache decorator aynı anahtar için iç repository'yi bir kez çağırır (bulunamayan sonuç dahil).
- `UnitOfWorkTests`: bir kayıt geçersizse hiçbir kayıt yazılmaz; hepsi geçerliyse ikisi birlikte yazılır; başarısız denemenin kayıtları sonrakine sızmaz.
- `DependencyInversionTests`: `DeliveryService`'e sahte `INotifier` verilerek gerçek e-posta göndermeden test edilir.

> Not: Korumasız singleton kaç örnek üreteceği zamanlamaya bağlı olduğu için test edilmez; yalnızca konsol örneğinde gösterilir.

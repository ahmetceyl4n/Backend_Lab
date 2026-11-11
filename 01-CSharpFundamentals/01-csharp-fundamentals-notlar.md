# C# dil temelleri: notlar

Konu sırası:

1. Değer tipleri ve referans tipleri
2. class, struct ve record (+ sealed, + string)
3. interface ve abstract class
4. Generics
5. delegate, `Func` / `Action` ve event
6. LINQ, deferred execution, `IEnumerable` ve `IQueryable`
7. Exception handling
8. `IDisposable`, `using` ve garbage collector
9. Sık sorulan küçük detaylar (`==` ve `Equals`, `const` ve `readonly`, nullable reference types)

---

## 1. Değer tipleri ve referans tipleri

C#'taki her tip bu iki gruptan birine girer. Fark, **değişkenin içinde ne durduğu**.

- **Değer tipi (value type):** Değişken verinin **kendisini** tutar. `int`, `double`, `bool`, `char`, `decimal`, `DateTime`, `enum`, `struct`.
- **Referans tipi (reference type):** Değişken verinin bellekte durduğu yerin **adresini** tutar. `class`, `string`, diziler, `List<T>`, `object`, `delegate`.

Benzetme:

- Değer tipi bir **fotokopi**: Arkadaşına fotokopi verdin, o üzerine yazdı; senin kâğıdın değişmez.
- Referans tipi bir **depo adresi**: Arkadaşına adresi verdin, o depoya gidip bir kutuyu taşıdı; sen de aynı depoya gidince kutuyu yeni yerinde görürsün.

### Atamada fark

```csharp
// Değer tipi: değer kopyalanır
int a = 10;
int b = a;
b = 20;
Console.WriteLine(a); // 10

// Referans tipi: adres kopyalanır, nesne aynı kalır
class Kargo { public string Durum { get; set; } = "Hazırlanıyor"; }

var k1 = new Kargo();
var k2 = k1;            // aynı nesneyi gösteren ikinci değişken
k2.Durum = "Dağıtımda";
Console.WriteLine(k1.Durum); // "Dağıtımda"
```

### Parametre geçişi ve `ref`

Değişkenleri **kutu** olarak düşün:

- `int s = 1;` → `s` kutusunda **1** yazıyor.
- `var k = new Kargo();` → heap'te bir nesne var; `k` kutusunda o nesnenin **adresi** yazıyor (ör. "Depo #7").

Metot çağrılınca:

- **`ref` yoksa:** Metot **yeni bir kutu** alır, senin kutundaki değer oraya **kopyalanır**. Metot kendi kutusuyla çalışır.
- **`ref` varsa:** Metot yeni kutu almaz, **senin kutunun kendisini** kullanır.

```csharp
void Degistir(int sayi, Kargo kargo)
{
    sayi = 99;               // kendi kopyası → dışarıdaki etkilenmez
    kargo.Durum = "Teslim";  // aynı nesnenin içi → dışarıdaki DEĞİŞİR
    kargo = new Kargo();     // kendi kutusunu başka nesneye çevirdi → dışarıdaki ETKİLENMEZ
}

int s = 1;
var k = new Kargo();
Degistir(s, k);
Console.WriteLine(s);        // 1
Console.WriteLine(k.Durum);  // "Teslim"
```

```csharp
void YeniKargoAta(ref Kargo kargo) => kargo = new Kargo { Durum = "Yeni" };

var k3 = new Kargo();
YeniKargoAta(ref k3);
Console.WriteLine(k3.Durum); // "Yeni"
```

| Tip | Metodun içinde yapılan | `ref` yok | `ref` var |
| --- | --- | --- | --- |
| `int` | `sayi = 99` | Dışarısı değişmez | Dışarısı değişir |
| `class` | `kargo.Durum = "Teslim"` (nesnenin içi) | Dışarısı **değişir** | Dışarısı değişir |
| `class` | `kargo = new Kargo()` (değişkenin kendisi) | Dışarısı değişmez | Dışarısı değişir |

Kural: **`ref` olmadan metot senin değişkenini (kutunu) değiştiremez; referans tipinde sadece kutunun gösterdiği nesnenin içini değiştirebilir.**

Benzerleri: `out` (`ref` gibi ama metodun değer ataması zorunlu; ör. `int.TryParse(metin, out var sonuc)`), `in` (kopyalamadan ama salt okunur geçirir; büyük struct'larda performans için).

### Stack ve heap

- **Stack:** Metot çağrılarının geçici alanı. Metot bitince kendiliğinden boşalır, çok hızlıdır.
- **Heap:** Nesnelerin yaşadığı alan. Kullanılmayan nesneleri garbage collector temizler.

"Değer tipleri stack'te durur" sözü **tam doğru değil**. Değer tipi **bulunduğu yerde** saklanır:

- Metodun içindeki yerel `int` → stack'te.
- Bir class'ın içindeki `int` alanı → nesneyle birlikte heap'te.

### Boxing ve unboxing

Değer tipini `object` gibi bir referans tipine atamak onu heap'te bir kutuya koyar (**boxing**); geri çıkarmak **unboxing**'dir.

```csharp
int sayi = 42;
object kutu = sayi;    // boxing: heap'te yeni nesne
int geri = (int)kutu;  // unboxing
```

Her boxing yeni bir heap nesnesi ve garbage collector yükü demektir. `ArrayList` gibi `object` tabanlı koleksiyonlar her elemanı kutular; `List<int>` gibi generic koleksiyonlar kutulamaz.

### Örnek soru: class içinde struct

```csharp
struct Konum { public int X; }
class Kurye { public Konum Konum; }

var kurye1 = new Kurye();
kurye1.Konum.X = 5;

var kurye2 = kurye1;        // Kurye class → adres kopyalanır, tek nesne
kurye2.Konum.X = 10;        // ortak nesnenin konumu 10 olur

Konum k = kurye1.Konum;     // Konum struct → değer kopyalanır, bağımsız kopya
k.X = 99;                   // sadece kopya değişir

Console.WriteLine(kurye1.Konum.X); // 10
Console.WriteLine(kurye2.Konum.X); // 10
Console.WriteLine(k.X);            // 99
```

Class atamasında iki değişken aynı nesneyi paylaşır; struct atamasında bağımsız bir kopya oluşur. Class'ın içindeki struct nesneyle birlikte heap'te durur ("struct her zaman stack'tedir" sözünün neden yanlış olduğunu gösterir).

### 💡 Mülakat notu: Değer tipi ve referans tipi

**Değer tipi** (`int`, `bool`, `decimal`, `DateTime`, `enum`, `struct`) değişkende verinin kendisini tutar; atamada ve parametre geçişinde **kopyalanır**. **Referans tipi** (`class`, `string`, dizi, `List<T>`, `object`, `delegate`) değişkende nesnenin adresini tutar; atamada adres kopyalanır, iki değişken **aynı nesneyi** gösterir.

**Parametre tuzağı:** Parametreler varsayılan olarak değerle geçer; referans tipinde kopyalanan şey adrestir. Metodun içinde nesnenin alanını değiştirmek dışarıyı etkiler, parametreye `new` ile yeni nesne atamak etkilemez. Dışarıdaki değişkeni de değiştirmek için `ref` gerekir.

**Stack / heap:** Değer tipi bulunduğu yerde saklanır: yerel değişkense stack'te, bir class'ın alanıysa nesneyle birlikte heap'te. Referans tipi nesneler heap'tedir, garbage collector temizler.

**Boxing:** Değer tipini `object`'e ya da bir interface'e atamak onu heap'te kutuya koyar; maliyetlidir. Generic koleksiyonlar boxing yapmaz.

---

## 2. class, struct ve record

Farkları üç soruda toplanır: **kopyalanınca ne olur, eşitlik nasıl karşılaştırılır, değiştirilebilir mi?**

### class

- Referans tipi; atamada nesne paylaşılır.
- `==` ve `Equals` varsayılan olarak **aynı nesne mi?** diye bakar.
- Kalıtım destekler.
- Kimliği olan, zamanla değişen nesneler için: entity'ler, servisler.

```csharp
class Kargo { public string TakipNo { get; set; } = ""; }

var a = new Kargo { TakipNo = "KRG1" };
var b = new Kargo { TakipNo = "KRG1" };
Console.WriteLine(a == b);      // False: içerik aynı ama iki farklı nesne
Console.WriteLine(a.Equals(b)); // False
```

### struct

- Değer tipi; atamada ve metoda geçişte içeriğin tamamı kopyalanır.
- `Equals` alanları karşılaştırır (varsayılanı reflection kullandığı için yavaş). `==` operatörü kendin tanımlamadıkça yoktur (derleme hatası).
- Kalıtım yok, ama interface uygulayabilir.
- Küçük (genelde ~16 byte), kısa ömürlü, değişmeyen veriler için: koordinat, para tutarı. `DateTime`, `TimeSpan`, `Guid` struct'tır.
- Büyük struct'lar her kopyalamada maliyet getirir.
- Değiştirilebilir struct'lar "kopyayı değiştirip asıl veriyi değiştirdim sanma" hatasına yol açar; `readonly struct` tercih edilir.

```csharp
readonly struct Konum
{
    public Konum(double enlem, double boylam) { Enlem = enlem; Boylam = boylam; }
    public double Enlem { get; }
    public double Boylam { get; }
}
```

### record (C# 9+)

- `record` (ya da `record class`) bir **referans tipidir**, ama **eşitliği değere göredir**: bütün alanlar eşitse `==` true döner. `Equals`, `==`, `GetHashCode` derleyici tarafından üretilir.
- Positional yazımda **değişmezdir** (özellikler `init`).
- **`with`** ifadesi: değiştirmek yerine bir alanı farklı yeni bir kopya oluşturur.
- `ToString()` içeriği okunur şekilde yazar.
- Değişmezliği **sığdır**: içindeki `List` gibi alanlar yine değiştirilebilir.
- EF Core entity'leri için genelde uygun değildir (entity'nin kimliği vardır; EF Core referans eşitliğine güvenir).
- `record struct` (C# 10+): değer eşitliği ve `with` getirir ama değer tipidir.

```csharp
public record KargoDto(string TakipNo, string Durum);

var a = new KargoDto("KRG1", "Dağıtımda");
var b = new KargoDto("KRG1", "Dağıtımda");
Console.WriteLine(a == b);   // True

var c = a with { Durum = "Teslim" };   // a değişmedi, yeni nesne
Console.WriteLine(a.Durum);  // "Dağıtımda"
Console.WriteLine(c.Durum);  // "Teslim"

// a.Durum = "Teslim";        // derleme hatası: init-only
```

| | class | struct | record (class) |
| --- | --- | --- | --- |
| Tip | Referans | Değer | Referans |
| Atamada | Nesne paylaşılır | İçerik kopyalanır | Nesne paylaşılır |
| `==` | Aynı nesne mi? | Varsayılan olarak yok | Alanlar eşit mi? |
| `Equals` | Aynı nesne mi? | Alanlar eşit mi? (yavaş) | Alanlar eşit mi? (üretilmiş) |
| Değişebilir mi? | Evet | Evet (`readonly struct` ile hayır) | Positional yazımda hayır, `with` ile kopya |
| Kalıtım | Var | Yok | Record'lar arasında var |
| Tipik kullanım | Entity, servis | Küçük, kısa ömürlü değerler | DTO, mesaj, value object |

### Örnek soru: hangisini seçerdin?

1. API'nin "kargo oluştur" isteğinde dışarıdan gelen veri (alıcı, adres, ağırlık) → **record.** Veri taşıyıcı (DTO); kimliği yok, içeriği önemli, işlenirken değişmemeli.
2. Veritabanında saklanan, durumu zamanla değişen `Kargo` → **class.** Kimliği olan ve değişen bir entity; EF Core referans eşitliğine güvenir.
3. Kuryenin anlık konumu (enlem, boylam), saniyede binlerce kez oluşturuluyor → **struct** (en doğrusu `readonly struct` / `readonly record struct`). Küçük, kısa ömürlü; class olsaydı her biri heap nesnesi olup garbage collector'a yük bindirirdi.

### 💡 Mülakat notu: class, struct ve record farkı

- **class:** Referans tipi. Atamada nesne paylaşılır. `==` ve `Equals` varsayılan olarak referansa bakar. Kalıtım destekler. Kimliği olan, değişen nesneler (entity, servis) için.
- **struct:** Değer tipi. Atamada ve parametre geçişinde tamamen kopyalanır. `Equals` alanlara bakar (varsayılanı yavaş), `==` tanımlanmadıkça yoktur. Kalıtım yok. Küçük, kısa ömürlü, değişmeyen veriler için. Mutable struct hatalara yol açar; `readonly struct` tercih edilir.
- **record:** Referans tipi, ama eşitliği değere göredir. Positional yazımda değişmezdir; değişiklik `with` ile yeni kopya oluşturarak yapılır. DTO, API modeli, kuyruk mesajı, value object için. Değişmezliği sığdır; EF Core entity'si için genelde uygun değildir.

---

## Ek: sealed

`sealed` bir class'a yazılırsa o class'tan **kalıtım alınamaz**. Bir `override` metoda yazılırsa (`sealed override`) alt sınıflar o metodu tekrar değiştiremez.

```csharp
public sealed class TakipNumarasiUretici { /* ... */ }
public class OzelUretici : TakipNumarasiUretici { }   // derleme hatası

public abstract class Gonderi
{
    protected virtual decimal Sigorta() => 10m;
}

public class Koli : Gonderi
{
    protected sealed override decimal Sigorta() => 15m;   // değiştirdi ve mühürledi
}

public class BuyukKoli : Koli
{
    protected override decimal Sigorta() => 25m;          // derleme hatası
}
```

Neden kullanılır:

1. **Tasarım niyetini göstermek:** "Bu sınıf kalıtım için tasarlanmadı." Birçok kod standardı "varsayılan olarak sealed, gerekirse aç" der.
2. **Güvenlik ve tutarlılık:** Önemli davranışın alt sınıflar tarafından bozulmasını engeller.
3. **Küçük performans kazancı:** JIT sanal çağrıları doğrudan çağrıya çevirebilir (asıl neden bu olmamalı).

| Anahtar kelime | Anlamı |
| --- | --- |
| `abstract` | Doğrudan oluşturulamaz, mutlaka türetilmeli |
| `sealed` | Oluşturulabilir ama türetilemez |
| `static` | Ne oluşturulabilir ne türetilebilir; sadece statik üyeler |
| `virtual` (metot) | Alt sınıf isterse `override` edebilir |
| `sealed override` (metot) | Bu sınıfta değiştirildi, alt sınıflar artık değiştiremez |

### 💡 Mülakat notu: sealed

`sealed` class'tan kalıtım alınamaz; `sealed override` metot alt sınıflarda tekrar değiştirilemez. Tasarım niyetini göstermek, davranışın bozulmasını engellemek ve küçük bir performans kazancı için kullanılır. **`string` sealed'dır**, çünkü değişmezliği sözlük anahtarı kullanımı, string havuzu ve güvenlik kontrolleri gibi birçok yerde varsayılır; bir alt sınıfla değiştirilebilir hale gelmesi bu varsayımları bozardı. Struct'lar zaten türetilemez; kalıtım gerekmeyen DTO'lar `sealed record` yazılabilir.

---

## Ek: string

String bir **referans tipidir**, ama **değişmezdir (immutable)**: oluşturulduktan sonra içeriği değiştirilemez. "Değiştiriyor" gibi görünen her işlem (`ToUpper`, `Replace`, `+=`) **yeni bir string** üretir.

```csharp
string a = "ahmet";   // heap'te "ahmet" nesnesi, a onu gösteriyor
string b = a;         // b de aynı nesneyi gösteriyor
b = "mehmet";         // YENİ bir "mehmet" nesnesi oluştu, b artık onu gösteriyor
Console.WriteLine(a); // "ahmet"
```

`b = "mehmet"` nesneyi değiştirmez, **`b` değişkeninin gösterdiği yeri** değiştirir. String'in içini değiştiren bir işlem olmadığı için (`b[0] = 'm'` derleme hatası) string'i paylaşmak güvenlidir; bu yüzden pratikte değer tipi gibi davranır.

```csharp
string b2 = a;
b2 = b2.ToUpper();      // yeni string: "AHMET"
b2.Replace("A", "E");   // yeni string döner ama atanmadığı için boşa gider
Console.WriteLine(a);   // "ahmet", hiçbir koşulda değişmez
```

**`==` string'te içeriği karşılaştırır** (operatör yeniden tanımlanmıştır):

```csharp
string x = "kargo";
string y = new string("kargo".ToCharArray());
Console.WriteLine(x == y);                 // True: içerik aynı
Console.WriteLine(ReferenceEquals(x, y));  // False: iki ayrı nesne
```

**Döngüde `+=` pahalıdır:** Her turda yeni string oluşur ve eski içerik kopyalanır. Çok sayıda birleştirmede `StringBuilder` kullanılır:

```csharp
var sb = new StringBuilder();
foreach (var k in kargolar)
    sb.Append(k.TakipNo).Append(';').Append(k.Durum).Append('\n');
string rapor = sb.ToString();
```

### 💡 Mülakat notu: String referans tipi ama neden değer tipi gibi davranıyor?

String referans tipidir ama değişmezdir; her "değiştirme" işlemi yeni bir nesne üretir. `b = "mehmet"` gibi bir atama nesneyi değil, değişkenin gösterdiği yeri değiştirir; diğer değişkenler eski nesneyi göstermeye devam eder. İçeriği değiştiren bir işlem olmadığı için paylaşmak güvenlidir. `==` string'te içeriği karşılaştırır (aynı nesne mi sorusunu `ReferenceEquals` cevaplar). Döngüde `+=` her turda yeni string oluşturduğu için pahalıdır; `StringBuilder` kullanılır. Değişmezliğin korunması için `string` sealed'dır.

---

## 3. interface ve abstract class

### Sorun

Kargo ücreti türe göre farklı hesaplanıyor. Her yere `if (tur == "standart") ... else if (tur == "hizli") ...` yazmak, yeni bir tür eklendiğinde bütün bu yerleri değiştirmeyi gerektirir. Çözüm, "fiyat hesaplayabilen bir şey" fikrini soyutlamak; kodun geri kalanı hangi türle çalıştığını bilmez.

### interface: "şunu yapabilirim" sözleşmesi

Interface tipin **ne yapabileceğini** söyler, **nasıl** yapacağını söylemez.

```csharp
public interface IFiyatHesaplayici
{
    decimal Hesapla(Kargo kargo);
}

public class StandartFiyat : IFiyatHesaplayici
{
    public decimal Hesapla(Kargo kargo) => kargo.AgirlikKg * 12.5m;
}

public class HizliFiyat : IFiyatHesaplayici
{
    public decimal Hesapla(Kargo kargo) => kargo.AgirlikKg * 20m + 50m;
}

public class KargoServisi
{
    private readonly IFiyatHesaplayici _hesaplayici;
    public KargoServisi(IFiyatHesaplayici hesaplayici) => _hesaplayici = hesaplayici;
    public decimal UcretGetir(Kargo kargo) => _hesaplayici.Hesapla(kargo);
}
```

`KargoServisi` hangi hesaplayıcıyla çalıştığını bilmez. Yeni tür eklemek için sadece yeni class yazılır; testte sahte bir hesaplayıcı verilebilir.

Benzetme: Interface bir **ehliyet** gibi; "araç kullanabilir" der, kişinin kim olduğunu söylemez. Bir kişinin birden fazla belgesi olabilir.

Kuralları:
- Bir class **birden fazla** interface uygulayabilir.
- **Alan (field) ve constructor içermez**, veri tutmaz.
- Struct'lar da uygulayabilir.

### abstract class: "yarısı hazır" temel sınıf

Aynı aileden sınıfların ortak verisini ve ortak kodunu taşır; eksik olduğu için doğrudan oluşturulamaz.

```csharp
public abstract class Gonderi
{
    // Ortak veri
    public string TakipNo { get; } = Guid.NewGuid().ToString("N")[..10];
    public DateTime OlusturmaTarihi { get; } = DateTime.UtcNow;

    // Ortak formül
    public decimal ToplamUcret() => TemelUcret() + Sigorta();

    // Alt sınıf yazmak ZORUNDA
    protected abstract decimal TemelUcret();

    // Alt sınıf İSTERSE değiştirir
    protected virtual decimal Sigorta() => 10m;
}

public class Koli : Gonderi
{
    public decimal AgirlikKg { get; init; }
    protected override decimal TemelUcret() => AgirlikKg * 12.5m;
}

public class Evrak : Gonderi
{
    protected override decimal TemelUcret() => 30m;
    protected override decimal Sigorta() => 0m;
}

var koli = new Koli { AgirlikKg = 4 };
Console.WriteLine(koli.ToplamUcret()); // 4 × 12.5 + 10 = 60
// var g = new Gonderi();   // HATA: abstract class oluşturulamaz
```

Kuralları:
- Bir class **yalnızca bir** sınıftan türeyebilir.
- Alan, constructor, `protected` üyeler içerebilir.
- `abstract` metot: gövdesi yok, alt sınıf **mutlaka** yazar.
- `virtual` metot: gövdesi var, alt sınıf **isterse** `override` eder.

### Birlikte kullanım

```csharp
public interface IBildirimGonderici
{
    Task GonderAsync(string alici, string mesaj);
}

public abstract class BildirimGondericiBase : IBildirimGonderici
{
    public async Task GonderAsync(string alici, string mesaj)
    {
        Console.WriteLine($"Bildirim gönderiliyor: {alici}");   // ortak adım
        await GercekGonderAsync(alici, mesaj);
    }

    protected abstract Task GercekGonderAsync(string alici, string mesaj);
}

public class SmsGonderici : BildirimGondericiBase { /* ... */ }
public class EpostaGonderici : BildirimGondericiBase { /* ... */ }
```

| | interface | abstract class |
| --- | --- | --- |
| Ne ifade eder? | "Şunu yapabilir" | "Şu ailenin üyesi" |
| Kaç tanesinden türetilir? | Birden fazla | Yalnızca bir |
| Veri (alan) tutar mı? | Hayır | Evet |
| Constructor | Yok | Var |
| Ortak kod | Çok sınırlı (default method) | Evet, asıl amacı |
| `new` ile oluşturulur mu? | Hayır | Hayır |
| Struct uygulayabilir mi? | Evet | Hayır |
| Tipik kullanım | DI, test, farklı türlerde ortak yetenek | Aynı ailede ortak veri ve kod |

### Gövdeli ve gövdesiz metot

Metodun iki kısmı vardır: **imza** (adı, parametreleri, dönüş tipi) ve **gövde** (süslü parantezlerin içindeki, ne yapacağını anlatan kod).

```csharp
public decimal Hesapla(Kargo kargo)      // imza
{
    return kargo.AgirlikKg * 12.5m;      // gövde
}

public decimal Hesapla2(Kargo kargo) => kargo.AgirlikKg * 12.5m;   // kısa yazılış, yine gövdeli
```

- **Gövdesiz metot:** Sadece imza, sonunda `;` var. "Ne yapacağını başkası yazacak." (interface metotları, `abstract` metotlar)
- **Gövdeli metot:** Ne yapacağı burada yazıyor.

### Default interface method (C# 8+)

Eskiden interface'teki bütün metotlar gövdesizdi. C# 8'den beri gövdeli metot yazılabilir:

```csharp
public interface IFiyatHesaplayici
{
    decimal Hesapla(Kargo kargo);                                  // uygulayan yazmak zorunda
    decimal KdvliHesapla(Kargo kargo) => Hesapla(kargo) * 1.20m;   // uygulayan yazmasa da çalışır
}
```

Asıl amacı **geriye dönük uyumluluk**: 50 class'ın uyguladığı bir interface'e yeni metot eklendiğinde hepsinin derleme hatası vermemesi. Günlük kodda nadiren kullanılır.

### 💡 Mülakat notu: interface ve abstract class farkı

**Interface** bir sözleşmedir: tipin ne yapabileceğini söyler, nasıl yapacağını söylemez. Bir class birden fazla interface uygulayabilir. Alan ve constructor içermez, veri tutmaz. Struct'lar da uygulayabilir. Backend'de dependency injection ve test için en sık kullanılan soyutlama aracıdır.

**Abstract class**, aynı aileden sınıfların ortak verisini ve kodunu taşıyan, doğrudan oluşturulamayan temel sınıftır. Bir class yalnızca bir sınıftan türeyebilir. Alan, constructor ve `protected` üyeler içerebilir. `abstract` metotlar zorunlu olarak, `virtual` metotlar isteğe bağlı olarak alt sınıfta yazılır.

Seçim: farklı türlerde ortak yetenek ya da değiştirilebilir bağımlılık → interface. Aynı ailede ortak veri ve kod → abstract class. İkisi birlikte de kullanılır.

### 💡 Mülakat notu: Default method varken abstract class neden hâlâ gerekli?

Default interface method interface'e **davranış** ekledi, ama interface hâlâ **veri tutamaz**: instance alanı ve constructor içeremez. Her nesneye ait `TakipNo`, `OlusturmaTarihi` gibi veriler ve onları başlatan mantık interface'te yazılamaz. Abstract class ortak veriyi, constructor ile başlatmayı ve veri üzerinde çalışan ortak kodu tek yerde toplar.

Amaçları da farklıdır: default method'un amacı mevcut uygulamaları bozmadan interface'e metot eklemektir (geriye dönük uyumluluk); abstract class'ın amacı aynı aileden sınıflara ("Koli bir Gönderi'dir") ortak bir temel vermektir.

---

## 4. Generics

### Sorun: aynı kodu farklı tipler için tekrar yazmak

Her tip için ayrı cache sınıfı (`KargoCache`, `KuryeCache`, `SubeCache`) kod tekrarıdır. Eski çözüm `object` kullanmaktı:

```csharp
public class Cache
{
    private readonly Dictionary<string, object> _veri = new();
    public void Ekle(string anahtar, object deger) => _veri[anahtar] = deger;
    public object? Getir(string anahtar) => _veri.GetValueOrDefault(anahtar);
}

var cache = new Cache();
cache.Ekle("k2", new Kurye());               // derleyici itiraz etmez
var kargo = (Kargo)cache.Getir("k2")!;       // çalışırken patlar: InvalidCastException
```

Sorunları: tip güvenliği yok (hata çalışırken çıkar), her okumada cast gerekir, değer tiplerinde boxing olur.

### Çözüm: tipi parametre olarak vermek

```csharp
public class Cache<T>
{
    private readonly Dictionary<string, T> _veri = new();
    public void Ekle(string anahtar, T deger) => _veri[anahtar] = deger;
    public T? Getir(string anahtar) => _veri.GetValueOrDefault(anahtar);
}

var kargoCache = new Cache<Kargo>();
kargoCache.Ekle("k1", new Kargo());
// kargoCache.Ekle("k2", new Kurye());   // DERLEME HATASI
Kargo? k = kargoCache.Getir("k1");        // cast yok

var sayac = new Cache<int>();             // boxing yok
```

`T` bir yer tutucudur; `Cache<Kargo>` yazınca her `T`, `Kargo` olur. Kazançlar: **tip güvenliği, cast yok, boxing yok.** `List<T>`, `Dictionary<TKey, TValue>`, `Task<T>`, `IEnumerable<T>`, `Func<T>` hep generic'tir.

### Generic metot

```csharp
public static T IlkVeyaVarsayilan<T>(List<T> liste, T varsayilan)
    => liste.Count > 0 ? liste[0] : varsayilan;

var ilkSayi = IlkVeyaVarsayilan(new List<int> { 5, 8 }, 0);   // T otomatik int anlaşılır
```

Derleyici çoğu zaman tipi parametrelerden kendisi çıkarır (type inference).

### Constraint'ler (`where`)

```csharp
public interface IEntity
{
    Guid Id { get; }
}

public class Repository<T> where T : class, IEntity
{
    private readonly Dictionary<Guid, T> _kayitlar = new();
    public void Ekle(T kayit) => _kayitlar[kayit.Id] = kayit;   // T'nin Id'si olduğu biliniyor
    public T? Getir(Guid id) => _kayitlar.GetValueOrDefault(id);
}

var kargoRepo = new Repository<Kargo>();     // Kargo : IEntity ise çalışır
// var sayiRepo = new Repository<int>();     // DERLEME HATASI
```

Kısıtlama derleyiciye `T` hakkında bilgi verir; bu sayede `kayit.Id` yazılabilir.

| Kısıtlama | Anlamı |
| --- | --- |
| `where T : class` | Referans tipi olmalı |
| `where T : struct` | Değer tipi olmalı |
| `where T : new()` | Parametresiz constructor'ı olmalı (`new T()` için) |
| `where T : IEntity` | Bu interface'i uygulamalı |
| `where T : Gonderi` | Bu sınıf ya da ondan türemiş olmalı |
| `where T : notnull` | Null olamayan tip olmalı |

Birlikte yazılabilir: `where T : class, IEntity, new()`.

### Kovaryans (ileri seviye, kısaca)

```csharp
IEnumerable<string> isimler = new List<string>();
IEnumerable<object> nesneler = isimler;    // çalışır

List<string> liste = new List<string>();
// List<object> nesneler2 = liste;         // DERLEME HATASI
```

`IEnumerable<out T>` sadece okuma yaptığı için daha genel tipe güvenle çevrilebilir. `List<T>`'ye yazma yapılabildiği için çevrilemez; çevrilseydi string listesine `int` eklenebilirdi.

### 💡 Mülakat notu: Generics

Generics tipi bir parametre olarak vermeyi sağlar: sınıf ya da metot bir kez yazılır, tip kullanıldığı yerde belirtilir (`Cache<Kargo>`, `List<int>`). `object` kullanmaya göre:

1. **Tip güvenliği:** Yanlış tip derleme zamanında yakalanır; `object` ile hata çalışırken çıkar (`InvalidCastException`).
2. **Cast gerekmez.**
3. **Boxing yoktur:** `List<int>` değer tiplerini kutulamaz.

**Constraint'ler (`where`)** `T` üzerinde kısıtlama koyar ve derleyiciye `T` hakkında bilgi verir; böylece `T`'nin üyelerine erişilebilir. Sık kullanılanlar: `class`, `struct`, `new()`, interface ya da temel sınıf, `notnull`. Backend'de generic repository (`IRepository<T>`), EF Core'daki `DbSet<T>`, `Task<T>`, `IEnumerable<T>` en sık örneklerdir.

**Kovaryans:** `IEnumerable<string>`, `IEnumerable<object>`'e atanabilir (sadece okuma, `out T`); `List<string>`, `List<object>`'e atanamaz (yazma yapılabilir, tip güvenliği bozulur).

### Ek: Cast (tip dönüştürme) ve object

**Cast**, bir değeri başka bir tipe çevirmektir.

- **Örtük (implicit):** Veri kaybı yoksa otomatik yapılır.
- **Açık (explicit):** Veri kaybı ya da hata riski varsa parantezle yazılır.

```csharp
int sayi = 42;
long buyuk = sayi;            // örtük: int → long

double ondalik = 9.99;
int tam = (int)ondalik;       // açık: 9 (ondalık kısım atılır)

object nesne = new Kurye();
Kargo k = (Kargo)nesne;       // derlenir ama çalışırken patlar: InvalidCastException
```

Güvenli kontrol:

```csharp
if (nesne is Kargo k1)        // is: kontrol eder ve aynı anda çevirir
    Console.WriteLine(k1.TakipNo);

Kargo? k2 = nesne as Kargo;   // as: uymazsa null döner
```

| Yazım | Tip yanlışsa |
| --- | --- |
| `(Kargo)nesne` | `InvalidCastException` fırlatır |
| `nesne as Kargo` | `null` döner |
| `nesne is Kargo k` | `false` döner |

**object ile generics farkı (koli benzetmesi):** C#'ta her tip `object`'ten türer. `object` **etiketsiz bir koli** gibidir: içine her şey girer, ama açınca ne olduğunu bilemezsin, kullanmak için cast etmen gerekir ve yanlış tip konduysa hata ancak çalışırken çıkar. Generics **etiketli koli** gibidir: `List<Kargo>` "içine sadece Kargo girer" der; yanlış tip derlerken yakalanır, çıkarırken cast gerekmez.

```csharp
List<object> koli = new();
koli.Add(new Kargo());
koli.Add("merhaba");               // girer: string de bir object
// koli[0].TakipNo                 // HATA: derleyici içinde ne olduğunu bilmiyor
Kargo k3 = (Kargo)koli[0];         // cast gerekiyor

List<Kargo> kargolar = new();
// kargolar.Add("merhaba");        // DERLEME HATASI
Kargo ilk = kargolar[0];           // cast yok
```

### Ek: Generic metotta operatör kullanımı

`T`'si kısıtlanmamış bir generic metotta derleyici `T` hakkında hiçbir şey bilmez; `>`, `+` gibi operatörlere izin vermez (T bir `Kargo` olabilir, iki kargo `>` ile karşılaştırılamaz).

```csharp
public static T EnBuyuk<T>(T a, T b) => a > b ? a : b;   // derlenmez
```

Çözüm, `T`'ye kısıtlama koyup yeteneği interface üzerinden kullanmak:

```csharp
public static T EnBuyuk<T>(T a, T b) where T : IComparable<T>
    => a.CompareTo(b) > 0 ? a : b;

EnBuyuk(3, 5);                          // 5
EnBuyuk("ahmet", "mehmet");             // "mehmet"
// EnBuyuk(new Kargo(), new Kargo());   // derleme hatası: Kargo karşılaştırılabilir değil
```

`int`, `string`, `DateTime` `IComparable<T>`'yi zaten uygular. Kendi sınıfın için uygulayıp `CompareTo` yazarsın:

```csharp
public class Kargo : IComparable<Kargo>
{
    public decimal AgirlikKg { get; set; }
    public int CompareTo(Kargo? diger) => AgirlikKg.CompareTo(diger?.AgirlikKg ?? 0);
}
```

(.NET 7+ ile sayılar için `where T : INumber<T>` gibi generic math interface'leri de var; junior seviyesinde gerekmez.)

### 💡 Mülakat notu: Cast ve generic constraint

Cast bir değeri başka tipe çevirir: örtük (güvenli, otomatik) ya da açık (parantezle; `(Kargo)nesne` tip yanlışsa çalışırken `InvalidCastException`). Güvenli alternatifler `as` (uymazsa `null`) ve `is` (pattern matching). `object` ile çalışmak her okumada cast gerektirir ve hatayı çalışma zamanına bırakır; generics bunu ortadan kaldırır.

Kısıtlanmamış `T` üzerinde operatör ya da üye kullanılamaz; derleyiciye yetenek garantisi vermek için constraint gerekir: `where T : IComparable<T>` sonrası `a.CompareTo(b)` kullanılabilir.

---

## 5. delegate, Func, Action ve event

### Delegate: bir metodu değişkene koymak

Değişkenlere şimdiye kadar veri koyduk. Delegate, bir değişkene ya da parametreye **bir metodu** koymayı sağlar; o değişken çağrılınca içindeki metot çalışır. Benzetme: şubeye bırakılan "kargo gelince şunu yap" talimat kâğıdı.

### Func, Action, Predicate

Günümüzde kendi delegate tipini yazmak yerine hazır tipler kullanılır:

```csharp
Action<string> bildir = mesaj => Console.WriteLine($"SMS: {mesaj}");
bildir("Kargonuz yola çıktı");            // dönüş yok

Func<decimal, decimal> ucretHesapla = agirlik => agirlik * 12.5m;
decimal ucret = ucretHesapla(4);          // 50; son tip parametresi dönüş tipi

Predicate<Kargo> agirMi = k => k.AgirlikKg > 10;   // bool döner

Func<int, int, int> topla = (a, b) => a + b;       // iki int alır, int döner
Action<int> yazdir = sayi => Console.WriteLine(sayi);
yazdir(topla(3, 4));                               // 7
```

| | Ne yapar? | Örnek |
| --- | --- | --- |
| `Action` | Bir şey yapar, dönüş yok | `Action<string>` |
| `Func<..., TSonuc>` | Bir şey yapar, sonuç döner (son tip dönüş tipi) | `Func<decimal, decimal>` |
| `Predicate<T>` | Kontrol yapar, `bool` döner | `Predicate<Kargo>` |

`=>` ile yazılan kısa, isimsiz metotlara **lambda** denir.

### Davranışı parametre olarak vermek

```csharp
List<Kargo> Filtrele(List<Kargo> kargolar, Func<Kargo, bool> kosul)
{
    var sonuc = new List<Kargo>();
    foreach (var k in kargolar)
        if (kosul(k))
            sonuc.Add(k);
    return sonuc;
}

var agirlar   = Filtrele(kargolar, k => k.AgirlikKg > 10);
var teslimler = Filtrele(kargolar, k => k.Durum == "Teslim");
```

LINQ tamamen bu fikre dayanır: `kargolar.Where(k => k.AgirlikKg > 10)`.

### Lambda ile Func'ın ilişkisi

**Lambda** bir yazım şekli (değer), **`Func`** onun tipi. `int sayi = 42`'deki `42` ile `int` ilişkisi gibi.

```csharp
Func<Kargo, bool> agirMi2 = k => k.AgirlikKg > 10;   // lambda → Func<Kargo, bool>

bool AgirMi3(Kargo k) => k.AgirlikKg > 10;
kargolar.Where(AgirMi3);                             // isimli metot da aynı tipe uyar
kargolar.Where(k => k.AgirlikKg > 10);               // lambda ile
```

`Where` parametre olarak `Func<T, bool>` ister; ona lambda ya da aynı imzaya sahip isimli bir metot verilebilir.

### event: abonelik sistemi

Event, delegate üzerine kurulu bir **publisher–subscriber** sistemidir: nesne "şu olunca haber vereceğim" der, ilgilenenler abone olur.

```csharp
public class Kargo
{
    public string TakipNo { get; init; } = "";
    public event Action<Kargo>? TeslimEdildi;

    public void TeslimEt()
    {
        Console.WriteLine($"{TakipNo} teslim edildi.");
        TeslimEdildi?.Invoke(this);   // abone yoksa null; ?. ile güvenli çağrı
    }
}

var kargo = new Kargo { TakipNo = "KRG1" };
kargo.TeslimEdildi += k => Console.WriteLine($"SMS gönderildi: {k.TakipNo}");
kargo.TeslimEdildi += k => Console.WriteLine($"Fatura kesildi: {k.TakipNo}");
kargo.TeslimEt();
```

Kargo sınıfı SMS'ten ya da faturadan habersizdir; yeni bir iş eklemek için kargoya dokunmadan yeni abone eklenir. `+=` abone olur, `-=` ayrılır.

**event ile düz delegate alanı farkı:** `event` kelimesi olmadan da (`public Action<Kargo>? TeslimEdildi;`) çalışır, ama:

1. Dışarıdan biri `=` ile atayarak diğer bütün aboneleri silebilir. `event` dışarıdan sadece `+=` / `-=`'e izin verir.
2. Dışarıdan biri olayı tetikleyebilir. `event`'te olayı sadece tanımlayan sınıf tetikler.

**Bellek sızıntısı:** Uzun ömürlü bir nesnenin event'ine abone olup `-=` ile ayrılmamak, abone nesnenin temizlenmesini engeller.

### C# event ile mesajlaşma (RabbitMQ / MassTransit) event'i

İkisi de publisher–subscriber fikrine dayanır ama farklı katmandadır:

| | C# event | MassTransit / RabbitMQ event |
| --- | --- | --- |
| Nerede çalışır? | Aynı uygulamanın içinde | Farklı uygulamalar / servisler arasında |
| Nasıl iletilir? | Doğrudan metot çağrısı | Mesaj kuyruğu üzerinden |
| Ne zaman işlenir? | Hemen, aynı anda | Sonra, ayrı olarak |
| Uygulama çökerse? | Haber kaybolur | Mesaj kuyrukta bekler |
| Benzetme | Şubede yüksek sesle bağırmak | Merkeze mektup göndermek |

### 💡 Mülakat notu: delegate, Func, Action ve event

**Delegate**, bir metodu değişkende ya da parametrede tutmayı sağlayan tiptir; davranışı dışarıdan vermeye yarar. Hazır tipler: **`Action<T>`** (dönüş yok), **`Func<T, TSonuc>`** (sonuç döner, son tip parametresi dönüş tipi), **`Predicate<T>`** (`bool` döner). `=>` ile yazılan isimsiz metot **lambda**'dır; lambda değer, `Func` onun tipidir. LINQ bu fikir üzerine kuruludur.

**Event**, delegate üzerine kurulu abonelik sistemidir; `+=` ile abone olunur, `-=` ile ayrılınır, yayınlayan abonelerini tanımaz. `event` kelimesi dışarıdan sadece `+=` / `-=`'e izin verir: dışarıdaki kod `=` ile aboneleri silemez ve olayı tetikleyemez. Ayrılmayı unutmak bellek sızıntısına yol açar.

C# event'i aynı uygulama içinde, anında ve kalıcı olmayan bir bildirimdir; RabbitMQ / MassTransit event'i servisler arasında, kuyrukta saklanan, asenkron işlenen bir mesajdır.

---

## 6. LINQ

### LINQ nedir?

Koleksiyonlar üzerinde döngü yazmadan filtreleme, dönüştürme, sıralama, gruplama yapmanın yolu.

```csharp
// Döngüyle
var sonuc = new List<string>();
foreach (var k in kargolar)
    if (k.Sehir == "Eskişehir")
        sonuc.Add(k.TakipNo);

// LINQ ile
var sonuc2 = kargolar
    .Where(k => k.Sehir == "Eskişehir")
    .Select(k => k.TakipNo)
    .ToList();
```

### En sık kullanılan metotlar

| Metot | Ne yapar? | Örnek |
| --- | --- | --- |
| `Where` | Filtreler | `.Where(k => k.AgirlikKg > 10)` |
| `Select` | Her elemanı dönüştürür | `.Select(k => k.TakipNo)` |
| `OrderBy` / `OrderByDescending` | Sıralar | `.OrderBy(k => k.OlusturmaTarihi)` |
| `ThenBy` / `ThenByDescending` | İkincil sıralama | `.OrderBy(k => k.Sehir).ThenByDescending(k => k.OlusturmaTarihi)` |
| `First` / `FirstOrDefault` | İlk elemanı alır | `.FirstOrDefault(k => k.TakipNo == "KRG1")` |
| `Single` / `SingleOrDefault` | Tek eleman bekler | `.SingleOrDefault(k => k.Id == id)` |
| `Any` | En az bir tane var mı? | `.Any(k => k.Durum == "Kayıp")` |
| `All` | Hepsi koşulu sağlıyor mu? | `.All(k => k.AgirlikKg < 30)` |
| `Count` | Kaç tane? | `.Count(k => k.Durum == "Teslim")` |
| `Sum` / `Average` / `Max` / `Min` | Hesaplama | `.Sum(k => k.AgirlikKg)` |
| `GroupBy` | Gruplar | `.GroupBy(k => k.Sehir)` |
| `Skip` / `Take` | Atla / al | `.Skip(20).Take(10)` |
| `ToList` / `ToArray` / `ToDictionary` | Koleksiyona çevirir | `.ToList()` |

**`First` / `FirstOrDefault` / `Single` / `SingleOrDefault`:**

- `First`: İlk elemanı döner; hiç yoksa exception.
- `FirstOrDefault`: Hiç yoksa varsayılan değer (class için `null`).
- `Single`: Tam olarak bir eleman bekler; hiç yoksa ya da birden fazlaysa exception.
- `SingleOrDefault`: Hiç yoksa varsayılan; birden fazlaysa yine exception.

Benzersiz olması gereken aramalarda (`Id`) `Single`, "herhangi biri" için `First`. Varlık kontrolünde `Count() > 0` yerine `Any()`: ilk eşleşmede durur.

**OrderBy yönü:** `OrderBy` her zaman artan sıradadır: tarihte **eskiden yeniye**, sayıda küçükten büyüğe, metinde A'dan Z'ye. Ters sıra için `OrderByDescending`. İkincil sıralama için ikinci bir `OrderBy` değil `ThenBy` kullanılır; ikinci `OrderBy` ilk sıralamayı bozar.

### Deferred execution (ertelenmiş çalıştırma)

LINQ sorgusu yazıldığı anda **çalışmaz**; sadece tarifi hazırlanır. Sonuç istendiğinde (`foreach`, `ToList`, `Count`, `First`, `Any`...) çalışır. Benzetme: tarif yazmak ile yemeği pişirmek.

```csharp
var kargolar = new List<Kargo>
{
    new() { TakipNo = "KRG1", Sehir = "Eskişehir" },
    new() { TakipNo = "KRG2", Sehir = "Ankara" }
};

var eskisehirdekiler = kargolar.Where(k => k.Sehir == "Eskişehir");   // çalışmadı
kargolar.Add(new() { TakipNo = "KRG3", Sehir = "Eskişehir" });

foreach (var k in eskisehirdekiler)   // ŞİMDİ çalışıyor
    Console.WriteLine(k.TakipNo);      // KRG1, KRG3
```

Sorguyu o anda çalıştırıp dondurmak için `ToList()`.

| Hemen çalışmayanlar (tarif) | Sorguyu çalıştıranlar |
| --- | --- |
| `Where`, `Select`, `OrderBy`, `GroupBy`, `Skip`, `Take` | `ToList`, `ToArray`, `ToDictionary`, `Count`, `Sum`, `Any`, `First`, `Single`, `foreach` |

Örnek:

```csharp
var sayilar = new List<int> { 1, 2, 3 };
var buyukler = sayilar.Where(s => s > 1);
sayilar.Add(4);
sayilar.Remove(2);
Console.WriteLine(buyukler.Count());            // 2 (liste o an: 1, 3, 4 → 3 ve 4)
Console.WriteLine(string.Join(", ", buyukler)); // 3, 4
```

**Tuzak (multiple enumeration):** Sorgu her kullanıldığında baştan çalışır. `Count()` sonra `First()` sonra `foreach` aynı işi üç kez yapar; veritabanında bu üç ayrı sorgu demektir. Birden fazla kullanılacaksa bir kez `ToList()` ile çalıştırılıp saklanır.

### IEnumerable ve IQueryable

- **`IEnumerable<T>`:** Sorgu **uygulamanın belleğinde**, C# ile çalışır.
- **`IQueryable<T>`:** Sorgu **SQL'e çevrilip veritabanında** çalışır.

Benzetme: Depoda 1 milyon kargo var, sadece Eskişehir'e gidenler lazım. IQueryable depoya "sadece Eskişehir'e gidenleri gönder" talimatı vermek; IEnumerable bütün kargoları şubeye taşıtıp kendin ayıklamak.

```csharp
// DOĞRU: filtre SQL'de
var eskisehir = _db.Kargolar.Where(k => k.Sehir == "Eskişehir").ToList();
// SQL: SELECT * FROM Kargolar WHERE Sehir = 'Eskişehir'

// YANLIŞ: önce bütün tablo belleğe, filtre C#'ta
var eskisehir2 = _db.Kargolar.ToList().Where(k => k.Sehir == "Eskişehir").ToList();
// SQL: SELECT * FROM Kargolar
```

**IEnumerable dönüş tipi neden sorguyu belleğe çeker?**

```csharp
public IEnumerable<Kargo> TumKargolar() => _db.Kargolar;
```

Bu metot tek başına hiçbir şey çekmez; içerideki nesne hâlâ `IQueryable`. Ama LINQ'te iki ayrı `Where` vardır: `Queryable.Where` (sorguya SQL olarak ekler) ve `Enumerable.Where` (C#'ta, bellekte filtreler). Derleyici hangisini çağıracağına nesnenin gerçek tipine değil, **değişkenin tanımlı tipine (etiketine)** bakarak karar verir.

```csharp
TumKargolar()                          // etiket: IEnumerable
    .Where(k => k.Sehir == "Eskişehir")// Enumerable.Where → sorguya hiçbir şey eklenmez
    .ToList();                         // EF Core: SELECT * FROM Kargolar (WHERE yok)
                                       // bütün satırlar gelir, filtre C#'ta yapılır
```

`AsEnumerable()` de aynı etkiyi yapar: veri çekmez, etiketi `IEnumerable`'a çevirir; sonraki LINQ metotları bellekte çalışır.

**Func ve Expression:** `IEnumerable.Where` bir `Func<T, bool>` alır (çalıştırılabilir metot). `IQueryable.Where` bir `Expression<Func<T, bool>>` alır (expression tree, yani ne yaptığının tarifi); EF Core bu tarifi SQL'e çevirir. Bu yüzden SQL'e çevrilemeyen ifadeler (kendi C# metodun) "çevrilemedi" hatası verir.

| | `IEnumerable<T>` | `IQueryable<T>` |
| --- | --- | --- |
| Sorgu nerede çalışır? | Uygulamanın belleğinde | Veritabanında (SQL) |
| Lambda ne olur? | `Func` | `Expression<Func>` (expression tree) |
| Ne zaman uygun? | Bellekteki koleksiyonlar | Veritabanı sorguları (EF Core) |
| Her C# kodu kullanılabilir mi? | Evet | Sadece SQL'e çevrilebilenler |
| Deferred execution | Var | Var |

### Sayfalama

```csharp
// A: DOĞRU
var sayfa = _db.Kargolar
    .OrderBy(k => k.OlusturmaTarihi)
    .Skip(20).Take(10)
    .ToList();
// SQL: ... ORDER BY OlusturmaTarihi OFFSET 20 ROWS FETCH NEXT 10 ROWS ONLY → 10 satır gelir

// B: YANLIŞ
var sayfa2 = _db.Kargolar
    .ToList()
    .OrderBy(k => k.OlusturmaTarihi)
    .Skip(20).Take(10)
    .ToList();
// SQL: SELECT * FROM Kargolar → 2 milyon satır gelir, bellekte sıralanır
```

`Skip` / `Take`'ten önce mutlaka `OrderBy` olmalı; yoksa veritabanı sırayı garanti etmez, kayıtlar sayfalar arasında tekrar edebilir ya da kaybolabilir.

### 💡 Mülakat notu: LINQ ve deferred execution

LINQ, koleksiyonlarda döngüsüz filtreleme (`Where`), dönüştürme (`Select`), sıralama (`OrderBy`, `ThenBy`), gruplama ve hesaplama (`Count`, `Sum`, `Any`) sağlar; metotlara verilen lambdalar `Func`'tır. `First` hiç eleman yoksa exception, `FirstOrDefault` varsayılan döner; `Single` tam olarak bir eleman bekler. Varlık kontrolünde `Any()` kullanılır.

**Deferred execution:** Sorgu yazıldığında değil, sonuç istendiğinde (`foreach`, `ToList`, `Count`, `First`) çalışır ve listenin o anki haline bakar. Her enumerate'te baştan çalışır; birden fazla kullanılacaksa `ToList()` ile bir kez çalıştırılıp saklanır.

### 💡 Mülakat notu: IEnumerable ile IQueryable farkı

`IEnumerable<T>` sorguyu bellekte, C# ile çalıştırır (`Func`). `IQueryable<T>` sorguyu expression tree olarak sağlayıcıya iletir; EF Core SQL'e çevirip veritabanında çalıştırır (`Expression<Func>`); sadece SQL'e çevrilebilen ifadeler kullanılabilir. İkisinde de deferred execution vardır.

Klasik hata: `ToList()`, `AsEnumerable()` ya da `IEnumerable` dönüş tipiyle sorguyu erken belleğe çekip filtreyi sonra yapmak. Derleyici `Where`'in hangisini (Queryable/Enumerable) çağıracağına değişkenin tanımlı tipine göre karar verir; tip `IEnumerable` ise filtre SQL'e eklenmez ve bütün tablo çekilir. Kural: `Where`, `OrderBy`, `Skip`, `Take`, `Select` `ToList()`'ten önce yazılır. Sayfalamada `Skip`/`Take`'ten önce mutlaka `OrderBy` olmalıdır.

---

## 7. Exception handling

### try, catch, finally

```csharp
try
{
    var kargo = await _repo.GetirAsync(takipNo);
    kargo.TeslimEt();
}
catch (KargoBulunamadiException ex)
{
    _logger.LogWarning("Kargo bulunamadı: {TakipNo}", takipNo);
}
catch (Exception ex)
{
    _logger.LogError(ex, "Beklenmeyen hata");
    throw;
}
finally
{
    Console.WriteLine("İşlem bitti");   // her durumda çalışır
}
```

- `catch` blokları yukarıdan aşağı denenir, ilk uyan seçilir: **özelden genele** sıralanır.
- `finally` hata olsa da olmasa da, `catch` içinde `return` ya da `throw` olsa da çalışır; kaynakları bırakmak için kullanılır.

### finally ve return

```csharp
int Hesapla()
{
    try
    {
        Console.WriteLine("try");
        return 1;
    }
    finally
    {
        Console.WriteLine("finally");
    }
}

Console.WriteLine(Hesapla());
// try
// finally
// 1
```

`return 1;` dönecek değeri hazırlar ama metottan hemen çıkılmaz; önce `finally` çalışır. Dışarıdaki `Console.WriteLine` metot bittikten sonra değeri yazar.

```csharp
int Hesapla2()
{
    int x = 1;
    try { return x; }       // dönüş kutusu ← x'in o anki değeri: 1 (kopyalandı)
    finally { x = 99; }     // x 99 oldu, dönüş kutusu hâlâ 1
}
Console.WriteLine(Hesapla2());   // 1
```

`return x;` o anda `x`'in değerini ayrı bir dönüş kutusuna kopyalar; `finally`'nin `x`'i değiştirmesi bu kopyayı etkilemez (değer tipi kopyası). Referans tipinde ise `finally` nesnenin içini değiştirirse değişiklik görünür, çünkü kopyalanan şey adrestir:

```csharp
Kargo Getir()
{
    var kargo = new Kargo { Durum = "Hazırlanıyor" };
    try { return kargo; }
    finally { kargo.Durum = "Teslim"; }
}
Console.WriteLine(Getir().Durum);   // "Teslim"
```

`finally` içinde `return` yazmak derleme hatasıdır.

### throw ile throw ex

```csharp
catch (Exception ex)
{
    _logger.LogError(ex, "Hata");
    throw;       // DOĞRU: stack trace korunur
}

catch (Exception ex)
{
    throw ex;    // YANLIŞ: stack trace bu satırdan yeniden başlar
}
```

**Stack trace**, hatanın hangi metotta, hangi satırda başladığını ve oraya hangi çağrılarla gelindiğini gösteren izdir. `throw ex;` hatanın asıl çıktığı yer bilgisini kaybettirir. Sarmak gerekirse orijinal `InnerException` olarak verilir:

```csharp
catch (SqlException ex)
{
    throw new KargoKaydedilemediException("Kargo kaydedilemedi", ex);
}
```

### Exception filtresi ve özel exception

```csharp
catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
{
    // sadece 404'ler
}

public class KargoBulunamadiException : Exception
{
    public string TakipNo { get; }
    public KargoBulunamadiException(string takipNo)
        : base($"Kargo bulunamadı: {takipNo}") => TakipNo = takipNo;
}
```

### Sık yapılan hatalar

1. **Hatayı yutmak** (`catch { }`): Hata görünmez olur. En azından loglanmalı; ele alınamıyorsa yeniden fırlatılmalı.
2. **Beklenen durumlar için exception:** Pahalıdır (stack trace toplanır). `int.TryParse`, `if`, `FirstOrDefault` kullanılmalı.
3. **Her yerde `catch (Exception)`:** Hata ancak onunla anlamlı bir şey yapılabiliyorsa yakalanır.
4. **Düz `new Exception(...)` fırlatmak:** Çağıranın hatayı ayırt etmesini zorlaştırır; anlamlı tip (`InvalidOperationException`, özel exception) tercih edilir.

### ASP.NET Core: global exception handling

```csharp
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
app.UseExceptionHandler();

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception ex, CancellationToken token)
    {
        var (status, baslik) = ex switch
        {
            KargoBulunamadiException => (404, "Kargo bulunamadı"),
            _                        => (500, "Beklenmeyen bir hata oluştu")
        };

        if (status == 500)
            logger.LogError(ex, "Beklenmeyen hata");

        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(new { title = baslik, status }, token);
        return true;
    }
}
```

Beklenmeyen hatalar tek merkezde yakalanır, loglanır; kullanıcıya stack trace ya da veritabanı mesajı gibi iç bilgiler gösterilmez.

### Result pattern

Beklenen, iş kuralı kaynaklı hatalarda exception yerine sonucu bir nesneyle döndürmek:

```csharp
public class Result<T>
{
    public bool BasariliMi { get; }
    public T? Deger { get; }
    public string? Hata { get; }

    private Result(bool basarili, T? deger, string? hata)
        => (BasariliMi, Deger, Hata) = (basarili, deger, hata);

    public static Result<T> Basarili(T deger) => new(true, deger, null);
    public static Result<T> Basarisiz(string hata) => new(false, default, hata);
}

public async Task<Result<Kargo>> TeslimEtAsync(string takipNo)
{
    var kargo = await _repo.GetirAsync(takipNo);
    if (kargo is null)
        return Result<Kargo>.Basarisiz("Kargo bulunamadı");
    if (kargo.Durum == Durum.Iptal)
        return Result<Kargo>.Basarisiz("İptal edilmiş kargo teslim edilemez");

    kargo.TeslimEt();
    return Result<Kargo>.Basarili(kargo);
}

var sonuc = await _servis.TeslimEtAsync(takipNo);
return sonuc.BasariliMi ? Ok(sonuc.Deger) : BadRequest(sonuc.Hata);
```

| | Exception (global handler ile) | Result pattern |
| --- | --- | --- |
| Hata görünür mü? | İmzada görünmez | İmzada açık: `Result<Kargo>` |
| Performans | Fırlatmak pahalı | Ucuz, normal dönüş |
| Kod | Sade, mutlu yol düz okunur | Her adımda kontrol, biraz daha fazla kod |
| Unutma riski | Yakalanmazsa global handler yakalar | Sonuç kontrol edilmezse hata sessizce geçebilir |
| Uygun durum | Beklenmeyen, sistem seviyesindeki hatalar | Beklenen, iş kuralı hataları |

Hazır kütüphaneler: FluentResults, ErrorOr.

### 💡 Mülakat notu: Exception handling

- `catch` blokları özelden genele sıralanır; `finally` her durumda (return/throw olsa da) çalışır. `return x;` dönecek değeri o anda kopyalar; `finally`'nin değer tipi değişkeni değiştirmesi sonucu etkilemez.
- `throw;` stack trace'i korur, `throw ex;` sıfırlar. Sarmak gerekirse `InnerException` verilir.
- `when` ile exception filtresi; `Exception`'dan türeyen özel exception'lar iş hatalarını ayırt etmeyi sağlar.
- Hatayı yutmamak, beklenen durumlarda exception kullanmamak (`TryParse`), anlamlı bir şey yapılamayan yerde yakalamamak.
- ASP.NET Core'da beklenmeyen hatalar global handler'da (`IExceptionHandler`) yakalanır, loglanır, `ProblemDetails` ile iç detay içermeyen cevap dönülür. Beklenen iş kuralı hatalarında Result pattern kullanılabilir: başarısızlık imzada görünür, exception maliyeti yoktur. "Beklenmeyen hataları global handler'da yakalayıp logluyorum, iş kuralı hatalarında Result pattern ile anlamlı mesaj dönüyorum."

---

## 8. Garbage collector, IDisposable ve using

### Garbage collector

Heap'teki, artık hiçbir değişkenin ulaşamadığı nesnelerin **belleğini** otomatik geri alır. **Deterministik değildir:** ne zaman çalışacağı belli değildir; .NET uygun gördüğünde (genelde bellek dolmaya başlayınca) çalışır.

**Deterministik** = ne zaman ve nasıl olacağı önceden kesin bilinen (alarm saati). **Deterministik değil** = ne zaman olacağı kesin bilinmeyen (çöp kamyonu: geleceği kesin, saati belli değil).

**Nesiller:**

- **Gen 0:** Yeni nesneler; en sık ve hızlı temizlenir.
- **Gen 1:** Bir temizliği atlatmışlar; ara bölge.
- **Gen 2:** Uzun yaşayanlar (cache, singleton); nadiren ve pahalı temizlenir.
- **Large Object Heap (LOH):** 85 KB'tan büyük nesneler; Gen 2 ile temizlenir.

Gözlem: çoğu nesne kısa ömürlüdür; GC bu yüzden bütün heap'i değil, çoğunlukla Gen 0'ı tarar. `GC.Collect()` elle çağrılmamalıdır.

### Unmanaged kaynaklar

GC yalnızca **managed belleği** temizler. Dosya, veritabanı bağlantısı, socket, işletim sistemi tanıtıcıları gibi **unmanaged kaynaklar** sınırlıdır ve GC'ye bırakılamaz; GC'nin zamanı belirsiz olduğu için beklerken bağlantı havuzu dolar. Benzetme: kuryenin işi bitince teslimat aracını hemen garaja getirmesi.

### IDisposable ve using

Böyle bir kaynak tutan sınıf `IDisposable` uygular; `Dispose()` kaynağı hemen bırakır.

```csharp
using (var dosya = new StreamWriter("rapor.txt"))
{
    dosya.WriteLine("KRG1;Teslim");
}   // hata olsa bile Dispose çağrılır (arka planda try/finally)

using var dosya2 = new StreamWriter("rapor2.txt");   // C# 8+: blok bitince dispose
```

Async kaynaklar için `await using` (`IAsyncDisposable`).

```csharp
// YANLIŞ: hata olursa Dispose'a hiç gelinmez, dosya açık kalır
public async Task<string> RaporOku()
{
    var okuyucu = new StreamReader("rapor.txt");
    var icerik = await okuyucu.ReadToEndAsync();
    if (string.IsNullOrEmpty(icerik))
        throw new Exception("Rapor boş");
    okuyucu.Dispose();
    return icerik;
}

// DOĞRU
public async Task<string> RaporOku2()
{
    using var okuyucu = new StreamReader("rapor.txt");
    var icerik = await okuyucu.ReadToEndAsync();
    if (string.IsNullOrEmpty(icerik))
        throw new InvalidOperationException("Rapor boş");
    return icerik;
}
```

### Dispose ile GC farkı

| | Garbage collector | Dispose |
| --- | --- | --- |
| Neyi temizler? | Nesnenin belleğini | Nesnenin tuttuğu dış kaynağı (dosya, bağlantı) |
| Kim yapar? | .NET, otomatik | Sen (ya da `using`) |
| Ne zaman? | Belli değil | Hemen, çağırdığın anda |
| Her nesne için mi? | Evet | Sadece `IDisposable` uygulayanlar |
| `using` ile ilgisi | Yok | `using` bunu çağırır |

Dispose nesneyi bellekten silmez; dosyayı kapatır, nesnenin belleğini sonra GC alır.

```csharp
var dosya = new StreamWriter("rapor.txt");  // nesne oluştu + dosya açıldı
dosya.Dispose();                            // DOSYA KAPANDI (hemen); nesne hâlâ bellekte
// ... daha sonra GC nesnenin belleğini temizler
```

### ASP.NET Core'da

- **DI'dan gelen nesneleri sen dispose etme** (`DbContext`); konteyner istek bitince eder. Kural: `new` ile sen oluşturduysan sen bırakırsın.
- **HttpClient tuzağı:** Her istekte `using var client = new HttpClient()` socket exhaustion'a yol açar (dispose edilen bağlantılar işletim sisteminde bir süre açık kalır). `IHttpClientFactory` (`AddHttpClient`) kullanılmalıdır.

```csharp
builder.Services.AddHttpClient<KargoApiClient>();

public class KargoApiClient(HttpClient client)
{
    public Task<string> DurumGetir(string takipNo)
        => client.GetStringAsync($"https://kargo-api.com/durum/{takipNo}");
}
```

### Finalizer

`~SinifAdi()`; GC nesneyi temizlemeden önce belirsiz bir zamanda çağırır. Dispose unutulursa son çare; GC'yi yavaşlatır, günlük kodda nadiren yazılır. **Dispose deterministik, finalizer değil.**

### .NET'te bellek sızıntısı

GC sadece **ulaşılamayan** nesneleri temizler. Ulaşılabilen ama gereksiz nesneler sızıntıya yol açar:

- Event'ten `-=` ile ayrılmamak
- Statik koleksiyonlarda sürekli biriktirmek (`static List<Kargo>`)
- Sınırsız bellek içi cache

### 💡 Mülakat notu: Garbage collector, IDisposable ve using

GC ulaşılamayan nesnelerin **belleğini** otomatik ve deterministik olmayan şekilde temizler; heap nesillere ayrılır (Gen 0/1/2, LOH). **Dispose** ise nesnenin tuttuğu **dış kaynağı** (dosya, bağlantı, socket) hemen bırakır; nesneyi bellekten silmez. Unmanaged kaynaklar sınırlı olduğu ve GC'nin zamanı belirsiz olduğu için Dispose ile bırakılır. `using` (hata olsa bile) Dispose'u çağırır, GC ile ilgisi yoktur.

Kendin `new` ile oluşturduğun `IDisposable`'ı sen bırakırsın; DI'dan gelenleri konteyner bırakır. `HttpClient`'ı her istekte oluşturup dispose etmek socket exhaustion'a yol açar; `IHttpClientFactory` kullanılır. Finalizer belirsiz zamanda çalışan son çaredir. .NET'te bellek sızıntısı ulaşılabilir kalan nesnelerle olur: ayrılmayan event aboneliği, statik koleksiyon, sınırsız cache.

---

## 9. Sık sorulan küçük detaylar

### `==` ve `Equals`

| Tip | `==` | `Equals` |
| --- | --- | --- |
| Değer tipleri | Değere bakar | Değere bakar |
| class (varsayılan) | Aynı nesne mi? | Aynı nesne mi? |
| string | İçeriğe bakar | İçeriğe bakar |
| record | Alanlara bakar | Alanlara bakar |
| struct | Tanımlanmadıkça yok | Alanlara bakar |

`Equals` yeniden yazılırsa **`GetHashCode` da yazılmalıdır**:

```csharp
public class Kargo
{
    public string TakipNo { get; init; } = "";
    public override bool Equals(object? obj) => obj is Kargo k && k.TakipNo == TakipNo;
    public override int GetHashCode() => TakipNo.GetHashCode();
}
```

`Dictionary` ve `HashSet` önce `GetHashCode` ile "rafı" bulur, sonra `Equals` ile karşılaştırır. Eşit nesnelerin hash kodu aynı olmazsa `HashSet` aynı kargoyu iki kez ekler, `Dictionary` kaydı bulamaz. Record'larda derleyici ikisini birlikte üretir.

### `const`, `readonly`, `static readonly`

```csharp
public class KargoAyarlari
{
    public const decimal KdvOrani = 0.20m;                           // derleme zamanı
    public readonly DateTime OlusturmaZamani;                        // constructor'da
    public static readonly string SirketAdi = Ayarlar.Oku("Sirket"); // ilk kullanımda bir kez

    public KargoAyarlari() => OlusturmaZamani = DateTime.UtcNow;
}
```

| | `const` | `readonly` | `static readonly` |
| --- | --- | --- | --- |
| Değer ne zaman belli olur? | Derleme zamanı | Nesne oluşturulurken (constructor) | Sınıf ilk kullanıldığında |
| Neye atanabilir? | Sadece sabitler | Her şey | Her şey |
| Her nesnede farklı olabilir mi? | Hayır | Evet | Hayır |

- `const` kullanıldığı yere kopyalanır; kütüphanede değişirse onu kullanan projeler yeniden derlenene kadar eski değeri kullanır. Değişebilecek değerler için `static readonly`.
- `readonly` alana sadece tanımlandığı yerde ya da constructor'da atanabilir:

```csharp
public class Kurye
{
    public const int MaksPaket = 30;
    public readonly string Ad;
    public Kurye(string ad) => Ad = ad;              // olur
    public void AdDegistir(string yeni) => Ad = yeni; // DERLEME HATASI
}
```

- `readonly` sığdır: `readonly List<Kargo> _liste` başka listeye çevrilemez ama içine eleman eklenebilir.

### Nullable

**`int?` / `DateTime?`** (nullable value type): Değer tipine "değer yok" anlamında `null` atanabilmesini sağlar (`Nullable<T>`).

```csharp
public DateTime? TeslimTarihi { get; set; }   // henüz teslim edilmediyse null
if (kargo.TeslimTarihi.HasValue)
    Console.WriteLine(kargo.TeslimTarihi.Value);
```

**Nullable reference types** (C# 8+, .NET 6'dan beri varsayılan açık): `string` null olamaz, `string?` olabilir. Derleyici null olabilecek değerin kontrolsüz kullanımında uyarı verir; amaç `NullReferenceException`'ı kod yazarken yakalamak.

```csharp
public string TakipNo { get; set; } = "";
public string? Aciklama { get; set; }
int uzunluk = kargo.Aciklama?.Length ?? 0;
```

### Null operatörleri

| Operatör | Ne yapar? | Örnek |
| --- | --- | --- |
| `?.` | Null değilse eriş, null ise `null` dön | `kargo?.Alici?.Ad` |
| `??` | Soldaki null ise sağdakini kullan | `aciklama ?? "Açıklama yok"` |
| `??=` | Null ise ata | `_cache ??= new()` |
| `!` | Null uyarısını susturur (kontrol yapmaz) | `kargo.Aciklama!.Length` |

```csharp
string aliciAdi = kargo?.Alici?.Ad ?? "Bilinmiyor";
```

`!` sadece uyarıyı susturur; değer gerçekten null'sa yine `NullReferenceException`.

### 💡 Mülakat notu: Sık sorulan küçük detaylar

- **`==` / `Equals`:** Değer tiplerinde değere, class'larda varsayılan olarak referansa bakar; string'te `==` içeriğe bakar; record'da ikisi de alanlara bakar. `Equals` yeniden yazılırsa `GetHashCode` da yazılmalı, yoksa `Dictionary`/`HashSet` bozulur.
- **`const` / `readonly` / `static readonly`:** `const` derleme zamanı sabiti, kullanıldığı yere kopyalanır. `readonly` tanımda ya da constructor'da bir kez atanır, her nesnede farklı olabilir. `static readonly` sınıf başına bir kez atanır. `readonly` sığdır.
- **Nullable:** `int?` değer tipine null verir (`HasValue`, `Value`). Nullable reference types (`string?`) null olabilecek değerleri işaretler, derleyici kontrolsüz kullanımda uyarır.
- **Null operatörleri:** `?.`, `??`, `??=`; `!` uyarıyı susturur ama kontrol yapmaz.

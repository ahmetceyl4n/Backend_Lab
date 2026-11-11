# C# dil temelleri: mülakat soruları

Önce soruyu kendi kelimelerinle, sesli cevapla; sonra "Cevap"a tıklayıp karşılaştır. Kod sorularında çıktıyı çalıştırmadan tahmin et.

İşaretler: ⭐ = çok sık sorulur, 🧩 = kod / tuzak sorusu

---

## 1. Değer tipleri ve referans tipleri

### ⭐ 1.1 Değer tipi ile referans tipi arasındaki fark nedir?

<details>
<summary>Cevap</summary>

Değer tipi (`int`, `bool`, `decimal`, `DateTime`, `enum`, `struct`) değişkende verinin kendisini tutar; atamada ve metoda geçişte kopyalanır. Referans tipi (`class`, `string`, dizi, `List<T>`, `object`) değişkende nesnenin adresini tutar; atamada adres kopyalanır, iki değişken aynı nesneyi gösterir ve birindeki değişiklik diğerinden görünür.

</details>

### 1.2 "Değer tipleri stack'te, referans tipleri heap'te durur." Bu doğru mu?

<details>
<summary>Cevap</summary>

Tam doğru değil. Referans tipi nesneler heap'tedir. Değer tipi ise bulunduğu yerde saklanır: metodun yerel değişkeniyse stack'te, bir class'ın alanıysa nesneyle birlikte heap'te durur.

</details>

### ⭐ 🧩 1.3 Çıktı ne olur?

```csharp
class Kargo { public string Durum { get; set; } = "Hazırlanıyor"; }

void Degistir(int sayi, Kargo kargo)
{
    sayi = 99;
    kargo.Durum = "Teslim";
    kargo = new Kargo { Durum = "Yeni" };
}

int s = 1;
var k = new Kargo();
Degistir(s, k);
Console.WriteLine(s);
Console.WriteLine(k.Durum);
```

<details>
<summary>Cevap</summary>

`1` ve `Teslim`. `int` kopyalanarak geçtiği için dışarıdaki `s` değişmez. `kargo.Durum = "Teslim"` aynı nesnenin içini değiştirdiği için dışarıdan görünür. `kargo = new Kargo()` sadece metodun kendi kopyası olan değişkeni başka nesneye çevirir; dışarıdaki `k` hâlâ eski nesneyi gösterir.

</details>

### 1.4 `ref` ne işe yarar? `out` ve `in` ile farkı nedir?

<details>
<summary>Cevap</summary>

`ref` olmadan metoda değişkenin içeriğinin kopyası gider; `ref` ile değişkenin kendisi gider, metodun yaptığı atama dışarıdaki değişkene yansır (`kargo = new Kargo()` dışarıyı da değiştirir). `out`, `ref` gibidir ama metodun değer ataması zorunludur (`int.TryParse(metin, out var sonuc)`). `in` kopyalamadan ama salt okunur geçirir; büyük struct'larda performans için kullanılır.

</details>

### 🧩 1.5 Çıktı ne olur?

```csharp
struct Konum { public int X; }
class Kurye { public Konum Konum; }

var kurye1 = new Kurye();
kurye1.Konum.X = 5;
var kurye2 = kurye1;
kurye2.Konum.X = 10;
Konum k = kurye1.Konum;
k.X = 99;

Console.WriteLine(kurye1.Konum.X);
Console.WriteLine(kurye2.Konum.X);
Console.WriteLine(k.X);
```

<details>
<summary>Cevap</summary>

`10`, `10`, `99`. `Kurye` class olduğu için `kurye2 = kurye1` adres kopyalar; tek nesne var, ikisi de 10 görür. `Konum` struct olduğu için `k = kurye1.Konum` bağımsız bir kopya oluşturur; `k.X = 99` kuryeyi etkilemez.

</details>

### ⭐ 1.6 Boxing ve unboxing nedir? Neden önemlidir?

<details>
<summary>Cevap</summary>

Boxing, bir değer tipinin `object`'e (ya da bir interface'e) atanınca heap'te bir kutuya konmasıdır; unboxing geri çıkarmaktır (`int geri = (int)kutu`). Her boxing yeni bir heap nesnesi ve garbage collector yükü demektir. `ArrayList` gibi `object` tabanlı koleksiyonlar her elemanı kutular; `List<int>` gibi generic koleksiyonlar kutulamaz.

</details>

---

## 2. class, struct, record, sealed, string

### ⭐ 2.1 class, struct ve record arasındaki farklar nelerdir?

<details>
<summary>Cevap</summary>

- **class:** Referans tipi; atamada nesne paylaşılır; `==` ve `Equals` varsayılan olarak referansa bakar; kalıtım destekler. Entity ve servisler için.
- **struct:** Değer tipi; atamada tamamen kopyalanır; `Equals` alanlara bakar, `==` tanımlanmadıkça yoktur; kalıtım yok. Küçük, kısa ömürlü, değişmeyen veriler için (`DateTime`, `Guid`, koordinat).
- **record:** Referans tipi ama eşitliği değere göredir (derleyici `Equals`, `==`, `GetHashCode` üretir); positional yazımda değişmezdir, değişiklik `with` ile yeni kopya oluşturarak yapılır. DTO, kuyruk mesajı, value object için.

</details>

### 🧩 2.2 Çıktı ne olur?

```csharp
public record KargoDto(string TakipNo, string Durum);
public class KargoEntity { public string TakipNo { get; set; } = ""; }

var a = new KargoDto("KRG1", "Dağıtımda");
var b = new KargoDto("KRG1", "Dağıtımda");
var c = new KargoEntity { TakipNo = "KRG1" };
var d = new KargoEntity { TakipNo = "KRG1" };

Console.WriteLine(a == b);
Console.WriteLine(c == d);
```

<details>
<summary>Cevap</summary>

`True` ve `False`. Record'da eşitlik alanlara göredir. Class'ta `==` iki değişkenin aynı nesneyi gösterip göstermediğine bakar; içerik aynı olsa da iki ayrı nesne oldukları için `False`.

</details>

### 2.3 Şu üç durumda class, struct, record'dan hangisini seçersin?

1. API'nin "kargo oluştur" isteğinde gelen veri
2. Veritabanında saklanan, durumu zamanla değişen `Kargo`
3. Saniyede binlerce kez oluşturulan kurye konumu (enlem, boylam)

<details>
<summary>Cevap</summary>

1. **record:** Veri taşıyıcı (DTO); kimliği yok, içeriği önemli, değişmemeli.
2. **class:** Kimliği olan ve değişen entity; EF Core referans eşitliğine güvenir.
3. **struct** (`readonly struct` ya da `readonly record struct`): Küçük, kısa ömürlü; class olsaydı her biri heap nesnesi olup GC'ye yük bindirirdi.

</details>

### 2.4 Mutable (değiştirilebilir) struct neden sorun çıkarır?

<details>
<summary>Cevap</summary>

Struct her atamada kopyalandığı için bir kopyayı değiştirip asıl veriyi değiştirdiğini sanabilirsin (ör. bir listeden alınan struct'ı değiştirmek listedekini değiştirmez). Bu yüzden struct'lar değişmez yazılır: `readonly struct`. Ayrıca büyük struct'lar her kopyalamada maliyet getirir.

</details>

### 2.5 `with` ifadesi ne yapar? Record'un değişmezliği tam mıdır?

<details>
<summary>Cevap</summary>

`with`, orijinali değiştirmeden bir ya da birkaç alanı farklı yeni bir kopya oluşturur: `var c = a with { Durum = "Teslim" };`. Değişmezlik sığdır: record'un içinde `List<string>` gibi bir alan varsa listenin içi yine değiştirilebilir.

</details>

### 2.6 `sealed` ne demektir? Neden kullanılır?

<details>
<summary>Cevap</summary>

`sealed` class'tan kalıtım alınamaz; `sealed override` metot alt sınıflarda tekrar değiştirilemez. Tasarım niyetini göstermek ("kalıtım için tasarlanmadı"), davranışın alt sınıflar tarafından bozulmasını engellemek ve küçük bir performans kazancı (JIT sanal çağrıları doğrudan çağrıya çevirebilir) için kullanılır. `abstract` mutlaka türetilmeli, `sealed` türetilemez, `static` ne oluşturulur ne türetilir.

</details>

### ⭐ 2.7 String referans tipi; neden değer tipi gibi davranıyor?

<details>
<summary>Cevap</summary>

String değişmezdir (immutable): oluşturulduktan sonra içeriği değiştirilemez, `ToUpper`, `Replace`, `+=` gibi her işlem yeni bir string üretir. İçeriği değiştiren bir işlem olmadığı için paylaşmak güvenlidir. Ayrıca `==` string'te içeriği karşılaştıracak şekilde yeniden tanımlanmıştır.

</details>

### 🧩 2.8 Çıktı ne olur?

```csharp
string a = "ahmet";
string b = a;
b = "mehmet";
b.ToUpper();
Console.WriteLine(a);
Console.WriteLine(b);
```

<details>
<summary>Cevap</summary>

`ahmet` ve `mehmet`. `b = "mehmet"` nesneyi değil, `b`'nin gösterdiği yeri değiştirir; `a` hâlâ "ahmet" nesnesini gösterir. `b.ToUpper()` yeni bir string döndürür ama bir yere atanmadığı için `b` değişmez.

</details>

### 2.9 Döngüde string birleştirmek neden yavaştır?

<details>
<summary>Cevap</summary>

String değişmez olduğu için her `+=` yeni bir string oluşturur ve eski içeriği oraya kopyalar; binlerce turda binlerce ara string oluşur ve hepsi çöp olur. Çözüm `StringBuilder`: içeriği tek, değiştirilebilir bir tamponda biriktirir.

</details>

### 2.10 `string` neden sealed?

<details>
<summary>Cevap</summary>

String'in değişmezliği sözlük anahtarı olarak kullanımı, string havuzu ve güvenlik kontrolleri gibi birçok yerde varsayılır. Bir alt sınıf onu değiştirilebilir yapabilseydi bu varsayımların hepsi bozulurdu.

</details>

---

## 3. interface ve abstract class

### ⭐ 3.1 Interface ile abstract class arasındaki fark nedir? Hangisini ne zaman kullanırsın?

<details>
<summary>Cevap</summary>

Interface bir sözleşmedir: tipin ne yapabileceğini söyler; bir class birden fazla interface uygulayabilir; alan ve constructor içermez, veri tutmaz; struct'lar da uygulayabilir. Abstract class aynı aileden sınıfların ortak verisini ve kodunu taşıyan, doğrudan oluşturulamayan temel sınıftır; bir class yalnızca bir sınıftan türeyebilir; alan, constructor, `protected` üye içerir; `abstract` metotlar zorunlu, `virtual` metotlar isteğe bağlı olarak override edilir.

Seçim: farklı türlerde ortak yetenek ya da değiştirilebilir bağımlılık (DI, test) → interface. "Bir türüdür" ilişkisi ve ortak veri/kod → abstract class. İkisi birlikte de kullanılır.

</details>

### ⭐ 3.2 C# 8'den beri interface'e gövdeli metot yazılabiliyor. Abstract class'a hâlâ neden ihtiyaç var?

<details>
<summary>Cevap</summary>

Default interface method interface'e davranış ekledi ama interface hâlâ veri tutamaz: instance alanı ve constructor içeremez. Her nesneye ait `TakipNo` gibi veriler ve onları başlatan mantık interface'te yazılamaz; abstract class bunları tek yerde toplar. Amaçları da farklıdır: default method'un amacı mevcut uygulamaları bozmadan interface'e metot eklemek (geriye dönük uyumluluk), abstract class'ın amacı aynı aileye ortak bir temel vermektir.

</details>

### 3.3 `abstract` metot ile `virtual` metot arasındaki fark nedir?

<details>
<summary>Cevap</summary>

`abstract` metodun gövdesi yoktur ve alt sınıf onu yazmak zorundadır. `virtual` metodun gövdesi (varsayılan davranışı) vardır; alt sınıf isterse `override` ile değiştirir.

</details>

### 3.4 Abstract class'tan neden `new` ile nesne oluşturulamaz?

<details>
<summary>Cevap</summary>

Çünkü eksiktir: `abstract` metotlarının (ör. `TemelUcret`) nasıl çalışacağını bilmez. Ancak bir alt sınıf bu eksikleri tamamladığında kullanılabilir hale gelir.

</details>

### 3.5 Gövdeli ve gövdesiz metot ne demektir?

<details>
<summary>Cevap</summary>

Metodun imzası (adı, parametreleri, dönüş tipi) ve gövdesi (süslü parantez içindeki kod) vardır. Gövdesiz metot sadece imzadan oluşur ve `;` ile biter; ne yapacağını başkası yazar (interface metotları, `abstract` metotlar). Gövdeli metot ne yapacağını kendisi içerir; `=>` ile yazılan kısa hali de gövdelidir.

</details>

---

## 4. Generics

### ⭐ 4.1 Generics nedir, ne kazandırır?

<details>
<summary>Cevap</summary>

Tipi bir parametre olarak vermeyi sağlar: sınıf ya da metot bir kez yazılır, tip kullanıldığı yerde belirtilir (`List<Kargo>`, `Cache<int>`). `object` kullanmaya göre üç kazancı vardır: tip güvenliği (yanlış tip derlerken yakalanır), cast gerekmez, değer tiplerinde boxing olmaz.

</details>

### 🧩 4.2 Bu kod derlenir mi? Derlenirse çalışırken ne olur?

```csharp
var liste = new List<object>();
liste.Add(new Kargo());
liste.Add(new Kurye());
Kargo k = (Kargo)liste[1];
```

<details>
<summary>Cevap</summary>

Derlenir, çünkü her tip bir `object`'tir ve açık cast derleyiciye "bana güven" demektir. Çalışırken `InvalidCastException` fırlatır: `liste[1]` bir `Kurye`, `Kargo`'ya çevrilemez. `List<Kargo>` kullanılsaydı `Kurye` eklemek derleme hatası olurdu.

</details>

### 4.3 Cast nedir? `(T)x`, `as` ve `is` farkı nedir?

<details>
<summary>Cevap</summary>

Cast bir değeri başka tipe çevirmektir; veri kaybı yoksa örtük (otomatik), varsa açık (parantezle) yapılır. `(Kargo)x` tip uymazsa `InvalidCastException` fırlatır; `x as Kargo` uymazsa `null` döner; `x is Kargo k` kontrol eder ve uyuyorsa aynı anda `k`'ya çevirir (pattern matching).

</details>

### ⭐ 🧩 4.4 Bu metot derlenir mi? Derlenmezse nasıl düzeltirsin?

```csharp
public static T EnBuyuk<T>(T a, T b) => a > b ? a : b;
```

<details>
<summary>Cevap</summary>

Derlenmez. `T` kısıtlanmadığı için derleyici `T`'nin `>` ile karşılaştırılabileceğini bilmez (`T` bir `Kargo` olabilir). Çözüm bir constraint ile yetenek garantisi vermek:

```csharp
public static T EnBuyuk<T>(T a, T b) where T : IComparable<T>
    => a.CompareTo(b) > 0 ? a : b;
```

</details>

### 4.5 Generic constraint'lere (`where`) örnek verir misin?

<details>
<summary>Cevap</summary>

`where T : class` (referans tipi), `where T : struct` (değer tipi), `where T : new()` (parametresiz constructor, `new T()` için), `where T : IEntity` (interface uygulamalı), `where T : Gonderi` (bu sınıftan türemeli), `where T : notnull`. Örnek: `class Repository<T> where T : class, IEntity`; constraint sayesinde metodun içinde `kayit.Id` yazılabilir.

</details>

### 4.6 `IEnumerable<string>`, `IEnumerable<object>`'e atanabiliyor ama `List<string>`, `List<object>`'e atanamıyor. Neden?

<details>
<summary>Cevap</summary>

Kovaryans. `IEnumerable<out T>` sadece okuma yapar; daha genel tipe çevirmek güvenlidir. `List<T>`'ye yazma yapılabilir; `List<object>` olarak görülseydi string listesine `int` eklenebilir ve tip güvenliği bozulurdu.

</details>

---

## 5. delegate, Func, Action ve event

### ⭐ 5.1 Delegate nedir?

<details>
<summary>Cevap</summary>

Bir metodu değişkende ya da parametrede tutmayı, sonra çağırmayı sağlayan tiptir. Davranışı dışarıdan vermeye yarar (ör. filtreleme koşulunu parametre olarak vermek). LINQ bu fikir üzerine kuruludur.

</details>

### ⭐ 5.2 `Func` ile `Action` arasındaki fark nedir? `Predicate` nedir?

<details>
<summary>Cevap</summary>

`Action<T>` parametre alır, bir şey yapar, değer döndürmez. `Func<T, TSonuc>` değer döndürür; son tip parametresi dönüş tipidir (`Func<int, int, int>` iki `int` alır, `int` döner). `Predicate<T>` bir kontrol yapıp `bool` döner.

</details>

### 🧩 5.3 Çıktı ne olur?

```csharp
Func<int, int, int> topla = (a, b) => a + b;
Action<int> yazdir = sayi => Console.WriteLine(sayi);
yazdir(topla(3, 4));
```

<details>
<summary>Cevap</summary>

`7`. `topla(3, 4)` 7 döndürür, o da `yazdir`'a verilir.

</details>

### 5.4 Lambda nedir? Lambda ile `Func` arasındaki ilişki nedir?

<details>
<summary>Cevap</summary>

Lambda (`k => k.AgirlikKg > 10`) kısa, isimsiz metot yazmanın sözdizimidir. `Func` ise o metodun tipidir; lambda değer, `Func` tipi. `Where` bir `Func<T, bool>` ister; ona lambda ya da aynı imzaya sahip isimli bir metot verilebilir.

</details>

### ⭐ 5.5 Event ile düz delegate alanı arasındaki fark nedir?

<details>
<summary>Cevap</summary>

`event` kelimesi dışarıdan sadece `+=` (abone ol) ve `-=` (ayrıl) yapılmasına izin verir. Düz delegate alanında dışarıdaki biri `=` ile atayarak diğer bütün aboneleri silebilir ya da olayı kendisi tetikleyebilir. Event'te olayı yalnızca tanımlayan sınıf tetikler.

</details>

### 5.6 Event kullanırken bellek sızıntısı nasıl olur?

<details>
<summary>Cevap</summary>

Uzun ömürlü bir nesnenin event'ine abone olup `-=` ile ayrılmamak. Yayınlayan nesne abone listesinde aboneyi tuttuğu için abone nesneye ulaşılabilir kalır ve garbage collector onu temizleyemez.

</details>

### 5.7 C# event'i ile RabbitMQ / MassTransit ile yayınlanan event arasındaki fark nedir?

<details>
<summary>Cevap</summary>

İkisi de publisher–subscriber fikrine dayanır. C# event'i aynı uygulamanın içinde çalışır, aboneler doğrudan ve hemen çağrılır, hiçbir yerde saklanmaz (uygulama çökerse kaybolur). Mesajlaşma event'i farklı servisler arasında çalışır, broker'da saklanır, dinleyen kapalıysa açılınca alır ve asenkron işlenir.

</details>

---

## 6. LINQ

### ⭐ 6.1 Deferred execution nedir?

<details>
<summary>Cevap</summary>

LINQ sorgusu yazıldığı anda çalışmaz; sadece tarifi hazırlanır. Sonuç istendiğinde (`foreach`, `ToList`, `Count`, `First`, `Any`) çalışır ve kaynağın o anki haline bakar. `Where`, `Select`, `OrderBy` sorguyu çalıştırmaz; `ToList`, `Count`, `First` gibi metotlar çalıştırır.

</details>

### ⭐ 🧩 6.2 Çıktı ne olur?

```csharp
var sayilar = new List<int> { 1, 2, 3 };
var buyukler = sayilar.Where(s => s > 1);
sayilar.Add(4);
sayilar.Remove(2);
Console.WriteLine(string.Join(", ", buyukler));
```

<details>
<summary>Cevap</summary>

`3, 4`. Sorgu `Where` satırında değil, `string.Join` elemanları istediğinde çalışır; o anda liste `1, 3, 4`'tür.

</details>

### 6.3 Aynı LINQ sorgusunu birden fazla kez kullanmanın sakıncası nedir?

<details>
<summary>Cevap</summary>

Sorgu her enumerate edildiğinde baştan çalışır (multiple enumeration). `Count()` sonra `foreach` aynı işi iki kez yapar; veritabanı sorgusunda bu iki ayrı sorgu demektir. Birden fazla kullanılacaksa bir kez `ToList()` ile çalıştırılıp saklanır.

</details>

### ⭐ 6.4 `First`, `FirstOrDefault`, `Single`, `SingleOrDefault` farkı nedir?

<details>
<summary>Cevap</summary>

`First` ilk elemanı döner, hiç yoksa exception fırlatır. `FirstOrDefault` hiç yoksa varsayılan değer (class için `null`) döner. `Single` tam olarak bir eleman bekler; hiç yoksa ya da birden fazlaysa exception. `SingleOrDefault` hiç yoksa varsayılan döner, birden fazlaysa exception. Benzersiz olması gereken aramalarda (`Id`) `Single` kullanılır.

</details>

### 6.5 Bir koleksiyonda eleman olup olmadığını kontrol etmek için `Count() > 0` mı `Any()` mi?

<details>
<summary>Cevap</summary>

`Any()`. İlk eşleşmeyi bulunca durur; `Count()` hepsini sayar. Veritabanında da `Any()` daha verimli bir SQL (`EXISTS`) üretir.

</details>

### ⭐ 6.6 `IEnumerable` ile `IQueryable` arasındaki fark nedir?

<details>
<summary>Cevap</summary>

`IEnumerable<T>` sorguyu uygulamanın belleğinde, C# ile çalıştırır; metotlara `Func` verilir. `IQueryable<T>` sorguyu expression tree (`Expression<Func>`) olarak sağlayıcıya iletir; EF Core bunu SQL'e çevirip veritabanında çalıştırır. Bu yüzden `IQueryable`'da sadece SQL'e çevrilebilen ifadeler kullanılabilir. İkisinde de deferred execution vardır.

</details>

### ⭐ 🧩 6.7 Bu iki kodun farkı nedir? Hangisini kullanırsın?

```csharp
// A
var a = _db.Kargolar.Where(k => k.Sehir == "Eskişehir").ToList();

// B
var b = _db.Kargolar.ToList().Where(k => k.Sehir == "Eskişehir").ToList();
```

<details>
<summary>Cevap</summary>

A. A'da filtre SQL'e çevrilir (`WHERE Sehir = 'Eskişehir'`), sadece gerekenler gelir. B'de `ToList()` önce bütün tabloyu belleğe çeker (`SELECT * FROM Kargolar`), filtre sonra C#'ta yapılır; büyük tablolarda bellek ve performans sorunu yaratır.

</details>

### 🧩 6.8 Bu metotta `ToList` yok; yine de tablonun tamamı belleğe gelir mi?

```csharp
public IEnumerable<Kargo> TumKargolar() => _db.Kargolar;

var sonuc = TumKargolar().Where(k => k.Sehir == "Eskişehir").ToList();
```

<details>
<summary>Cevap</summary>

Evet. Metot tek başına hiçbir şey çekmez; ama dönüş tipi `IEnumerable` olduğu için derleyici `Queryable.Where` yerine `Enumerable.Where`'i seçer (karar değişkenin tanımlı tipine göre verilir). Filtre sorguya eklenmez; `ToList()` çağrılınca EF Core `SELECT * FROM Kargolar` çalıştırır, bütün satırlar gelir ve filtre C#'ta yapılır. `AsEnumerable()` de aynı etkiyi yapar.

</details>

### 🧩 6.9 2 milyon kargoluk tabloda sayfalama: hangisi doğru, neden?

```csharp
// A
_db.Kargolar.OrderBy(k => k.OlusturmaTarihi).Skip(20).Take(10).ToList();

// B
_db.Kargolar.ToList().OrderBy(k => k.OlusturmaTarihi).Skip(20).Take(10).ToList();
```

<details>
<summary>Cevap</summary>

A. Sıralama ve sayfalama SQL'de yapılır (`ORDER BY ... OFFSET 20 ROWS FETCH NEXT 10 ROWS ONLY`), sadece 10 satır gelir. B 2 milyon satırı belleğe çekip orada sıralar. Ayrıca `Skip`/`Take`'ten önce `OrderBy` şarttır; yoksa veritabanı sırayı garanti etmez ve kayıtlar sayfalar arasında tekrar edebilir ya da kaybolabilir.

</details>

### 6.10 `OrderBy` hangi yönde sıralar? İki alana göre nasıl sıralarsın?

<details>
<summary>Cevap</summary>

`OrderBy` artan sıradadır: tarihte eskiden yeniye, sayıda küçükten büyüğe, metinde A'dan Z'ye; ters yön için `OrderByDescending`. İkinci alan için `ThenBy` / `ThenByDescending` kullanılır; ikinci bir `OrderBy` ilk sıralamayı bozar: `.OrderBy(k => k.Sehir).ThenByDescending(k => k.OlusturmaTarihi)`.

</details>

### 6.11 `IQueryable` sorgusunda kendi C# metodunu çağırırsan ne olur?

<details>
<summary>Cevap</summary>

EF Core o metodu SQL'e çeviremediği için "ifade çevrilemedi" hatası verir. Ya ifade SQL'e çevrilebilecek şekilde yazılır ya da önce veritabanında filtrelenip, kalan küçük sonuç üzerinde bellekte metot uygulanır.

</details>

---

## 7. Exception handling

### ⭐ 7.1 `throw` ile `throw ex` arasındaki fark nedir?

<details>
<summary>Cevap</summary>

`throw;` orijinal stack trace'i korur. `throw ex;` stack trace'i o satırdan yeniden başlatır; hatanın asıl çıktığı yer bilgisi kaybolur. Hata başka bir exception'a sarılacaksa orijinal `InnerException` olarak verilir: `throw new XException("...", ex);`.

</details>

### ⭐ 🧩 7.2 Çıktı ne olur, hangi sırayla?

```csharp
int Hesapla()
{
    try { Console.WriteLine("try"); return 1; }
    finally { Console.WriteLine("finally"); }
}
Console.WriteLine(Hesapla());
```

<details>
<summary>Cevap</summary>

`try`, `finally`, `1`. `return 1;` dönecek değeri hazırlar ama metottan çıkmadan önce `finally` çalışır; dışarıdaki `Console.WriteLine` metot bitince değeri yazar.

</details>

### 🧩 7.3 Bu metot ne döndürür?

```csharp
int Hesapla()
{
    int x = 1;
    try { return x; }
    finally { x = 99; }
}
```

<details>
<summary>Cevap</summary>

`1`. `return x;` o anda `x`'in değerini ayrı bir dönüş kutusuna kopyalar; `finally`'nin `x`'i 99 yapması bu kopyayı etkilemez. Referans tipinde `finally` nesnenin içini değiştirseydi değişiklik görünürdü, çünkü kopyalanan şey adres olurdu.

</details>

### 7.4 `catch` bloklarının sırası neden önemlidir?

<details>
<summary>Cevap</summary>

C# yukarıdan aşağı ilk uyan `catch`'i seçer. `catch (Exception)` en üste yazılırsa her şeyi o yakalar ve alttakiler hiç çalışmaz (derleyici hata verir). Bu yüzden özelden genele sıralanır.

</details>

### 7.5 `finally` ne zaman çalışır? Ne için kullanılır?

<details>
<summary>Cevap</summary>

Hata olsa da olmasa da, `catch` içinde `return` ya da `throw` olsa da çalışır (sürecin tamamen öldürülmesi gibi uç durumlar hariç). Dosya kapatmak, kilit bırakmak gibi kaynak temizliği için kullanılır; `using` arka planda `try/finally`'ye çevrilir.

</details>

### 7.6 Exception'ları normal akış kontrolü için kullanmak neden kötüdür?

<details>
<summary>Cevap</summary>

Exception fırlatmak pahalıdır (stack trace toplanır) ve kodu okunmaz hale getirir. Beklenen durumlarda (`int.Parse` yerine `int.TryParse`, `First` yerine `FirstOrDefault` + kontrol) normal kontrol kullanılır; exception beklenmeyen durumlar içindir.

</details>

### ⭐ 7.7 ASP.NET Core'da hata yönetimini nasıl yaparsın?

<details>
<summary>Cevap</summary>

Beklenmeyen hataları her endpoint'te değil, tek merkezde, global exception handler (`IExceptionHandler` ya da middleware) ile yakalarım; loglarım ve kullanıcıya iç detay (stack trace, veritabanı mesajı) içermeyen bir cevap (`ProblemDetails`, 500) dönerim. Özel exception'ları burada uygun HTTP koduna (404, 409) çeviririm. İş kuralı kaynaklı beklenen hatalarda ise Result pattern ile anlamlı mesajlar dönerim.

</details>

### 7.8 Result pattern nedir? Exception'a göre artısı ve eksisi nedir?

<details>
<summary>Cevap</summary>

Beklenen hata durumlarında exception fırlatmak yerine başarılı/başarısız bilgisini taşıyan bir nesne (`Result<T>`) döndürmektir. Artıları: başarısızlık ihtimali imzada görünür, çağıran kontrol etmek zorunda kalır, exception maliyeti yoktur. Eksileri: kod biraz uzar, sonucu kontrol etmeyi unutmak hatayı sessizce geçirebilir. Beklenen iş kuralı hataları için Result, beklenmeyen sistem hataları için exception + global handler kullanılır. Kütüphaneler: FluentResults, ErrorOr.

</details>

### 7.9 Boş `catch { }` bloğunun sakıncası nedir?

<details>
<summary>Cevap</summary>

Hatayı yutar: işlem başarısız olduğu halde uygulama her şey yolundaymış gibi devam eder ve hata bulunması çok zor hale gelir. Hata yakalandıysa en azından loglanmalı; ele alınamıyorsa yeniden fırlatılmalıdır.

</details>

---

## 8. Garbage collector, IDisposable ve using

### ⭐ 8.1 Garbage collector nasıl çalışır?

<details>
<summary>Cevap</summary>

Heap'teki, artık hiçbir yerden ulaşılamayan nesnelerin belleğini otomatik geri alır; ne zaman çalışacağı belli değildir (deterministik değil). Çoğu nesnenin kısa ömürlü olduğu gözlemine dayanarak heap'i nesillere ayırır: Gen 0 (yeni, sık ve hızlı temizlenir), Gen 1 (ara), Gen 2 (uzun yaşayanlar, nadiren ve pahalı temizlenir). 85 KB'tan büyük nesneler Large Object Heap'e gider. `GC.Collect()` elle çağrılmamalıdır.

</details>

### ⭐ 8.2 `Dispose` ile garbage collector arasındaki fark nedir?

<details>
<summary>Cevap</summary>

GC nesnelerin belleğini temizler; otomatiktir ve zamanı belirsizdir. Dispose nesnenin tuttuğu dış kaynağı (dosya, veritabanı bağlantısı, socket) bırakır; kod tarafından çağrılır ve hemen gerçekleşir. Dispose nesneyi bellekten silmez; nesnenin belleğini sonra GC alır. Sınırlı kaynaklar GC'nin belirsiz zamanına bırakılamayacağı için Dispose ile hemen bırakılır.

</details>

### ⭐ 8.3 `using` ne işe yarar?

<details>
<summary>Cevap</summary>

`IDisposable` bir nesnenin `Dispose` metodunun, blok bittiğinde hata olsa bile çağrılmasını garanti eder; arka planda `try/finally`'ye çevrilir. `using var x = ...` içinde bulunduğu blok bitince dispose eder; async kaynaklar için `await using`. GC ile ilgisi yoktur.

</details>

### 🧩 8.4 Bu kodda ne sorun var?

```csharp
public async Task<string> RaporOku()
{
    var okuyucu = new StreamReader("rapor.txt");
    var icerik = await okuyucu.ReadToEndAsync();
    if (string.IsNullOrEmpty(icerik))
        throw new Exception("Rapor boş");
    okuyucu.Dispose();
    return icerik;
}
```

<details>
<summary>Cevap</summary>

Hata fırlatılırsa (rapor boşsa ya da okuma patlarsa) `Dispose()` satırına hiç gelinmez ve dosya açık kalır. Çözüm `using var okuyucu = new StreamReader("rapor.txt");` yazıp elle `Dispose`'u kaldırmak. Ek olarak düz `Exception` yerine anlamlı bir tip (`InvalidOperationException` ya da özel exception) fırlatılmalı.

</details>

### 8.5 DI'dan aldığın `DbContext`'i dispose etmen gerekir mi?

<details>
<summary>Cevap</summary>

Hayır. DI konteyneri onun ömrünü yönetir ve istek bitince dispose eder. Kural: kendin `new` ile oluşturduğun `IDisposable`'ı sen bırakırsın; DI'dan gelenleri konteyner bırakır. Elle dispose etmek aynı istekteki diğer kodların "nesne dispose edildi" hatası almasına yol açar.

</details>

### ⭐ 8.6 Her istekte `using var client = new HttpClient()` yazmak neden sorunludur?

<details>
<summary>Cevap</summary>

HttpClient dispose edilse bile altındaki ağ bağlantısı işletim sisteminde bir süre açık bekler; yoğun trafikte binlerce bağlantı birikir ve yeni bağlantı açılamaz hale gelir (socket exhaustion). Çözüm `IHttpClientFactory` (`builder.Services.AddHttpClient<...>()`); bağlantıları havuzda tutup yeniden kullanır.

</details>

### 8.7 Finalizer ile Dispose arasındaki fark nedir?

<details>
<summary>Cevap</summary>

Dispose deterministiktir; kod tarafından, istenen anda çağrılır. Finalizer (`~Sinif()`) GC tarafından belirsiz bir zamanda çağrılır; Dispose unutulursa son çaredir, GC'yi yavaşlatır ve günlük kodda nadiren yazılır.

</details>

### ⭐ 8.8 .NET'te garbage collector varken bellek sızıntısı olur mu?

<details>
<summary>Cevap</summary>

Olur. GC sadece ulaşılamayan nesneleri temizler; ulaşılabilir kalan ama gereksiz nesneler birikir. Tipik sebepler: event'ten `-=` ile ayrılmamak, statik koleksiyonlarda sürekli biriktirmek (`static List<Kargo>`), boyut ya da süre sınırı olmayan bellek içi cache.

</details>

### 8.9 Deterministik ne demektir?

<details>
<summary>Cevap</summary>

Ne zaman ve nasıl olacağı önceden kesin bilinen demektir. `Dispose` deterministiktir (çağrıldığı satırda çalışır); garbage collector deterministik değildir (temizleyeceği kesin, ne zaman temizleyeceği belli değil).

</details>

---

## 9. Sık sorulan küçük detaylar

### ⭐ 9.1 `==` ile `Equals` arasındaki fark nedir?

<details>
<summary>Cevap</summary>

Değer tiplerinde ikisi de değere bakar. Class'larda varsayılan olarak ikisi de referansa (aynı nesne mi?) bakar. String'te `==` içeriğe bakacak şekilde yeniden tanımlanmıştır. Record'larda ikisi de alanlara bakar. Struct'ta `Equals` alanlara bakar, `==` tanımlanmadıkça yoktur.

</details>

### ⭐ 9.2 `Equals`'ı override edersen neden `GetHashCode`'u da override etmelisin?

<details>
<summary>Cevap</summary>

`Dictionary` ve `HashSet` önce `GetHashCode` ile hangi bölmeye bakacağını bulur, sonra `Equals` ile karşılaştırır. Kural: eşit nesnelerin hash kodu aynı olmalıdır. Yalnız `Equals` değiştirilirse eşit sayılan iki nesne farklı bölmelere düşer; `HashSet` aynı kaydı iki kez ekler, `Dictionary` kaydı bulamaz.

</details>

### ⭐ 9.3 `const`, `readonly` ve `static readonly` arasındaki fark nedir?

<details>
<summary>Cevap</summary>

`const` derleme zamanında belli olan sabittir (sayı, string, bool); kullanıldığı yere kopyalanır. `readonly` tanımlandığı yerde ya da constructor'da bir kez atanır; çalışma zamanında hesaplanabilir ve her nesnede farklı olabilir. `static readonly` sınıf ilk kullanıldığında bir kez atanır, sınıf başına tek değerdir. `const` kütüphanede değişirse onu kullanan projeler yeniden derlenene kadar eski değeri kullanır; değişebilecek değerler için `static readonly` daha güvenlidir.

</details>

### 🧩 9.4 Bu kod derlenir mi?

```csharp
public class Kurye
{
    public const int MaksPaket = 30;
    public readonly string Ad;
    public Kurye(string ad) => Ad = ad;
    public void AdDegistir(string yeni) => Ad = yeni;
}
```

<details>
<summary>Cevap</summary>

Derlenmez. `AdDegistir`, `readonly` olan `Ad`'a atama yapıyor; `readonly` alana sadece tanımlandığı yerde ya da constructor'da atama yapılabilir.

</details>

### 9.5 `readonly List<Kargo> _liste;` tanımlıysa listeye eleman eklenebilir mi?

<details>
<summary>Cevap</summary>

Evet. `readonly` sığdır: `_liste` başka bir listeye çevrilemez ama gösterdiği listenin içi değiştirilebilir. Değişmeyen şey değişkenin gösterdiği yerdir, nesnenin içi değil.

</details>

### 9.6 `int?` nedir? Nullable reference types nedir?

<details>
<summary>Cevap</summary>

`int?` (`Nullable<int>`) değer tipine "değer yok" anlamında `null` atanabilmesini sağlar; `HasValue` ve `Value` ile kullanılır (ör. henüz teslim edilmemiş kargonun `DateTime? TeslimTarihi`). Nullable reference types (C# 8+) ile `string` null olamaz, `string?` olabilir; derleyici null olabilecek değerin kontrolsüz kullanımında uyarı verir. Amaç `NullReferenceException`'ı kod yazarken yakalamaktır.

</details>

### 9.7 `?.`, `??`, `??=` ve `!` operatörleri ne yapar?

<details>
<summary>Cevap</summary>

`?.` nesne null değilse üyeye erişir, null ise `null` döner. `??` soldaki null ise sağdakini kullanır. `??=` değişken null ise atar. `!` derleyicinin null uyarısını susturur ama kontrol yapmaz; değer gerçekten null'sa yine `NullReferenceException` alınır. Örnek: `kargo?.Alici?.Ad ?? "Bilinmiyor"`.

</details>

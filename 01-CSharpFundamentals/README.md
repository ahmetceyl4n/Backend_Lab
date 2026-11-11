# 01 - CSharpFundamentals

C# dilinin mülakatlarda en çok sorulan temellerini çalıştırılabilir örneklerle gösterir: değer ve
referans tipleri, parametre geçişi, boxing, class/struct/record eşitliği, interface ve abstract class,
generics, delegate ve event, LINQ'in ertelenmiş çalışması, exception yönetimi, IDisposable ve GC,
string ve sabitler. Örnekler kargo alanından (paket, gönderi, şube, kurye, takip numarası) seçilmiştir.

## Çalıştırma

```bash
# Menüyle:
dotnet run --project CSharpFundamentals

# Doğrudan bir örnek (ör. 9. örnek):
dotnet run --project CSharpFundamentals -- 9
```

## Örnekler

| # | Örnek | Ne gösterir |
|---|-------|-------------|
| 1 | ValueVsReference | `int` ataması değeri kopyalar; `class` ataması aynı nesneyi gösteren ikinci değişken üretir |
| 2 | ParameterPassing | Metoda `int` / `class` geçmek; alan değişikliği dışarı yansır, parametreye `new` yansımaz, `ref` ile yansır |
| 3 | StructInsideClass | Class içindeki struct alanı: class ataması paylaşır, struct'ı dışarı almak kopyalar; property'de CS1612 |
| 4 | Boxing | `int` → `object` kopyası, iki kutunun `==` ile eşit olmaması; `ArrayList` vs `List<int>` süre ve bellek |
| 5 | ClassStructRecord | `==` / `Equals`: class referans, struct alan, record değer karşılaştırır; `with` ve init-only |
| 6 | InterfaceVsAbstract | Kargo fiyatı: abstract class ortak akış taşır, interface sözleşmedir; default interface method |
| 7 | Generics | `InMemoryRepository<T>` ve `DefaultingCache<TKey, TValue>`; `where T : class, IEntity`, `new()`, `notnull` |
| 8 | DelegatesEvents | `Func` / `Action` / `Predicate`; teslimat event'i; düz delegate alanının `=` ile ezilmesi ve dışarıdan tetiklenmesi |
| 9 | LinqDeferred | Sorgu enumerate edilince çalışır, her enumerate'te yeniden çalışır; `ToList` dondurur; `yield` iterator |
| 10 | ExceptionHandling | try/catch/finally sırası, `when` filtresi, özel exception, `throw` vs `throw ex` stack trace farkı |
| 11 | DisposableAndGc | `using` ve `using var` ile Dispose zamanı, finalizer'ın belirsiz zamanı, GC nesilleri ve LOH |
| 12 | StringAndEquality | String değişmezliği, `+` vs `StringBuilder`, `const` vs `readonly`, nullable reference types |

Bazı örneklerde derleme hatası veren satırlar **yorum satırı** olarak bırakıldı (ör. `// r1.Weight = 10; // CS8852`).
Açıp derlersen derleyicinin seni nasıl durdurduğunu görürsün.

## Örnek çıktı

```
--- 2. ParameterPassing: int / class parametre, alan değiştirme, new atama, ref ---
1) int geçildi, içeride 50 yapıldı      -> dışarıda price = 100  (kopya değişti)
2) ref int geçildi, içeride 50 yapıldı  -> dışarıda price = 50  (asıl değişken değişti)

3) class geçildi, içeride Weight=99     -> dışarıda TR100(99 kg)  (aynı nesne, alan değişti)
4) class geçildi, içeride p = new(...)  -> dışarıda TR100(99 kg)  (sadece yerel kopya yönlendi)
5) ref class geçildi, içeride p = new() -> dışarıda YENI(1 kg)  (dışarıdaki değişken yönlendi)
```

```
--- 9. LinqDeferred: sorgu enumerate edilince çalışır, ToList, iki kez enumerate, yield ---
1) Sorgu tanımlandı           -> filtre çağrı sayısı: 0
2) Listeye 30 eklendi, enumerate: [15, 22, 30]  -> çağrı: 5
3) heavy.Count() = 3                  -> çağrı: 10  (iki kez çalıştı)
4) ToList sonra 50 eklendi    -> snapshot: [15, 22, 30], sorgu: [15, 22, 30, 50]
```

```
--- 10. ExceptionHandling ---
2) Stack trace'in ilk satırı:
   throw    -> at ...Example10_ExceptionHandling.CallCarrierApi()
   throw ex -> at ...Example10_ExceptionHandling.RethrowWithThrowEx()
```

## Testler

```bash
dotnet test CSharpFundamentals.Tests
```

- `ParameterPassingTests`: değerle geçen `int` değişmez, `ref` değişir; class alanı değişir, parametreye `new` yansımaz, `ref` ile yansır.
- `StructCopyTests`: class ataması aynı nesneyi paylaşır; struct kopyası değişince orijinal değişmez.
- `RecordEqualityTests`: record değer eşitliği (`==`, `Equals`, `GetHashCode`), class'ın eşit olmaması, `with`'in yeni nesne üretmesi.
- `LinqDeferredTests`: sorgu tanımlanınca çalışmaz, her enumerate'te yeniden çalışır, `ToList` bir kez çalışır, `yield` tembel üretir.
- `ExceptionRethrowTests`: `throw` hatanın çıktığı metodu korur, `throw ex` siler.
- `DisposableTests`: `using` bloğu ve `using var` Dispose'u çağırır; exception olsa bile çağırır.

> Not: Süre ve bellek ölçen kısımlar (Boxing, StringBuilder) ile finalizer'ın ne zaman çalıştığı makineye
> ve GC'ye bağlı olduğu için test edilmez; yalnızca konsol örneğinde gösterilir.

## Notlarım ve mülakat soruları

- [Notlar](01-csharp-fundamentals-notlar.md): konu anlatımı, örnekler ve her bölümün sonunda mülakat notu
- [Mülakat soruları](01-csharp-fundamentals-sorular.md): cevapları gizli sorular (⭐ sık sorulan, 🧩 kod / tuzak sorusu)

# CLAUDE.md — dotnet-backend-lab

Bu repo bir **öğrenme ve vitrin** reposudur; ürün değildir. Amaç: .NET backend mülakatına
hazırlanırken çalışılan konuları **çalıştırılabilir örnekler ve testlerle** göstermek.
Konular zamanla tek tek eklenir.

## Genel kurallar

- .NET güncel LTS sürümü kullanılır (`net10.0`). Tek solution: `DotnetBackendLab.sln`.
- Kod, tip ve dosya adları **İngilizce**; yorumlar ve konsol çıktıları **Türkçe**.
- Örnekler **hayattan** seçilir; her konu için o konuyu en iyi anlatan alan kullanılır (ör. kargo, bankacılık, e-ticaret, rezervasyon...). Tek bir alana bağlı kalma zorunluluğu yoktur.
- Her örnek kısadır (tek dosya, 40–100 satır) ve **sonucu + süreyi** ekrana net yazar.
- Ortak ayarlar `Directory.Build.props` içindedir: nullable açık, uyarılar görünür.
- GitHub Actions her push'ta `restore → build → test` çalıştırır (`setup-dotnet`, `10.0.x`).

## Klasör düzeni

`src/` ve `tests/` ayrımı **yoktur**. Her konu kendi klasöründe her şeyiyle durur:

```
NN-Konu/
  README.md              # örnek listesi, çalıştırma komutu, örnek çıktı
  NOTES.md               # kullanıcının notları — sadece boş başlıklarla oluşturulur, içeriği kullanıcı yazar
  Konu/                  # konsol projesi
  Konu.Tests/            # xUnit test projesi
```

- Klasör adı konu numarasıyla başlar: `01-CSharpFundamentals`, `04-ThreadsAsync`, `15-ConcurrentCollections`, ...
- **Numaralandırma kuralı:** konu numaraları kullanıcının çalışma listesindeki sırayı izler (aşağıdaki
  "Konular" tablosu, 01–20). Numara konuya sabittir; konular hangi sırayla eklenirse eklensin değişmez,
  araya boşluk kalması normaldir. Yeni konu eklerken numarayı bu tablodan al.
- Proje ve namespace adları **rakamla başlamaz** ve klasör numarası değişse bile değişmez:
  - `Konu.csproj` → `RootNamespace: DotnetBackendLab.Konu`
  - `Konu.Tests.csproj` → `RootNamespace: DotnetBackendLab.Konu.Tests`
  - Örnek: `15-ConcurrentCollections/ConcurrentCollections/` ve `15-ConcurrentCollections/ConcurrentCollections.Tests/`.
- Notlar her konunun `NOTES.md` dosyasındadır; ayrı bir `docs/` klasörü yoktur.

## Konsol uygulaması davranışı

- Numaralı menüyle açılır: `dotnet run`.
- Numarayla doğrudan bir örnek çalışır: `dotnet run -- 3`.
- Her örnek `IExample` arayüzünü uygular (Number, Title, RunAsync); `ExampleRunner` menüyü basar ve süreyi ölçer.

## Testler

- xUnit kullanılır.
- Sadece **deterministik** davranış test edilir (lock'lu sayaç, lock'lu BankAccount, SemaphoreSlim sınırı, iptal).
- Korumasız/hatalı sürümler test **edilmez** (sonuç rastgele → kararsız test); yalnızca konsol örneğinde gösterilir.

## Kök README

Reponun amacını ve konular tablosunu içerir: `no | konu | klasör | notlar (NOTES.md) | durum`.
Tabloda her konunun **klasörüne** ve **NOTES.md** dosyasına link bulunur.

## Çalışma tarzı

- Bir şey yazmadan önce plan madde madde gösterilir ve kullanıcı onayı beklenir.
- Her örnek yazıldığında: neyi gösterdiği ve hangi satır değişirse neyin bozulacağı 1–2 cümleyle açıklanır.
- Kapsam dışına çıkılmaz; bir sonraki konuya kendiliğinden geçilmez.

## Konular

| No | Konu | Durum |
|----|------|-------|
| 01 | C# dil temelleri (CSharpFundamentals) | Hazır |
| 02 | SOLID ve design pattern'ler | Planlandı |
| 03 | ASP.NET Core temelleri | Planlandı |
| 04 | Thread/Task/async (ThreadsAsync) | Hazır |
| 05 | EF Core | Planlandı |
| 06 | SQL | Planlandı |
| 07 | Algoritma | Planlandı |
| 08 | HTTP ve REST | Planlandı |
| 09 | Uygulama mimarisi | Planlandı |
| 10 | Kimlik doğrulama | Planlandı |
| 11 | Test | Planlandı |
| 12 | Cache ve Redis | Planlandı |
| 13 | Git ve code review | Planlandı |
| 14 | RabbitMQ | Planlandı |
| 15 | Concurrent koleksiyonlar | Planlandı |
| 16 | Arka plan işleri | Planlandı |
| 17 | Güvenlik | Planlandı |
| 18 | Canlıya çıkış | Planlandı |
| 19 | Dağıtık sistem desenleri | Planlandı |
| 20 | Performans | Planlandı |

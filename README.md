# dotnet-backend-lab

.NET backend mülakatına hazırlanırken çalıştığım konuları **çalıştırılabilir örnekler ve
testlerle** gösteren bir öğrenme ve vitrin reposu. Ürün değil; her konu kısa, net ve kendi
başına çalışır. Konular zamanla tek tek eklenir.

## Öne çıkanlar

- .NET güncel LTS (`net10.0`), tek solution: `DotnetBackendLab.sln`.
- Her konu kendi klasöründe: konsol örnekleri + xUnit testleri + README + notlar.
- Konsol uygulaması menüyle açılır; numarayla doğrudan örnek de çalışır.
- Örnekler hayattan; her konu için o konuyu en iyi anlatan alan kullanılır. Kod/isimler İngilizce, yorum/çıktı Türkçe.
- Her push'ta GitHub Actions: `restore → build → test`.

## Konular

| No | Konu | Klasör | Notlar | Mülakat soruları | Durum |
|----|------|--------|--------|------------------|-------|
| 01 | C# dil temelleri | [01-CSharpFundamentals](01-CSharpFundamentals/) | [Notlar](01-CSharpFundamentals/01-csharp-fundamentals-notlar.md) | [Sorular](01-CSharpFundamentals/01-csharp-fundamentals-sorular.md) | ✅ Hazır |
| 02 | SOLID ve design pattern'ler | — | — | — | Planlandı |
| 03 | ASP.NET Core temelleri | — | — | — | Planlandı |
| 04 | Thread/Task/async | [04-ThreadsAsync](04-ThreadsAsync/) | [NOTES.md](04-ThreadsAsync/NOTES.md) | — | ✅ Hazır |
| 05 | EF Core | — | — | — | Planlandı |
| 06 | SQL | — | — | — | Planlandı |
| 07 | Algoritma | — | — | — | Planlandı |
| 08 | HTTP ve REST | — | — | — | Planlandı |
| 09 | Uygulama mimarisi | — | — | — | Planlandı |
| 10 | Kimlik doğrulama | — | — | — | Planlandı |
| 11 | Test | — | — | — | Planlandı |
| 12 | Cache ve Redis | — | — | — | Planlandı |
| 13 | Git ve code review | — | — | — | Planlandı |
| 14 | RabbitMQ | — | — | — | Planlandı |
| 15 | Concurrent koleksiyonlar | — | — | — | Planlandı |
| 16 | Arka plan işleri | — | — | — | Planlandı |
| 17 | Güvenlik | — | — | — | Planlandı |
| 18 | Canlıya çıkış | — | — | — | Planlandı |
| 19 | Dağıtık sistem desenleri | — | — | — | Planlandı |
| 20 | Performans | — | — | — | Planlandı |

Numaralar çalışma listemdeki sırayı izler ve konuya sabittir; konular hangi sırayla eklenirse eklensin numara değişmez.

## Çalıştırma

```bash
# Tümünü derle ve test et
dotnet build DotnetBackendLab.sln
dotnet test DotnetBackendLab.sln

# Bir konunun örneklerini menüyle çalıştır
dotnet run --project 04-ThreadsAsync/ThreadsAsync

# Doğrudan bir örnek
dotnet run --project 04-ThreadsAsync/ThreadsAsync -- 6
```

## Depo düzeni

```
NN-Konu/
  README.md            # örnek listesi, çalıştırma komutu, örnek çıktı
  NN-konu-notlar.md    # konu notlarım (benim yazdığım)
  NN-konu-sorular.md   # mülakat soruları ve cevapları (benim yazdığım)
  Konu/                # konsol projesi      (RootNamespace: DotnetBackendLab.Konu)
  Konu.Tests/          # xUnit test projesi  (RootNamespace: DotnetBackendLab.Konu.Tests)
```

Klasör adları konu numarasıyla başlar (`01-CSharpFundamentals`, `04-ThreadsAsync`); proje ve namespace adları rakamla başlamaz.
Düzen kuralları ve çalışma tarzı için `CLAUDE.md` dosyasına bakın.

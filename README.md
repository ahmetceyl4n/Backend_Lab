# dotnet-backend-lab

.NET backend mülakatına hazırlanırken çalıştığım konuları **çalıştırılabilir örnekler ve
testlerle** gösteren bir öğrenme ve vitrin reposu. Ürün değil; her konu kısa, net ve kendi
başına çalışır. Konular zamanla tek tek eklenir.

## Öne çıkanlar

- .NET güncel LTS (`net10.0`), tek solution: `DotnetBackendLab.sln`.
- Her konu kendi klasöründe: konsol örnekleri + xUnit testleri + README + notlar.
- Konsol uygulaması menüyle açılır; numarayla doğrudan örnek de çalışır.
- Örnekler kargo alanından (kargo, şube, takip numarası, kurye). Kod/isimler İngilizce, yorum/çıktı Türkçe.
- Her push'ta GitHub Actions: `restore → build → test`.

## Konular

| No | Konu | Klasör | Notlar | Durum |
|----|------|--------|--------|-------|
| 01 | Threads & Async | [01-ThreadsAsync](01-ThreadsAsync/) | [NOTES.md](01-ThreadsAsync/NOTES.md) | ✅ Hazır |

## Çalıştırma

```bash
# Tümünü derle ve test et
dotnet build DotnetBackendLab.sln
dotnet test DotnetBackendLab.sln

# Bir konunun örneklerini menüyle çalıştır
dotnet run --project 01-ThreadsAsync/ThreadsAsync

# Doğrudan bir örnek
dotnet run --project 01-ThreadsAsync/ThreadsAsync -- 6
```

## Depo düzeni

```
NN-Konu/
  README.md            # örnek listesi, çalıştırma komutu, örnek çıktı
  NOTES.md             # kişisel notlar
  Konu/                # konsol projesi      (RootNamespace: DotnetBackendLab.Konu)
  Konu.Tests/          # xUnit test projesi  (RootNamespace: DotnetBackendLab.Konu.Tests)
```

Klasör adları sıralıdır (`01-ThreadsAsync`); proje ve namespace adları rakamla başlamaz.
Düzen kuralları ve çalışma tarzı için `CLAUDE.md` dosyasına bakın.

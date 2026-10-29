# 01 - ThreadsAsync

Thread, ThreadPool, Task, `async`/`await`, paralellik, eşzamanlılık sınırlama ve senkronizasyon
konularını çalıştırılabilir örneklerle gösterir. Bu konudaki örnekler çoğunlukla kargo/lojistik
alanından (kargo, şube, takip numarası, kurye) seçilmiştir.

## Çalıştırma

```bash
# Menüyle:
dotnet run --project ThreadsAsync

# Doğrudan bir örnek (ör. 6. örnek):
dotnet run --project ThreadsAsync -- 6
```

## Örnekler

| # | Örnek | Ne gösterir |
|---|-------|-------------|
| 1 | ThreadPoolReuse | `new Thread` her seferinde yeni thread üretir; ThreadPool thread'leri tekrar kullanır |
| 2 | ThreadVsTask | Task ile sonuç dönüşü ve `await` ile hata yakalama; Thread'de aynı işin zahmeti |
| 3 | AwaitThreads | `await` öncesi/sonrası thread numarası; `Thread.Sleep` (bloklar) vs `Task.Delay` (bloklamaz) |
| 4 | SequentialVsWhenAll | Sırayla `await` (süreler toplanır) vs `Task.WhenAll` (paralel, en yavaş kadar) |
| 5 | CpuVsIo | CPU işinde `Parallel.For` kazancı; I/O işini `Task.Run`'a sarmanın kazanç getirmemesi |
| 6 | LimitedConcurrency | Sınırsız `WhenAll` vs `SemaphoreSlim(10)` vs `Parallel.ForEachAsync(10)` — tepe eşzamanlılık |
| 7 | WhenAllWhenAny | `WhenAll`'da `await`'in yalnızca ilk hatayı fırlatması, `InnerExceptions`; `WhenAny` ile zaman aşımı |
| 8 | RaceCondition | `counter++` ile kaybolan güncellemeler; `lock` ve `Interlocked` ile düzeltme |
| 9 | BankAccount | `ParaCek` check-then-act: korumasız / Interlocked (hâlâ hatalı) / lock (doğru) |
| 10 | LockOrderDeadlock | Ters kilit sırasının deadlock'u; `Monitor.TryEnter` + zaman aşımı ile donmadan tespit |
| 11 | Cancellation | `CancellationTokenSource` zaman aşımı, token'ın alt metotlara geçişi, `OperationCanceledException` |

## Örnek çıktı

```
--- 4. SequentialVsWhenAll: 3 servis çağrısı sırayla vs Task.WhenAll ---
Sırayla : İstanbul:74 paket, Ankara:16 paket, İzmir:42 paket
   Süre: 941 ms (~900, toplanır)

WhenAll : İstanbul:47 paket, Ankara:85 paket, İzmir:32 paket
   Süre: 310 ms (~300, paralel)
--- Bitti: toplam 1252 ms ---
```

```
--- 6. LimitedConcurrency: WhenAll / SemaphoreSlim(10) / Parallel.ForEachAsync(10) ---
Sınırsız WhenAll          : en yüksek eşzamanlı = 1000 (12 ms)
SemaphoreSlim(10)         : en yüksek eşzamanlı =   10 (1555 ms)
Parallel.ForEachAsync(10) : en yüksek eşzamanlı =   10 (1550 ms)
```

## Testler

```bash
dotnet test ThreadsAsync.Tests
```

- `CounterTests` — lock'lu ve Interlocked'lı sayaç her zaman beklenen değeri verir.
- `BankAccountTests` — lock'lu hesap paralel çekimlerde asla eksiye düşmez.
- `SemaphoreConcurrencyTests` — `SemaphoreSlim(10)` ile tepe eşzamanlılık 10'u geçmez.
- `CancellationTests` — iptal edilen işlem `OperationCanceledException` fırlatır.

> Not: Korumasız/hatalı sürümler (race condition, eksiye düşen hesap) kararsız sonuç verdiği için
> test edilmez; yalnızca konsol örneğinde gösterilir.

## Notlarım

Konuya dair kişisel notlar: [NOTES.md](NOTES.md)

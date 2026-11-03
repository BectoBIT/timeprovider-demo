# BectoBIT – TimeProvider w .NET 8: działający przykład

<<<<<<< HEAD
[![CI](https://github.com/BectoBIT/timeprovider-demo/actions/workflows/ci.yml/badge.svg)](https://github.com/BectoBIT/timeprovider-demo/actions/workflows/ci.yml)
=======
[![CI](https://github.com/BectoBIT/timeprovider-demo/actions/workflows/ci.yml/badge.svg)](https://github.com/bectobit/timeprovider-demo/actions/workflows/ci.yml)
>>>>>>> c8d1c75940da8ca80892de7831a00682a6ceafea

Przykład do filmu [„Czas też jest zależnością”](https://youtu.be/V3mkU8Ut6sA) na kanale [@BectoBIT](https://www.youtube.com/@BectoBIT). Pokazuje, jak wstrzyknąć czas zamiast `DateTime.UtcNow`
i jak w teście przesunąć go o 31 dni bez czekania.

```
BectoBIT.TimeProviderDemo.sln
├─ src/Subscriptions              biblioteka: Subscription, SubscriptionService (bez DateTime.Now)
├─ demo/SubscriptionDemo          aplikacja konsolowa pokazująca efekt
└─ tests/Subscriptions.Tests      3 testy xUnit z FakeTimeProvider
```

Wymagania: .NET 8 SDK lub nowszy.

## Uruchomienie

```
dotnet run --project demo/SubscriptionDemo
dotnet test
```

Wersje pakietów w `.csproj` są przypięte. Jeśli `restore` zgłosi brak którejś wersji, zaktualizuj ją, np.:
`dotnet add tests/Subscriptions.Tests package Microsoft.Extensions.TimeProvider.Testing`.

## Co pokazuje demo

1. **`TimeProvider.System`.** To prawdziwy zegar, tak jak na produkcji. Wynik zależy od dzisiejszej daty.
2. **`FakeTimeProvider`.** Start 1.11.2026, potem `Advance(...)` o 10 dni, 19 dni i aż do sekundy przed końcem.
   O 23:59:58 subskrypcja jest jeszcze **AKTYWNA**, a o 23:59:59 (dokładnie `ValidTo`) już **WYGASŁA**,
   bo warunek to `ValidTo > teraz`. Jeśli chcesz, żeby ostatnia sekunda się liczyła, zmień go na `>=`
   i popraw test `..._one_second_before_end_and_expired_at_end`.
3. **`Task.Delay(24 h, time)`.** Zadanie kończy się zaraz po `time.Advance(24 h)`, a realnie mijają milisekundy.
   Ten sam zegar steruje timerami, więc da się testować kod, który na coś czeka.

## Najważniejsze linie

```csharp
public class SubscriptionService(TimeProvider time)
{
    public bool IsActive(Subscription s) => s.ValidTo > time.GetUtcNow();
}

// aplikacja (Program.cs z DI):
builder.Services.AddSingleton(TimeProvider.System);

// test:
var time = new FakeTimeProvider(new DateTimeOffset(2026, 11, 1, 0, 0, 0, TimeSpan.Zero));
time.Advance(TimeSpan.FromDays(31));
```

`GetUtcNow()` zwraca `DateTimeOffset`, dlatego `Subscription.ValidTo` też jest `DateTimeOffset`.

## Licencja

MIT – zobacz [LICENSE](LICENSE).

---

Każdy bit ma swój kierunek. · https://bectobit.pl

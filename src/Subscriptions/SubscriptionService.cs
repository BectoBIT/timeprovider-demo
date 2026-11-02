namespace Subscriptions;

/// <summary>
/// Klasa nie pyta systemu o godzinę (brak DateTime.UtcNow).
/// Czas przychodzi z zewnątrz: w aplikacji TimeProvider.System, w teście FakeTimeProvider.
/// </summary>
public class SubscriptionService(TimeProvider time)
{
    public bool IsActive(Subscription s) => s.ValidTo > time.GetUtcNow();

    /// <summary>Ile pełnych dni zostało do końca subskrypcji (0, jeśli już wygasła).</summary>
    public int DaysLeft(Subscription s) =>
        Math.Max(0, (int)(s.ValidTo - time.GetUtcNow()).TotalDays);
}

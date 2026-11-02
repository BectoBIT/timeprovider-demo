namespace Subscriptions;

/// <summary>Subskrypcja ważna do określonej chwili (DateTimeOffset – jak to, co zwraca TimeProvider).</summary>
public record Subscription(DateTimeOffset ValidTo);

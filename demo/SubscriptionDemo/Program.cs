using System.Diagnostics;
using Microsoft.Extensions.Time.Testing;
using Subscriptions;

Console.OutputEncoding = System.Text.Encoding.UTF8;
var subscription = new Subscription(ValidTo: new(2026, 11, 30, 23, 59, 59, TimeSpan.Zero));
Console.WriteLine($"Subskrypcja ważna do: {subscription.ValidTo:yyyy-MM-dd HH:mm:ss} UTC");

// 1) Zegar systemowy – tak działa aplikacja na produkcji.
var real = new SubscriptionService(TimeProvider.System);
Console.WriteLine();
Console.WriteLine("[1] TimeProvider.System (prawdziwy zegar)");
Console.WriteLine($"    teraz: {TimeProvider.System.GetUtcNow():yyyy-MM-dd HH:mm} UTC -> {Status(real)}");

// 2) Sztuczny zegar – przesuwamy czas, zamiast na niego czekać.
var time = new FakeTimeProvider(new DateTimeOffset(2026, 11, 1, 0, 0, 0, TimeSpan.Zero));
var sut = new SubscriptionService(time);
Console.WriteLine();
Console.WriteLine("[2] FakeTimeProvider (czas pod kontrolą testu)");
Show();
foreach (var step in new[] { TimeSpan.FromDays(10), TimeSpan.FromDays(19), TimeSpan.FromHours(23) + TimeSpan.FromMinutes(59) + TimeSpan.FromSeconds(58), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1) })
{
    time.Advance(step);
    Show($"+{Fmt(step)}");
}

// 3) Ten sam zegar steruje czekaniem: Task.Delay(24 h) kończy się od razu po Advance.
Console.WriteLine();
Console.WriteLine("[3] Task.Delay(24 h) z FakeTimeProvider");
var sw = Stopwatch.StartNew();
var delay = Task.Delay(TimeSpan.FromHours(24), time);
Console.WriteLine($"    po starcie:       ukończony = {delay.IsCompleted}");
time.Advance(TimeSpan.FromHours(24));
await delay;
Console.WriteLine($"    po Advance(24 h): ukończony = {delay.IsCompleted}, realnie minęło {sw.ElapsedMilliseconds} ms");

string Status(SubscriptionService s) => s.IsActive(subscription) ? $"AKTYWNA (zostało dni: {s.DaysLeft(subscription)})" : "WYGASŁA";
void Show(string label = "start")
    => Console.WriteLine($"    {label,-12} {time.GetUtcNow():yyyy-MM-dd HH:mm:ss} -> {Status(sut)}");
static string Fmt(TimeSpan t) => t.TotalDays >= 1 ? $"{t.TotalDays:0} dni" : t.TotalSeconds < 60 ? $"{t.TotalSeconds:0} s" : $"{t:hh\\:mm\\:ss}";

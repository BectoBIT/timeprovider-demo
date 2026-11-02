using Microsoft.Extensions.Time.Testing;
using Subscriptions;
using Xunit;

public class SubscriptionServiceTests
{
    private static readonly DateTimeOffset Start = new(2026, 11, 1, 0, 0, 0, TimeSpan.Zero);
    private static readonly Subscription Subscription = new(ValidTo: new(2026, 11, 30, 23, 59, 59, TimeSpan.Zero));

    [Fact]
    public void Subscription_expires_after_31_days()
    {
        var time = new FakeTimeProvider(Start);
        var sut = new SubscriptionService(time);

        Assert.True(sut.IsActive(Subscription));
        time.Advance(TimeSpan.FromDays(31));
        Assert.False(sut.IsActive(Subscription));
    }

    [Fact]
    public void Subscription_is_active_one_second_before_end_and_expired_at_end()
    {
        var time = new FakeTimeProvider(Subscription.ValidTo.AddSeconds(-1));
        var sut = new SubscriptionService(time);

        Assert.True(sut.IsActive(Subscription));
        time.Advance(TimeSpan.FromSeconds(1));   // dokładnie ValidTo
        Assert.False(sut.IsActive(Subscription));
    }

    [Fact]
    public async Task Delay_completes_when_fake_time_advances()
    {
        var time = new FakeTimeProvider(Start);
        var delay = Task.Delay(TimeSpan.FromHours(24), time);

        Assert.False(delay.IsCompleted);
        time.Advance(TimeSpan.FromHours(24));
        await delay;                              // bez czekania doby
        Assert.True(delay.IsCompleted);
    }
}

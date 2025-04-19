using Polly;
using Polly.Wrap;
using Microsoft.Data.Sqlite;

public static class ResiliencePolicies
{
    public static IAsyncPolicy WrapRetryAndCircuitBreaker()
    {
        var retryPolicy = Policy
            .Handle<SqliteException>()
            .WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10)
            });

        var circuitBreakerPolicy = Policy
            .Handle<SqliteException>()
            .CircuitBreakerAsync(2, TimeSpan.FromMinutes(1));

        return Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);
    }
}

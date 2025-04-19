using Polly;
using Microsoft.Data.Sqlite;

public static class CircuitBreakerPolicy
{
    public static IAsyncPolicy Get()
    {
        return Policy
            .Handle<SqliteException>()
            .CircuitBreakerAsync(2, TimeSpan.FromMinutes(1));
    }
}

using Polly;
using Microsoft.Data.Sqlite;

public static class RetryPolicy
{
    public static IAsyncPolicy Get()
    {
        return Policy
            .Handle<SqliteException>()
            .WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10)
            });
    }
}

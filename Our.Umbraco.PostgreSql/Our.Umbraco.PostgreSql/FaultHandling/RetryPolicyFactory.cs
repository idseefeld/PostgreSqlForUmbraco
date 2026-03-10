using Our.Umbraco.PostgreSql.FaultHandling.Strategies;
using Umbraco.Cms.Infrastructure.Persistence.FaultHandling;

namespace Our.Umbraco.PostgreSql.FaultHandling;

/// <summary>
///     Provides a factory class for instantiating application-specific retry policies.
/// </summary>
public static class RetryPolicyFactory
{
    public static RetryPolicy GetPostgreSqlConnectionRetryPolicy()
    {
        RetryStrategy retryStrategy = RetryStrategy.DefaultExponential;
        var retryPolicy = new RetryPolicy(new NetworkConnectivityErrorDetectionStrategy(), retryStrategy);

        return retryPolicy;
    }

    public static RetryPolicy GetPostgreSqlCommandRetryPolicy()
    {
        RetryStrategy retryStrategy = RetryStrategy.DefaultFixed;
        var retryPolicy = new RetryPolicy(new NetworkConnectivityErrorDetectionStrategy(), retryStrategy);

        return retryPolicy;
    }
}

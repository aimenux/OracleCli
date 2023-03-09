using System.Diagnostics;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging;

namespace Tests;

public sealed class OracleFixture : IAsyncLifetime
{
    private readonly OracleTestcontainer _container;

    public string ConnectionString => _container.ConnectionString;

    public OracleFixture()
    {
        var builder = new TestcontainersBuilder<OracleTestcontainer>()
            .WithDatabase(new OracleTestcontainerConfiguration
            {
                Password = "p@ssWord"
            })
            .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new OracleWaitStrategy()));

        _container = builder.Build();
    }

    public async Task InitializeAsync()
    {
        try
        {
            await _container.StartAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    public async Task DisposeAsync()
    {
        try
        {
            await _container.DisposeAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    private class OracleWaitStrategy : IWaitUntil
    {
        public async Task<bool> Until(ITestcontainersContainer testcontainers, ILogger logger)
        {
            await Task.Delay(TimeSpan.FromSeconds(60));
            return true;
        }
    }
}
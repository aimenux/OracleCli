using System.Diagnostics;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Testcontainers.Oracle;

namespace Tests;

public sealed class OracleFixture : IAsyncLifetime
{
    private readonly OracleContainer _container;

    public string ConnectionString => _container.GetConnectionString();

    public OracleFixture()
    {
        _container = new OracleBuilder()
            .WithPassword("p@ssWord")
            .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new OracleWaitStrategy()))
            .Build();
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
        public async Task<bool> UntilAsync(IContainer container)
        {
            await Task.Delay(TimeSpan.FromSeconds(60));
            return true;
        }
    }
}
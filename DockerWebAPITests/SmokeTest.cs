using DockerWebAPI.DbStuff;
using DockerWebAPI.Pulumi;
using DockerWebAPI.Pulumi.InfrastructureTemplates;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NUnit.Framework;

namespace DockerWebAPITests;

public class SmokeTest : TestFactory
{
    [Test]
    public async Task Smoke([Values] CpuSize cpus)
    {
        var dbConnectionString = _db.ConnectionString;
        Console.WriteLine(dbConnectionString);
        // Thread.Sleep(5000);
        var db = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<RandomDbContext>();

        var name = "Bobsilol";
        db.Users.Add(new User
        {
            Name = name
        });

        await db.SaveChangesAsync();

        var first = db.Users.First();
        first.Name.Should().Be(name);
    }
}

public class TestFactory : WebApplicationFactory<RandomDbContext>
{
    public MySqlTestcontainer _db = new TestcontainersBuilder<MySqlTestcontainer>()
        .WithDatabase(new MySqlTestcontainerConfiguration
        {
            Database = "somedb",
            Password = "root",
            Username = "root"
        }).Build();

    public IServiceProvider _serviceProvider;

    public TestFactory()
    {
        _serviceProvider = this.Services;
    }

    [OneTimeSetUp]
    public async Task Setup()
    {
        await _db.StartAsync();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _db.StopAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // services.RemoveAll(typeof(RandomDbContext));
            services.AddDbContext<RandomDbContext>(x =>
                x.UseMySql(_db.ConnectionString, ServerVersion.AutoDetect(_db.ConnectionString)));
        });
    }
}
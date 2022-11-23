using DockerWebAPI.Pulumi;
using DockerWebAPI.Pulumi.InfrastructureTemplates;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using NUnit.Framework;

namespace DockerWebAPITests;

public class SmokeTest
{
    private MySqlTestcontainer _db = new TestcontainersBuilder<MySqlTestcontainer>()
        .WithDatabase(new MySqlTestcontainerConfiguration()).Build();

    [Test]
    public void Smoke([Values] CpuSize cpus)
    {
        
        Console.WriteLine(cpus.ToCpuUnits());
    }
}
namespace WeaviateClient.Test.Integration;

using Client;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[TestClass]
public sealed class SchemaClientTest
{
    private static IServiceProvider serviceProvider;
    private static IConfiguration Configuration { get; set; }
    
    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
        var serviceCollection = new ServiceCollection();
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        serviceCollection.AddSingleton<IConfiguration>(Configuration);

        serviceCollection.AddWeaviateClient(options =>
        {
            options.BaseUrl = Configuration["WCD_HOST_NAME"] ?? string.Empty;
            options.ApiKey = Configuration["WCD_API_KEY"] ?? string.Empty;
            options.UserAgent = "local-test";
            options.OpenAIKey = Configuration["OPENAI_API_KEY"] ?? string.Empty;
        });

        serviceProvider = serviceCollection.BuildServiceProvider();
    }
    
    [TestMethod]
    [TestCategory("Integration")]
    public async Task DeleteSchema_ShouldSucceed()
    {   // Arrange
        var client = serviceProvider.GetRequiredService<IWeaviateClient>();
        
        // Act
        await client.Schema().DeleteAsync("Person");
        
        // Assert
        // TODO
    }
}
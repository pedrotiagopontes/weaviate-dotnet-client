namespace WeaviateClient.Test.Integration;

using API.Model;
using Client;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[TestClass]
public sealed class ObjectCreatorTest
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
    
    [ClassCleanup()]
    public static void ClassCleanup()
    {
        // Seed schema for integration tests
        // Note: This is a workaround to seed the schema for the integration tests for this POC.
        // For a live app we should use some kind of migration tool to make sure our migration tests don't depend
        // on other interfaces of the same SDK.
        var client = serviceProvider.GetRequiredService<IWeaviateClient>();
        client.Schema().DeleteAsync("IntegrationTestObjectCreator");
    }
    
    [TestMethod]
    [TestCategory("Integration")]
    public async Task CreateObject_ShouldSucceed_WhenValidInput()
    {   // Arrange
        var expectedObject = new WeaviateObject
        {
            Id = Guid.NewGuid(),
            Class = "IntegrationTestObjectCreator",
            Properties = new Dictionary<string, object>
            {
                { "name", "TestUser" },
                { "age", 30 },
                { "nickname", "Test" },
            },
            Vector = [1.0f, 1.0f, 1.0f]
        };
        
        var client = serviceProvider.GetRequiredService<IWeaviateClient>();
        
        // Act
        var result = await client.Data().Creator().
            WithId(expectedObject.Id.Value).
            WithClassName("IntegrationTestObjectCreator").
            WithProperties(new Dictionary<string, object>
            {
                {"name", "TestUser"},
                {"age", 30},
            }).
            WithProperty("nickname", "Test").
            WithVector([1.0f, 1.0f, 1.0f]).
            CreateAsync();
        
        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.CreationTimeUnix);
        Assert.IsNotNull(result.LastUpdateTimeUnix);
        Assert.AreEqual(expectedObject.Id, result.Id);
        Assert.AreEqual(expectedObject.Class, result.Class);
        foreach (var property in expectedObject.Properties)
        {
            Assert.IsNotNull(result.Properties?[property.Key]);
            Assert.AreEqual(property.Value.ToString(), result.Properties[property.Key].ToString());
        }
        
        AssertEqualVector(result.Vector, expectedObject.Vector);
    }

    private static void AssertEqualVector(float[] result, float[] expected)
    {
        Assert.IsNotNull(result);
        foreach (var expectedVector in expected)
        {
            foreach (var resultVector in result)
            {
                Assert.AreEqual(expectedVector, resultVector);
            }
        }
    }
}
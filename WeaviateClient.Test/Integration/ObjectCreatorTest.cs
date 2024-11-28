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
    
    [TestMethod]
    [TestCategory("Integration")]
    public async Task CreateObject_ShouldSucceed_WhenValidInput()
    {   // Arrange
        var expectedObject = new WeaviateObject
        {
            Class = "Person",
            Properties = new Dictionary<string, object>
            {
                { "name", "TestUser" },
                { "age", 30 }
            },
            Vector = [1.0f, 1.0f, 1.0f]
        };
        
        var client = serviceProvider.GetRequiredService<IWeaviateClient>();
        
        // Act
        var result = await client.Data().Creator().
            WithClassName("Person").
            WithProperties(new Dictionary<string, object>
            {
                {"name", "TestUser"},
                {"age", 30},
            }).
            WithVector([1.0f, 1.0f, 1.0f]).
            CreateAsync();
        
        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Id);
        Assert.IsNotNull(result.CreationTimeUnix);
        Assert.IsNotNull(result.LastUpdateTimeUnix);
        Assert.AreEqual(expectedObject.Class, result.Class);
        foreach (var property in expectedObject.Properties)
        {
            Assert.IsNotNull(result.Properties?[property.Key]);
            Assert.AreEqual(property.Value.ToString(), result.Properties[property.Key].ToString());
        }
        
        Assert.IsNotNull(result.Vector);
        foreach (var expectedVector in expectedObject.Vector)
        {
            foreach (var resultVector in result.Vector)
            {
                Assert.AreEqual(expectedVector, resultVector);
            }
        }
    }
}
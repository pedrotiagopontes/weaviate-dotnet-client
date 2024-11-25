namespace WeaviateClient.Test.Integration;

using API.Model;
using Client;
using Microsoft.Extensions.Configuration;

[TestClass]
public sealed class ObjectCreatorTest
{
    private string hostAddress;
    private string apiKey;
    private static IConfiguration Configuration { get; set; }
    
    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }

    [TestInitialize]
    public void Initialize()
    {
        hostAddress = Configuration["WCD_HOST_NAME"] ?? string.Empty;
        apiKey = Configuration["WCD_API_KEY"] ?? string.Empty;

        if (string.IsNullOrEmpty(hostAddress) || string.IsNullOrEmpty(apiKey))
        {
            Assert.Fail("Environment variables 'WCD_HOST_NAME' or 'WCD_API_KEY' are not set.");
        }
    }
    
    [TestMethod]
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
        var httpClient = new HttpClient();
        var client = new WeaviateClient(httpClient);
        client.
            WithBaseURl(hostAddress).
            WithApikey(apiKey).
            WithUserAgent("local-test").
            AcceptHeaders(["application/json"]);
        
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
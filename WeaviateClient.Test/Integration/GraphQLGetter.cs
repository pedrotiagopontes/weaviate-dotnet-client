namespace WeaviateClient.Test.Integration;

using Client;
using Microsoft.Extensions.Configuration;
using Model;

[TestClass]
public sealed class GraphQLGetter
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
    public async Task GraphQLGetter_ShouldSucceed_WhenValidInput()
    {   // Arrange
        
        var httpClient = new HttpClient();
        var client = new WeaviateClient(httpClient);
        client.
            WithBaseURl(hostAddress).
            WithApikey(apiKey).
            WithUserAgent("local-test").
            AcceptHeaders(["application/json"]);
        
        // Act
        var result = await client.GraphQL().Get().
            WithClassName("Person").
            WithFields(["name", "age"]).
            QueryAsync();
        
        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Data);
        //TODO: better assert
    }
}
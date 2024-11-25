namespace WeaviateClient.Test.Integration;

using Client;
using Microsoft.Extensions.Configuration;

[TestClass]
public sealed class SchemaDeleterTest
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
    [TestCategory("Integration")]
    public async Task DeleteSchema_ShouldSucceed()
    {   // Arrange
        var httpClient = new HttpClient();
        var client = new WeaviateClient(httpClient);
        client.
            WithBaseURl(hostAddress).
            WithApikey(apiKey).
            WithUserAgent("local-test").
            AcceptHeaders(["application/json"]);
        
        // Act
        await client.SchemaDeleter().DeleteAsync("Person");
        
        // Assert
        // TODO
    }
}
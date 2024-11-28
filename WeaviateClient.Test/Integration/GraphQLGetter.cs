namespace WeaviateClient.Test.Integration;

using System.Text.Json;
using Client;
using Extensions;
using GraphQL.QueryBuilder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[TestClass]
public sealed class GraphQLGetter
{
    private static IServiceProvider serviceProvider;
    private static IConfiguration Configuration { get; set; }

    [ClassInitialize]
    public static async Task ClassInit(TestContext context)
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
        
        // Seed schema for integration tests
        // Note: This is a workaround to seed the schema for the integration tests for this POC.
        // For a live app we should use some kind of migration tool to make sure our migration tests don't depend
        // on other interfaces of the same SDK.
        var client = serviceProvider.GetRequiredService<IWeaviateClient>();
        var objectsToCreate = 10;
        float[] startVector = [1.0f, 2.0f, 3.0f];
        for (var i = 0; i < objectsToCreate; i++)
        {
            var personFields = new Dictionary<string, object>
            {
                {"name", $"Person {i}"},
                {"age", i},
            };
    
            await client.Data().Creator().
                WithClassName("IntegrationTest").
                WithProperties(personFields).
                WithVector(startVector)
                .CreateAsync();
        
            startVector[0] += 1.0f;
            startVector[1] += 1.0f;
            startVector[2] += 1.0f;
        }
    }
    
    [ClassCleanup()]
    public static void ClassCleanup()
    {
        // Seed schema for integration tests
        // Note: This is a workaround to seed the schema for the integration tests for this POC.
        // For a live app we should use some kind of migration tool to make sure our migration tests don't depend
        // on other interfaces of the same SDK.
        var client = serviceProvider.GetRequiredService<IWeaviateClient>();
        client.Schema().DeleteAsync("IntegrationTest");
    }
    
    [TestMethod]
    [TestCategory("Integration")]
    public async Task GraphQLGetter_BaseCase_ShouldSucceed()
    {   // Arrange
        var client = serviceProvider.GetRequiredService<IWeaviateClient>();
        
        // Act
        var result = await client.GraphQL().Get().
            WithClassName("IntegrationTest").
            WithFields(["name", "age"]).
            RunAsync();
        
        // Assert
        Assert.IsNotNull(result);
        Assert.IsNull(result.Errors);
        Assert.IsNotNull(result.Data);
        var responseData = JsonSerializer.Serialize(result.Data);
        Assert.IsTrue(responseData.Contains("{\"age\":0,\"name\":\"Person 0\"}"));
        Assert.IsTrue(responseData.Contains("{\"age\":1,\"name\":\"Person 1\"}"));
        Assert.IsTrue(responseData.Contains("{\"age\":2,\"name\":\"Person 2\"}"));
        Assert.IsTrue(responseData.Contains("{\"age\":3,\"name\":\"Person 3\"}"));
        Assert.IsTrue(responseData.Contains("{\"age\":4,\"name\":\"Person 4\"}"));
        Assert.IsTrue(responseData.Contains("{\"age\":5,\"name\":\"Person 5\"}"));
        Assert.IsTrue(responseData.Contains("{\"age\":6,\"name\":\"Person 6\"}"));
        Assert.IsTrue(responseData.Contains("{\"age\":7,\"name\":\"Person 7\"}"));
        Assert.IsTrue(responseData.Contains("{\"age\":8,\"name\":\"Person 8\"}"));
        Assert.IsTrue(responseData.Contains("{\"age\":9,\"name\":\"Person 9\"}"));
    }
    
    [TestMethod]
    [TestCategory("Integration")]
    public async Task GraphQLGetter_BM25_ShouldSucceed()
    {   // Arrange
        var client = serviceProvider.GetRequiredService<IWeaviateClient>();
        
        // Act
        var search = new BM25Builder().WithQuery("Person 1").FilterOn(["name"]);
        var result = await client.GraphQL().Get().
            WithClassName("IntegrationTest").
            WithFields(["name", "age"]).
            WithSearch(search).
            WithLimit(1).
            RunAsync();
        
        // Assert
        Assert.IsNotNull(result);
        Assert.IsNull(result.Errors);
        Assert.IsNotNull(result.Data);
        Assert.IsTrue(JsonSerializer.Serialize(result.Data).Contains("{\"age\":1,\"name\":\"Person 1\"}"));
    }
    
    [TestMethod]
    [TestCategory("Integration")]
    public async Task GraphQLGetter_Hybrid_ShouldSucceed()
    {   // Arrange
        var client = serviceProvider.GetRequiredService<IWeaviateClient>();
        
        // Act
        var search = new HybridBuilder().WithQuery("Person 3").WithVector([4.0f, 5.0f, 6.0f]);
        var result = await client.GraphQL().Get().
            WithClassName("IntegrationTest").
            WithFields(["name", "age"]).
            WithSearch(search).
            WithLimit(2).
            RunAsync();
        
        // Assert
        Assert.IsNotNull(result);
        Assert.IsNull(result.Errors);
        Assert.IsNotNull(result.Data);
        Console.WriteLine(JsonSerializer.Serialize(result.Data));
        Assert.IsTrue(JsonSerializer.Serialize(result.Data).Contains("{\"age\":3,\"name\":\"Person 3\"},{\"age\":4,\"name\":\"Person 4\"}"));
    }
    
    [TestMethod]
    [TestCategory("Integration")]
    public async Task GraphQLGetter_NearVector_ShouldSucceed()
    {   // Arrange
        var client = serviceProvider.GetRequiredService<IWeaviateClient>();
        
        // Act
        var search = new NearVectorBuilder().WithVector([3.0f, 4.0f, 5.0f]);
        var result = await client.GraphQL().Get().
            WithClassName("IntegrationTest").
            WithFields(["name", "age"]).
            WithSearch(search).
            WithLimit(2).
            RunAsync();
        
        // Assert
        Assert.IsNotNull(result);
        Assert.IsNull(result.Errors);
        Assert.IsNotNull(result.Data);
        Assert.IsTrue(JsonSerializer.Serialize(result.Data).Contains("{\"age\":2,\"name\":\"Person 2\"},{\"age\":3,\"name\":\"Person 3\"}"));
    }
}
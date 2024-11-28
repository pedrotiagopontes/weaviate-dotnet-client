using System.Text.Json;
using Microsoft.Extensions.Configuration;
using WeaviateClient.GraphQL.Model;
using WeaviateClient.GraphQL.QueryBuilder;
using Microsoft.Extensions.DependencyInjection;
using WeaviateClient.Client;
using WeaviateClient.Extensions;

var client = GetClientFromDIContainer();
//var client = GetDefaultClient();

await DeleteExistingClasses(client);
await CreatePersonObjects(client, 250);
await ExecuteBm25Search(client);
await ExecuteHybridSearch(client);
await ExecuteNearVectorSearch(client);
await IterateAllObjects(client);
return;


async Task DeleteExistingClasses(IWeaviateClient weaviateClient)
{
    var schema = await weaviateClient.Schema().GetAsync();
    foreach (var schemaClass in schema.Classes)
    {
        Console.WriteLine($"Removing existing class {schemaClass.Class}");
        await weaviateClient.Schema().DeleteAsync(schemaClass.Class);
    }
    Console.WriteLine("Deleted all existing classes");
}

async Task CreatePersonObjects(IWeaviateClient client, int objectsToCreate)
{
    Console.WriteLine($"Starting creation of {objectsToCreate} objects.");

    float[] startVector = [1.0f, 2.0f, 3.0f];
    for (var i = 0; i < objectsToCreate; i++)
    {
        var personFields = new Dictionary<string, object>
        {
            {"name", $"Person {i}"},
            {"age", i},
            {"counter", i}
        };
    
        await client.Data().Creator().
            WithClassName("Person").
            WithProperties(personFields).
            WithVector(startVector)
            .CreateAsync();
        
        startVector[0] += 1.0f;
        startVector[1] += 1.0f;
        startVector[2] += 1.0f;
    }

    Console.WriteLine($"Finished creation of {objectsToCreate} objects.");
}

async Task ExecuteBm25Search(IWeaviateClient client1)
{
    var searchPersonWithBm25 = new BM25Builder().WithQuery("Person 10").FilterOn(["name"]);
    var searchPersonWithBm25Query = client1.GraphQL().Get().
        WithClassName("Person").
        WithFields(["name", "age"]).
        WithAdditionalFields(["id", "vector"]).
        WithSearch(searchPersonWithBm25).
        WithLimit(1);

    var result = await searchPersonWithBm25Query.RunAsync();
    PrintQuery(searchPersonWithBm25Query.ToString(), result);

    searchPersonWithBm25Query.WithLimit(5);
    var result2 = await searchPersonWithBm25Query.RunAsync();
    PrintQuery(searchPersonWithBm25Query.ToString(), result2);
}

async Task ExecuteHybridSearch(IWeaviateClient weaviateClient1)
{
    var hybridQuerySearch = new HybridBuilder().WithQuery("Person 5").WithVector([10.0f, 11.0f, 12.0f]);
    var hybridQuery =  weaviateClient1.GraphQL().Get().
        WithClassName("Person").
        WithFields(["name"]).
        WithSearch(hybridQuerySearch).
        WithLimit(5);

    var result = await hybridQuery.RunAsync();
    PrintQuery(hybridQuery.ToString(), result);

    var hybridQueryWithAlphaSearch = new HybridBuilder()
        .WithQuery("Person 5")
        .WithProperties(["name"])
        .WithVector([10.0f, 11.0f, 12.0f])
        .WithAlpha(0.1f);

    var hybridQueryWithAlpha = weaviateClient1.GraphQL().Get().
        WithClassName("Person").
        WithFields(["name"]).
        WithSearch(hybridQueryWithAlphaSearch).
        WithLimit(5);

    var result2 = await hybridQueryWithAlpha.RunAsync();
    PrintQuery(hybridQueryWithAlpha.ToString(), result2);
}

async Task ExecuteNearVectorSearch(IWeaviateClient client2)
{
    var searchQuery = new NearVectorBuilder().WithVector([10.0f, 11.0f, 12.0f]).WithCertainty(0.1f);
    var fullQuery = client2.GraphQL().Get().
        WithClassName("Person").
        WithFields(["name"]).
        WithSearch(searchQuery).
        WithLimit(5);
    
    var result = await fullQuery.RunAsync();
    PrintQuery(fullQuery.ToString(), result);

    var searchQuery2 = new NearVectorBuilder().WithVector([1.0f, 2.0f, 3.0f]);
    var nearVectorQuery2 = client2.GraphQL().Get().
        WithClassName("Person").
        WithFields(["name"]).
        WithSearch(searchQuery2).
        WithLimit(5);
    
    var result2 = await nearVectorQuery2.RunAsync();
    PrintQuery(nearVectorQuery2.ToString(), result2);
}

async Task IterateAllObjects(IWeaviateClient client3)
{
    var cursorWithPersonId = "";
    var totalCount = 0;

    while (true)
    {
        var query = client3.GraphQL().Get()
            .WithClassName("Person")
            .WithFields(["counter"])
            .WithAdditionalFields(["id"])
            .WithAfter(cursorWithPersonId)
            .WithLimit(55);

        var result = await query.RunAsync();
        //PrintQuery(query.ToString(), result);
    
        if (!result.Data.ContainsKey("Get"))
            return;
    
        var data =  JsonSerializer.Deserialize<Get>(result.Data["Get"].ToString());
        if (data.Person.Count == 0)
        {
            Console.WriteLine("Finished iteration");
            Console.WriteLine($"Total count: {totalCount}");
            return;
        }
    
        totalCount += data.Person.Sum(person => person.Counter);
        cursorWithPersonId = data.Person[data.Person.Count - 1].AddicitionalFields["id"];
        Console.WriteLine($"Current count {totalCount} at cursor {cursorWithPersonId}");
    }
}

IWeaviateClient GetDefaultClient()
{
    var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();
    
    var options = new WeaviateClientOptions
    {
        BaseUrl = configuration["WCD_HOST_NAME"] ?? string.Empty,
        ApiKey = configuration["WCD_API_KEY"] ?? string.Empty,
        UserAgent = "local-test",
        OpenAIKey = configuration["OPENAI_API_KEY"] ?? string.Empty
    };

    return WeaviateClient.Client.WeaviateClient.CreateDefaultClient(options);
}

IWeaviateClient GetClientFromDIContainer()
{
    var services = new ServiceCollection();
    var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();

    services.AddSingleton<IConfiguration>(configuration);

    services.AddWeaviateClient(options =>
    {
        options.BaseUrl = configuration["WCD_HOST_NAME"] ?? string.Empty;
        options.ApiKey = configuration["WCD_API_KEY"] ?? string.Empty;
        options.UserAgent = "local-test";
        options.OpenAIKey = configuration["OPENAI_API_KEY"] ?? string.Empty;
    });
    
    var serviceProvider = services.BuildServiceProvider(); 
    return serviceProvider.GetRequiredService<IWeaviateClient>();
}

void PrintQuery(string query , GraphQLResponse result)
{
    Console.WriteLine("query:");
    Console.WriteLine(query);
    Console.WriteLine("result:");
    Console.WriteLine("\t" + JsonSerializer.Serialize(result));
    Console.WriteLine();
    Console.WriteLine("--------------------------------------------------------------------------");
}
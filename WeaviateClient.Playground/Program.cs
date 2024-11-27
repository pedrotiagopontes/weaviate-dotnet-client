using System.Text.Json;
using Microsoft.Extensions.Configuration;
using WeaviateClient.GraphQL.Model;
using WeaviateClient.GraphQL.QueryBuilder;

var client = InitializeWeaviateClient();
await DeleteExistingClasses(client);
await CreatePersonObjects(client, 250);
await ExecuteBm25Search(client);
await ExecuteHybridSearch(client);
await ExecuteNearVectorSearch(client);
await IterateAllObjects(client);
return;


async Task DeleteExistingClasses(WeaviateClient.Client.WeaviateClient weaviateClient)
{
    var schema = await weaviateClient.SchemaGetter().GetAsync();
    foreach (var schemaClass in schema.Classes)
    {
        Console.WriteLine($"Removing existing class {schemaClass.Class}");
        await weaviateClient.SchemaDeleter().DeleteAsync(schemaClass.Class);
    }
    Console.WriteLine("Deleted all existing classes");
}

async Task<Guid?> CreatePersonObjects(WeaviateClient.Client.WeaviateClient client, int objectsToCreate)
{
    Guid? firstPersonID = Guid.Empty;
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
    
        var person = await client.Data().Creator().
            WithClassName("Person").
            WithProperties(personFields).
            WithVector(startVector)
            .CreateAsync();

        if (i == 0)
        {
            // save first person ID to use in the cursor
            firstPersonID = person.Id;
        }
        
        startVector[0] += 1.0f;
        startVector[1] += 1.0f;
        startVector[2] += 1.0f;
    }

    Console.WriteLine($"Finished creation of {objectsToCreate} objects.");
    
    return firstPersonID;
}

async Task ExecuteBm25Search(WeaviateClient.Client.WeaviateClient client1)
{
    var searchPersonWithBm25 = new BM25Builder().WithQuery("Person 10").FilterOn(["name"]);
    var searchPersonWithBm25Query = client1.GraphQL().Get().
        WithClassName("Person").
        WithFields(["name", "age"]).
        WithAdditionalFields(["id", "vector"]).
        WithSearch(searchPersonWithBm25).
        WithLimit(1);

    var result = await searchPersonWithBm25Query.QueryAsync();
    PrintQuery(searchPersonWithBm25Query.ToString(), result);

    searchPersonWithBm25Query.WithLimit(5);
    var result2 = await searchPersonWithBm25Query.QueryAsync();
    PrintQuery(searchPersonWithBm25Query.ToString(), result2);
}

async Task ExecuteHybridSearch(WeaviateClient.Client.WeaviateClient weaviateClient1)
{
    var hybridQuerySearch = new HybridBuilder().WithQuery("Person 5").WithVector([10.0f, 11.0f, 12.0f]);
    var hybridQuery =  weaviateClient1.GraphQL().Get().
        WithClassName("Person").
        WithFields(["name"]).
        WithSearch(hybridQuerySearch).
        WithLimit(5);

    var result = await hybridQuery.QueryAsync();
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

    var result2 = await hybridQueryWithAlpha.QueryAsync();
    PrintQuery(hybridQueryWithAlpha.ToString(), result2);
}

async Task ExecuteNearVectorSearch(WeaviateClient.Client.WeaviateClient client2)
{
    var searchQuery = new NearVectorBuilder().WithVector([10.0f, 11.0f, 12.0f]).WithCertainty(0.1f);
    var fullQuery = client2.GraphQL().Get().
        WithClassName("Person").
        WithFields(["name"]).
        WithSearch(searchQuery).
        WithLimit(5);
    
    var result = await fullQuery.QueryAsync();
    PrintQuery(fullQuery.ToString(), result);

    var searchQuery2 = new NearVectorBuilder().WithVector([1.0f, 2.0f, 3.0f]);
    var nearVectorQuery2 = client2.GraphQL().Get().
        WithClassName("Person").
        WithFields(["name"]).
        WithSearch(searchQuery2).
        WithLimit(5);
    
    var result2 = await nearVectorQuery2.QueryAsync();
    PrintQuery(nearVectorQuery2.ToString(), result2);
}

WeaviateClient.Client.WeaviateClient InitializeWeaviateClient()
{
    var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();

    var hostAddress = configuration["WCD_HOST_NAME"] ?? string.Empty;
    var apiKey = configuration["WCD_API_KEY"] ?? string.Empty;
    var openAiKey = configuration["OPENAI_API_KEY"] ?? string.Empty;

    if (string.IsNullOrEmpty(hostAddress) || string.IsNullOrEmpty(apiKey))
    {
        throw new MissingFieldException("WCD_HOST_NAME or WCD_API_KEY not found");
    }

    var httpClient = new HttpClient();
    var weaviateClient2 = new WeaviateClient.Client.WeaviateClient(httpClient);
    weaviateClient2.
        WithBaseURl(hostAddress).
        WithApikey(apiKey).
        AcceptHeaders(["application/json"]).
        WithUserAgent("local-test").
        WithOpenAIKey(openAiKey);
    return weaviateClient2;
}

async Task IterateAllObjects(WeaviateClient.Client.WeaviateClient client3)
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

        var result = await query.QueryAsync();
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

void PrintQuery(string query , GraphQLResponse result)
{
    Console.WriteLine("query:");
    Console.WriteLine(query);
    Console.WriteLine("result:");
    Console.WriteLine("\t" + JsonSerializer.Serialize(result));
    Console.WriteLine();
    Console.WriteLine("--------------------------------------------------------------------------");
}
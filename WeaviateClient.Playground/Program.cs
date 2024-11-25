using System.Text.Json;
using DotNetEnv;
using WeaviateClient.GraphQL.QueryBuilder;

Env.Load();
var hostAddress = Environment.GetEnvironmentVariable("WCD_HOST_NAME");
if (hostAddress is null){
    Console.WriteLine("WCD_HOST_NAME not found");
    return;
}
var apiKey = Environment.GetEnvironmentVariable("WCD_API_KEY");
if (apiKey is null){
    Console.WriteLine("WCD_API_KEY not found");
    return;
}

var openAIKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

var httpClient = new HttpClient();
var client = new WeaviateClient.Client.WeaviateClient(httpClient);
client.
    WithBaseURl(hostAddress).
    WithApikey(apiKey).
    AcceptHeaders(["application/json"]).
    WithUserAgent("local-test").
    WithOpenAIKey(openAIKey);

await DeleteExistingClasses(client);
await CreatePersonObjects(client, 250);

await ExecuteBm25Search(client);
await ExecuteHybridSearch(client);
await ExecuteNearVectorSearch(client);
// TODO: Get all objects (hint: use the cursor api) and sum up the counter property

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

async Task CreatePersonObjects(WeaviateClient.Client.WeaviateClient client, int objectsToCreate)
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

async Task ExecuteBm25Search(WeaviateClient.Client.WeaviateClient client1)
{
    var searchPersonWithBm25 = new BM25Builder().WithQuery("Person 10").FilterOn(["name"]);
    var person10 = await client1.GraphQL().Get().
        WithClassName("Person").
        WithFields(["name", "age"]).
        WithSearch(searchPersonWithBm25).
        WithLimit(1).
        QueryAsync();

    Console.WriteLine($"BM25 search: {searchPersonWithBm25.Build()} with limit 1:");
    Console.WriteLine("\t" +JsonSerializer.Serialize(person10));

    var person10Limit5 = await client1.GraphQL().Get().
        WithClassName("Person").
        WithFields(["name", "age"]).
        WithSearch(searchPersonWithBm25).
        WithLimit(5).
        QueryAsync();

    Console.WriteLine($"BM25 search: {searchPersonWithBm25.Build()} with limit 5:");
    Console.WriteLine("\t" + JsonSerializer.Serialize(person10Limit5));
}

async Task ExecuteHybridSearch(WeaviateClient.Client.WeaviateClient weaviateClient1)
{
    var hybridQuery = new HybridBuilder().WithQuery("Person 5").WithVector([10.0f, 11.0f, 12.0f]);
    var hybridQueryResult = await weaviateClient1.GraphQL().Get().
        WithClassName("Person").
        WithFields(["name"]).
        WithSearch(hybridQuery).
        WithLimit(5).
        QueryAsync();

    Console.WriteLine($"hybrid search: {hybridQuery.Build()} with limit 5:");
    Console.WriteLine("\t" + JsonSerializer.Serialize(hybridQueryResult));

    var hybridQueryWithAlpha = new HybridBuilder()
        .WithQuery("Person 5")
        .WithProperties(["name"])
        .WithVector([10.0f, 11.0f, 12.0f])
        .WithAlpha(0.1f);

    var hybridQueryWithAlphaResult = await weaviateClient1.GraphQL().Get().
        WithClassName("Person").
        WithFields(["name"]).
        WithSearch(hybridQuery).
        WithLimit(5).
        QueryAsync();

    Console.WriteLine($"hybrid search: {hybridQueryWithAlpha.Build()} with limit 5:");
    Console.WriteLine("\t" + JsonSerializer.Serialize(hybridQueryWithAlphaResult));
}

async Task ExecuteNearVectorSearch(WeaviateClient.Client.WeaviateClient client2)
{
    var nearVectorQuery = new NearVectorBuilder().WithVector([10.0f, 11.0f, 12.0f]).WithCertainty(0.1f);
    var nearVectorQueryResult = await client2.GraphQL().Get().
        WithClassName("Person").
        WithFields(["name"]).
        WithSearch(nearVectorQuery).
        WithLimit(5).
        QueryAsync();
    Console.WriteLine($"nearVector search: {nearVectorQuery.Build()} with limit 5:");
    Console.WriteLine("\t" + JsonSerializer.Serialize(nearVectorQueryResult));

    var nearVectorQuery2 = new NearVectorBuilder().WithVector([1.0f, 2.0f, 3.0f]);
    var nearVectorQueryResult2 = await client2.GraphQL().Get().
        WithClassName("Person").
        WithFields(["name"]).
        WithSearch(nearVectorQuery2).
        WithLimit(5).
        QueryAsync();
    Console.WriteLine($"nearVector search: {nearVectorQuery2.Build()} with limit 5:");
    Console.WriteLine("\t" + JsonSerializer.Serialize(nearVectorQueryResult2));
}
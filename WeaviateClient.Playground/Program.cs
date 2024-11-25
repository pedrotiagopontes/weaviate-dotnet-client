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
    
// Get all objects using REST API
// var objects = await client.Data().Getter().GetAllAsync();
// foreach (var obj in objects)
// {
//     Console.WriteLine(JsonSerializer.Serialize(obj));
// }

// Get one object using REST API
// var obj1 = await client.Data().Getter().GetAsync(Guid.Parse("2b0fd838-e964-48f1-9899-6282475fe858"));
// Console.WriteLine(JsonSerializer.Serialize(obj1));

// Create an object using REST API
// var result = await client.Data().Creator().
//     WithClassName("Person").
//     WithProperties(new Dictionary<string, object>
//     {
//         {"name", "Pedro"},
//         {"age", 30}
//     }).
//     CreateAsync();
//     
// Console.WriteLine(JsonSerializer.Serialize(result));
//
// // Get objects using GraphQL
// var queryResult = await client.GraphQL()
//     .Get()
//     .WithClassName("Person")
//     .WithFields(["name", "age"])
//     .WithLimit(1)
//     .WithOffset(1)
//     .WithSearch()
//     .QueryAsync();
// Console.WriteLine("BM25 query:");
// Console.WriteLine(JsonSerializer.Serialize(queryResult.Data));

// var query = new HybridQueryBuilder().WithQuery("Fisherman that catches salmon").WithAlpha(0.5f);
// var hybridQuery = await client.GraphQL()
//     .Get()
//     .WithClassName("Question")
//     .WithFields(["question", "answer"])
//     .WithLimit(2)
//     .WithSearch(query)
//     .QueryAsync();
// Console.WriteLine("Hybrid query:");
// Console.WriteLine(JsonSerializer.Serialize(hybridQuery.Data));

var nearQuery = new NearVectorBuilder().WithVector([1.0f, 2.0f]);
var nearQueryResult = await client.GraphQL()
    .Get()
    .WithClassName("Question")
    .WithFields(["question", "answer"])
    .WithLimit(2)
    .WithSearch(nearQuery)
    .QueryAsync();
Console.WriteLine("nearQueryResult query:");
Console.WriteLine(JsonSerializer.Serialize(nearQueryResult));
using System.Text.Json;
using DotNetEnv;

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

var httpClient = new HttpClient();
var client = new WeaviateClient.Client.WeaviateClient(httpClient);
client.
    WithBaseURl(hostAddress).
    WithApikey(apiKey).
    WithUserAgent("local-test").
    AcceptHeaders(["application/json"]);
    
// Get all objects using REST API
var objects = await client.Data().Getter().GetAllAsync();
foreach (var obj in objects)
{
    Console.WriteLine(JsonSerializer.Serialize(obj));
}

// Get one object using REST API
// var obj1 = await client.Data().Getter().GetAsync(Guid.Parse("2b0fd838-e964-48f1-9899-6282475fe858"));
// Console.WriteLine(JsonSerializer.Serialize(obj1));

// Create an object using REST API
var result = await client.Data().Creator().
    WithClassName("Person").
    WithProperties(new Dictionary<string, object>
    {
        {"name", "Pedro"},
        {"age", 30}
    }).
    CreateAsync();
    
Console.WriteLine(JsonSerializer.Serialize(result));

// Get objects using GraphQL
var queryResult = await client.GraphQL()
    .Get()
    .WithClassName("Person")
    .WithFields(["name", "age"])
    .WithLimit(1)
    .WithOffset(1)
    .WithBM25("Pedro", ["name"])
    .QueryAsync();
Console.WriteLine("QueryResult:");
Console.WriteLine(JsonSerializer.Serialize(queryResult.Data));
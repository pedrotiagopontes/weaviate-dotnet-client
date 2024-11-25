namespace WeaviateClient.GraphQL;

using System.Text;
using System.Text.Json;
using Model;

public class GetQueryBuilder: GraphQlQueryBuilder
{
    private readonly HttpClient httpClient;
    private readonly string baseUrl;

    public GetQueryBuilder(HttpClient httpClient, string baseUrl)
    {
        this.httpClient = httpClient;
        this.baseUrl = $"{baseUrl}/graphql";
        Root("Get");
    }
    
    public GetQueryBuilder WithClassName(string className)
    {
        QueryBuilder.AppendLine($"    {className} {{");
        return this;
    }

    
    public GetQueryBuilder WithFields(string[] fields)
    {
        foreach (var field in fields)
        {
            QueryBuilder.AppendLine($"        {field}");
        }
        return this;
    }

    // // Adds a method (e.g., bm25) with arguments
    // public GetQueryBuilder Method(string methodName, IDictionary<string, string> arguments)
    // {
    //     var formattedArgs = string.Join(", ", arguments.Select(arg => $"{arg.Key}: {arg.Value}"));
    //     QueryBuilder.AppendLine($"    {methodName}({formattedArgs}) {{");
    //     return this;
    // }
    
    private string Build()
    {
        CloseBlock();
        CloseAll();
        return $"{{\n{QueryBuilder.ToString()}}}";
    }

    public async Task<GraphQLResponse> QueryAsync()
    {
        var rawQuery = Build();
        var query = new GraphQLQuery(rawQuery);
        var jsonContent = new StringContent(JsonSerializer.Serialize(query),
            Encoding.UTF8,
            "application/json");

        var response = await httpClient.PostAsync(baseUrl, jsonContent);

        response.EnsureSuccessStatusCode();
        var responseStream = await response.Content.ReadAsStreamAsync();
        var graphQlResponse = await JsonSerializer.DeserializeAsync<GraphQLResponse>(responseStream);

        return graphQlResponse ?? new GraphQLResponse();
    }
}

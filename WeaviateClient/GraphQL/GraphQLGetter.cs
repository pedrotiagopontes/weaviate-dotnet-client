namespace WeaviateClient.GraphQL;

using System.Text;
using System.Text.Json;
using Model;
using QueryBuilder;

public class GraphQLGetter
{
    private readonly HttpClient httpClient;
    private readonly string baseUrl;
    private readonly QueryBuilder.QueryBuilder queryBuilder;

    public GraphQLGetter(HttpClient httpClient, string baseUrl)
    {
        this.httpClient = httpClient;
        this.baseUrl = $"{baseUrl}/graphql";
        queryBuilder = new QueryBuilder.QueryBuilder();
        queryBuilder.Operation("Get");
    }
    
    public GraphQLGetter WithClassName(string className)
    {
        queryBuilder.WithClassName(className);
        return this;
    }

    public GraphQLGetter WithFields(string[] fields)
    {
        queryBuilder.WithFields(fields);
        return this;
    }

    public GraphQLGetter WithLimit(int limit)
    {
        queryBuilder.WithLimit(limit);
        return this;
    }

    public GraphQLGetter WithOffset(int offset)
    {
        queryBuilder.WithOffset(offset);
        return this;
    }

    public GraphQLGetter WithSearch(ISearchQueryBuilder searchQueryBuilder)
    {
        queryBuilder.WithSearch(searchQueryBuilder);
        return this;
    }

    public async Task<GraphQLResponse> QueryAsync()
    {
        var rawQuery = queryBuilder.Build();
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

namespace WeaviateClient.GraphQL;

public class GraphQL(HttpClient httpClient, string baseUrl)
{
    public GetQueryBuilder Get()
    {
        return new GetQueryBuilder(httpClient, baseUrl);
    }
}
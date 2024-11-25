namespace WeaviateClient.GraphQL;

public class GraphQL(HttpClient httpClient, string baseUrl)
{
    public GraphQLGetter Get()
    {
        return new GraphQLGetter(httpClient, baseUrl);
    }
}
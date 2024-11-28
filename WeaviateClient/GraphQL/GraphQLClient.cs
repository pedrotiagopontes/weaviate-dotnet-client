namespace WeaviateClient.GraphQL;

using Client;

public class GraphQLClient(WeaviateHttpClient httpClient):IGraphQLClient
{
    public IGraphQLGetter Get()
    {
        return new GraphQLGetter(httpClient, new QueryBuilder.QueryBuilder());
    }
}
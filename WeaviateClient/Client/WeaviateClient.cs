namespace WeaviateClient.Client;

using API.Object;
using API.Schema;
using GraphQL;

public class WeaviateClient : IWeaviateClient
{
    private readonly IObjectDataClient objectDataClient;
    private readonly ISchemaClient schemaClient;
    private readonly IGraphQLClient graphQlClient;
    
    // WeaviateClient main constructor to be used by DI container
    public WeaviateClient(
        IObjectDataClient objectDataClient,
        ISchemaClient schemaClient,
        IGraphQLClient graphQlClient)
    {
        this.objectDataClient = objectDataClient ?? throw new ArgumentNullException(nameof(objectDataClient));
        this.schemaClient = schemaClient ?? throw new ArgumentNullException(nameof(schemaClient));
        this.graphQlClient = graphQlClient ?? throw new ArgumentNullException(nameof(graphQlClient));
    }

    // CreateDefaultClient is factory helper method to create a WeaviateClient with default HttpClient
    // for a simpler UX when DI is not required
    public static WeaviateClient CreateDefaultClient(WeaviateClientOptions options)
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(options.BaseUrl);
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {options.ApiKey}");
        httpClient.DefaultRequestHeaders.Add("User-Agent", options.UserAgent);
        
        var weaviateHttpClient = new WeaviateHttpClient(httpClient);
        return new WeaviateClient(
            new ObjectDataClient(weaviateHttpClient),
            new SchemaClient(weaviateHttpClient),
            new GraphQLClient(weaviateHttpClient)
        );
    }
    
    // CreateWithHttpClient is factory helper method to create a WeaviateClient with custom HttpClient
    // for a simpler UX when DI is not required
    public static WeaviateClient CreateWithHttpClient(HttpClient httpClient)
    {
        var weaviateHttpClient = new WeaviateHttpClient(httpClient);
        return new WeaviateClient(
            new ObjectDataClient(weaviateHttpClient),
            new SchemaClient(weaviateHttpClient),
            new GraphQLClient(weaviateHttpClient)
        );
    }

    public IObjectDataClient Data() => objectDataClient;
    public ISchemaClient Schema() => schemaClient;
    public IGraphQLClient GraphQL() => graphQlClient;
}
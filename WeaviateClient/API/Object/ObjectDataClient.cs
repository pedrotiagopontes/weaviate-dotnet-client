namespace WeaviateClient.API.Object;

using Client;

public class ObjectDataClient : IObjectDataClient
{
    private readonly WeaviateHttpClient httpClient;

    public ObjectDataClient(WeaviateHttpClient httpClient)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public IObjectCreator Creator() => new ObjectCreator(httpClient);
    public IObjectGetter Getter() => new ObjectGetter(httpClient);
}
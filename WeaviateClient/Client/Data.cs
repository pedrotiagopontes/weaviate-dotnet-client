namespace weaviate_dotnet_client_poc.Client;

using Objects;

public class Data(HttpClient httpClient, string baseUrl)
{
    public ObjectCreator Creator()
    { 
        return new ObjectCreator(httpClient, baseUrl);
    }
    
    public ObjectGetter Getter()
    { 
        //TODO: review this to match patterns
        return new ObjectGetter(httpClient, baseUrl);
    }
}
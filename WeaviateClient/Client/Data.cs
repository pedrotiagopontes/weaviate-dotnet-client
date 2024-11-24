namespace weaviate_dotnet_client_poc.Client;

using Objects;

public class Data(HttpClient httpClient, string baseUrl)
{
    public ObjectCreator Creator()
    { 
        return new ObjectCreator(httpClient, baseUrl);
    }
    
    public ObjectService Getter()
    { 
        //TODO: review this to match patterns
        return new ObjectService(httpClient, baseUrl);
    }
}
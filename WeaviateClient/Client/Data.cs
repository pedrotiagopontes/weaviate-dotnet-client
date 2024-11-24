namespace WeaviateClient.Client;

using Object;

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
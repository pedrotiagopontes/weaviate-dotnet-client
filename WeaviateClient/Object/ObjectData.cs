namespace WeaviateClient.Object;

public class ObjectData(HttpClient httpClient, string baseUrl)
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
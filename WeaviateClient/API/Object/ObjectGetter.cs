namespace WeaviateClient.API.Object;

using Client;
using Model;

public class ObjectGetter(WeaviateHttpClient httpClient): IObjectGetter
{
    private const string ResourcePath = "objects";

    public async Task<WeaviateObject> GetAsync(Guid uuid)
    {
        return await httpClient.GetAsync<WeaviateObject>(ResourcePath, uuid);
    }
    
    public async Task<List<WeaviateObject>> GetAllAsync()
    {
        return await httpClient.GetAllAsync<List<WeaviateObject>>(ResourcePath);
    }
}
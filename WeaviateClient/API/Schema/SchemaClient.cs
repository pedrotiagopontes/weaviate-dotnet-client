namespace WeaviateClient.API.Schema;

using Client;
using Model;

public class SchemaClient(WeaviateHttpClient httpClient):ISchemaClient
{
    private const string ResourcePath = "schema";

    public async Task<Schema> GetAsync()
    {
        return await httpClient.GetAllAsync<Schema>(ResourcePath);
    }
    
    public async Task DeleteAsync(string className)
    { 
        await httpClient.DeleteAsync(ResourcePath, className);
    }
}
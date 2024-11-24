namespace WeaviateClient.Object;

using System.Text.Json;
using Model;

public class ObjectGetter(HttpClient httpClient, string baseUrl)
{
    private const string ObjectsPath = "objects";

    public async Task<WeaviateObject> GetAsync(Guid uuid)
    {
        var requestUri = $"{baseUrl}/{ObjectsPath}/{uuid}";
        var responseStream = await httpClient.GetStreamAsync(requestUri);
        var obj = await JsonSerializer.DeserializeAsync<WeaviateObject>(responseStream);

        return obj ?? new WeaviateObject();
    }
    
    public async Task<List<WeaviateObject>> GetAllAsync()
    {
        var requestUri = $"{baseUrl}/{ObjectsPath}/";
        var responseStream = await httpClient.GetStreamAsync(requestUri);
        var responseObject = await JsonSerializer.DeserializeAsync<ObjectListResponse>(responseStream);

        return responseObject?.Objects ?? [];
    }
}
namespace weaviate_dotnet_client_poc.Objects;

using System.Text;
using System.Text.Json;
using Models;


/// <summary>
/// 
/// </summary>
public class ObjectService(HttpClient httpClient, string baseUrl)
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
        var responseObject = await JsonSerializer.DeserializeAsync<ObjectResponse>(responseStream);

        return responseObject?.Objects ?? [];
    }
    
    public async Task<WeaviateObject> CreateAsync(WeaviateObject weaviateObject)
    {
        var requestUri = $"{baseUrl}/{ObjectsPath}"; 
        var jsonContent = new StringContent(JsonSerializer.Serialize(weaviateObject),
            Encoding.UTF8,
            "application/json");

        var response = await httpClient.PostAsync(requestUri, jsonContent);

        response.EnsureSuccessStatusCode();
        var responseStream = await response.Content.ReadAsStreamAsync();
        var createdObject = await JsonSerializer.DeserializeAsync<WeaviateObject>(responseStream);

        return createdObject ?? new WeaviateObject();
    }

}
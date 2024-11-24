namespace WeaviateClient.Object;

using System.Text;
using System.Text.Json;
using Model;

public class ObjectCreator(HttpClient httpClient, string baseUrl)
{
    private const string ObjectsPath = "objects";
    
    private readonly HttpClient httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    private readonly string baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
    
    private readonly WeaviateObject weaviateObject = new();

    public ObjectCreator WithClassName(string className)
    {
        weaviateObject.Class = className;
        return this;
    }

    public ObjectCreator WithProperties(Dictionary<string, object> properties)
    {
        weaviateObject.Properties = properties;
        return this;
    }

    public ObjectCreator WithProperty(string key, object value)
    {
        weaviateObject.Properties ??= new Dictionary<string, object>();
        weaviateObject.Properties[key] = value;
        return this;
    }

    public ObjectCreator WithVector(float[] vector)
    {
        weaviateObject.Vector = vector;
        return this;
    }

    public ObjectCreator WithNamedVectors(Dictionary<string, float[]> vectors)
    {
        weaviateObject.Vectors = vectors;
        return this;
    }

    public ObjectCreator WithNamedVector(string key, float[] value)
    {
        weaviateObject.Vectors ??= new Dictionary<string, float[]>();
        weaviateObject.Vectors[key] = value;
        return this;
    }
    
    public async Task<WeaviateObject> CreateAsync()
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
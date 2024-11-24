namespace weaviate_dotnet_client_poc.Objects;


using Models;

public class ObjectCreator
{
    private WeaviateObject _weaviateObject;
    private ObjectService _objectService;

    public ObjectCreator(HttpClient httpClient, string baseUrl)
    {
        _objectService = new ObjectService(httpClient, baseUrl);
        _weaviateObject = new WeaviateObject();
    }

    public ObjectCreator WithClassName(string className)
    {
        _weaviateObject.Class = className;
        return this;
    }

    public ObjectCreator WithProperties(Dictionary<string, object> properties)
    {
        _weaviateObject.Properties = properties;
        return this;
    }

    public ObjectCreator WithProperty(string key, object value)
    {
        _weaviateObject.Properties ??= new Dictionary<string, object>();
        _weaviateObject.Properties[key] = value;
        return this;
    }

    public ObjectCreator WithVector(float vector)
    {
        _weaviateObject.Vector = vector;
        return this;
    }

    public ObjectCreator WithVectors(Dictionary<string, float> vectors)
    {
        _weaviateObject.Vectors = vectors;
        return this;
    }

    public ObjectCreator WithAdditionalVector(string key, float value)
    {
        _weaviateObject.Vectors ??= new Dictionary<string, float>();
        _weaviateObject.Vectors[key] = value;
        return this;
    }

    public ObjectCreator WithTenant(string tenant)
    {
        _weaviateObject.Tenant = tenant;
        return this;
    }

    public async Task<WeaviateObject> CreateAsync()
    {
        return await _objectService.CreateAsync(_weaviateObject);
    }
}
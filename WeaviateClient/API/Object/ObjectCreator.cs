namespace WeaviateClient.API.Object;

using Client;
using Model;

public class ObjectCreator(WeaviateHttpClient httpClient):IObjectCreator
{
    private const string ResourcePath = "objects";
    
    private readonly WeaviateHttpClient httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    
    private readonly WeaviateObject weaviateObject = new();

    public IObjectCreator WithId(Guid id)
    {
        weaviateObject.Id = id;
        return this;
    }
    
    public IObjectCreator WithClassName(string className)
    {
        weaviateObject.Class = className;
        return this;
    }

    public IObjectCreator WithProperties(Dictionary<string, object> properties)
    {
        weaviateObject.Properties = properties;
        return this;
    }

    public IObjectCreator WithProperty(string key, object value)
    {
        weaviateObject.Properties ??= new Dictionary<string, object>();
        weaviateObject.Properties[key] = value;
        return this;
    }

    public IObjectCreator WithVector(float[] vector)
    {
        weaviateObject.Vector = vector;
        return this;
    }

    public IObjectCreator WithNamedVectors(Dictionary<string, float[]> vectors)
    {
        weaviateObject.Vectors = vectors;
        return this;
    }

    public IObjectCreator WithNamedVector(string key, float[] value)
    {
        weaviateObject.Vectors ??= new Dictionary<string, float[]>();
        weaviateObject.Vectors[key] = value;
        return this;
    }
    
    public async Task<WeaviateObject> CreateAsync()
    {
        return await httpClient.PostAsync<WeaviateObject, WeaviateObject>(ResourcePath, weaviateObject);
    }
}
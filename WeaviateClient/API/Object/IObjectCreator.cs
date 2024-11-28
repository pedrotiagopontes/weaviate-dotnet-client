using WeaviateClient.API.Model;

namespace WeaviateClient.API.Object;

public interface IObjectCreator
{
    IObjectCreator WithId(Guid id);
    IObjectCreator WithClassName(string className);
    IObjectCreator WithProperties(Dictionary<string, object> properties);
    IObjectCreator WithProperty(string key, object value);
    IObjectCreator WithVector(float[] vector);
    Task<WeaviateObject> CreateAsync();
}
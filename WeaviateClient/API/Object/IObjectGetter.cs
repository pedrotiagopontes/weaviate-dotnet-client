namespace WeaviateClient.API.Object;

using Model;

public interface IObjectGetter
{
    Task<WeaviateObject> GetAsync(Guid uuid);
    Task<List<WeaviateObject>> GetAllAsync();
}
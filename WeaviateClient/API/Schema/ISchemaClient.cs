namespace WeaviateClient.API.Schema;

using Model;

public interface ISchemaClient
{
    Task<Schema> GetAsync();
    Task DeleteAsync(string className);
}
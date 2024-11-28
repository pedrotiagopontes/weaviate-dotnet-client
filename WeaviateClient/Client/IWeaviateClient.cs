using WeaviateClient.API.Object;
using WeaviateClient.API.Schema;
using WeaviateClient.GraphQL;

namespace WeaviateClient.Client;

public interface IWeaviateClient
{
    IObjectDataClient Data();
    ISchemaClient Schema();
    IGraphQLClient GraphQL();
}
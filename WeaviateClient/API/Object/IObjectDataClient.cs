namespace WeaviateClient.API.Object;

public interface IObjectDataClient
{
    IObjectCreator Creator();
    IObjectGetter Getter();
}
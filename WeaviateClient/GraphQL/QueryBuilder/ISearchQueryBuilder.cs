namespace WeaviateClient.GraphQL.QueryBuilder;

public interface ISearchQueryBuilder
{
    string SearchType();
    string Build();
}
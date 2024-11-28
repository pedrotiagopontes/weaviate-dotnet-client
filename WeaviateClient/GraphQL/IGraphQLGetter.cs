namespace WeaviateClient.GraphQL;

using Model;
using QueryBuilder;

public interface IGraphQLGetter
{
    IGraphQLGetter WithClassName(string className);
    IGraphQLGetter WithFields(string[] fields);
    IGraphQLGetter WithAdditionalFields(string[] additionalFields);
    IGraphQLGetter WithLimit(int limit);
    IGraphQLGetter WithOffset(int offset);
    IGraphQLGetter WithSearch(ISearchQueryBuilder searchQueryBuilder);
    IGraphQLGetter WithAfter(string cursor);
    Task<GraphQLResponse> RunAsync();
}
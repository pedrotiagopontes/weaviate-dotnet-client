namespace WeaviateClient.GraphQL.QueryBuilder;

public interface IQueryBuilder
{
    IQueryBuilder Operation(string operation);
    IQueryBuilder WithClassName(string className);
    IQueryBuilder WithFields(string[] fields);
    IQueryBuilder WithAdditionalFields(string[] additionalFields);
    IQueryBuilder WithLimit(int limit);
    IQueryBuilder WithOffset(int offset);
    IQueryBuilder WithSearch(ISearchQueryBuilder searchQueryBuilder);
    IQueryBuilder WithAfter(string cursor);
    string Build();
}
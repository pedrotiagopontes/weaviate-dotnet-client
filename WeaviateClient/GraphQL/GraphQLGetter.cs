namespace WeaviateClient.GraphQL;

using Client;
using Model;
using QueryBuilder;

public class GraphQLGetter: IGraphQLGetter
{
    private readonly WeaviateHttpClient httpClient;
    private readonly IQueryBuilder queryBuilder;

    public GraphQLGetter(WeaviateHttpClient httpClient, IQueryBuilder queryBuilder)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.queryBuilder = queryBuilder ?? throw new ArgumentNullException(nameof(queryBuilder));
        queryBuilder.Operation("Get");
    }
    
    public IGraphQLGetter WithClassName(string className)
    {
        queryBuilder.WithClassName(className);
        return this;
    }

    public IGraphQLGetter WithFields(string[] fields)
    {
        queryBuilder.WithFields(fields);
        return this;
    }

    public IGraphQLGetter WithAdditionalFields(string[] additionalFields)
    {
        queryBuilder.WithAdditionalFields(additionalFields);
        return this;
    }

    public IGraphQLGetter WithLimit(int limit)
    {
        queryBuilder.WithLimit(limit);
        return this;
    }

    public IGraphQLGetter WithOffset(int offset)
    {
        queryBuilder.WithOffset(offset);
        return this;
    }

    public IGraphQLGetter WithSearch(ISearchQueryBuilder searchQueryBuilder)
    {
        queryBuilder.WithSearch(searchQueryBuilder);
        return this;
    }
    
    public IGraphQLGetter WithAfter(string cursor)
    {
        if (string.IsNullOrWhiteSpace(cursor))
        {
            // we want to explicitly ignore empty cursor for a better UX when using it.
            // This way the user doens't need to care about it.
            return this;
        }
        
        queryBuilder.WithAfter(cursor);
        return this;
    }
    public override string ToString()
    {
        return queryBuilder.Build();
    }

    public async Task<GraphQLResponse> RunAsync()
    {
        var rawQuery = queryBuilder.Build();
        var query = new GraphQLQuery(rawQuery);
        return await httpClient.PostAsync<GraphQLQuery, GraphQLResponse>("graphql", query);
    }
}

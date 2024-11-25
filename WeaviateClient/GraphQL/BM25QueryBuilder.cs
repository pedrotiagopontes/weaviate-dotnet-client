namespace WeaviateClient.GraphQL;
public class BM25QueryBuilder
{
    private string? bm25Query;
    private readonly List<string> bm25FilterFields = new();

    public BM25QueryBuilder WithQuery(string query)
    {
        bm25Query = query;
        return this;
    }

    public BM25QueryBuilder FilterOn(string[] fields)
    {
        bm25FilterFields.AddRange(fields);
        return this;
    }

    public string Build()
    {
        if (string.IsNullOrEmpty(bm25Query))
        {
            throw new InvalidOperationException("BM25 query string must be specified.");
        }
        
        var bm25Parameter = new
        {
            query = bm25Query,
            properties = bm25FilterFields.Count > 0 ? $"[{string.Join(", ", bm25FilterFields.Select(p => $"\"{p}\""))}]" : "[]"
        };

        return bm25FilterFields.Count > 0
            ? $"{{ query: \"{bm25Parameter.query}\", properties: {bm25Parameter.properties} }}"
            : $"{{ query: \"{bm25Parameter.query}\" }}";
    }
}

namespace WeaviateClient.GraphQL;

using System.Text;

public class QueryBuilder
{
    private readonly StringBuilder queryBuilder = new();
    private string? root;
    private string? className;
    private readonly List<string> fields = [];
    private readonly Dictionary<string, string> parameters = new();
    private readonly BM25QueryBuilder bm25QueryBuilder = new();

    public QueryBuilder Operation(string operationName)
    {
        this.root = operationName;
        return this;
    }

    public QueryBuilder WithClassName(string className)
    {
        this.className = className;
        return this;
    }

    public QueryBuilder WithFields(string[] fields)
    {
        this.fields.AddRange(fields);
        return this;
    }

    public QueryBuilder WithParameter(string key, object value)
    {
        parameters[key] = value.ToString();
        return this;
    }

    public QueryBuilder WithLimit(int limit)
    {
        return WithParameter("limit", limit);
    }

    public QueryBuilder WithOffset(int offset)
    {
        return WithParameter("offset", offset);
    }
    
    public QueryBuilder WithBM25(string query)
    {
        bm25QueryBuilder.WithQuery(query);
        WithParameter("bm25", bm25QueryBuilder.Build());
        return this;
    }
    
    public QueryBuilder WithBM25(string query, string[] filterFields)
    {
        bm25QueryBuilder.WithQuery(query).FilterOn(filterFields);
        WithParameter("bm25", bm25QueryBuilder.Build());
        return this;
    }

    public string Build()
    {
        if (string.IsNullOrEmpty(root) || string.IsNullOrEmpty(className))
        {
            throw new InvalidOperationException("Both root and class name must be specified.");
        }

        queryBuilder.AppendLine($"{root} {{");
        queryBuilder.Append($"    {className}");

        // Append parameters if any
        if (parameters.Count > 0)
        {
            var formattedParams = string.Join(", ", parameters.Select(p => $"{p.Key}: {p.Value}"));
            queryBuilder.Append($" ({formattedParams})");
        }

        queryBuilder.AppendLine(" {");

        // Append fields
        foreach (var field in fields)
        {
            queryBuilder.AppendLine($"        {field}");
        }

        // Close blocks
        queryBuilder.AppendLine("    }");
        queryBuilder.AppendLine("}");

        return $"{{\n{queryBuilder.ToString()}}}";
    }
}

namespace WeaviateClient.GraphQL.QueryBuilder;

using System.Text;

public class QueryBuilder
{
    private readonly StringBuilder queryBuilder = new();
    private string? root;
    private string? className;
    private readonly List<string> fields = [];
    private readonly Dictionary<string, string> parameters = new();
    private bool hasSearchQuery;

    public QueryBuilder Operation(string operationName)
    {
        this.root = operationName;
        return this;
    }

    public QueryBuilder WithClassName(string name)
    {
        this.className = name;
        return this;
    }

    public QueryBuilder WithFields(string[] requestedFields)
    {
        this.fields.AddRange(requestedFields);
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

    public QueryBuilder WithAfter(string cursor)
    {
        return WithParameter("after", $"\"{cursor}\"");
    }
    
    public QueryBuilder WithSearch(ISearchQueryBuilder searchQueryBuilder)
    {
        if (hasSearchQuery)
        {
            throw new ArgumentException("can only configure one search parameter per query");
        }
        WithParameter(searchQueryBuilder.SearchType(), searchQueryBuilder.Build());
        hasSearchQuery = true;
        return this;
    }
    
    private QueryBuilder WithParameter(string key, object value)
    {
        parameters[key] = value.ToString() ?? throw new ArgumentException("value cannot be null");
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
        
        if (parameters.Count > 0)
        {
            ValidateCursorParameters();
            var formattedParams = string.Join(", ", parameters.Select(p => $"{p.Key}: {p.Value}"));
            queryBuilder.Append($" ({formattedParams})");
        }

        queryBuilder.AppendLine(" {");
        
        foreach (var field in fields)
        {
            queryBuilder.AppendLine($"        {field}");
        }

        // Close blocks
        queryBuilder.AppendLine("    }");
        queryBuilder.AppendLine("}");

        return $"{{\n{queryBuilder}}}";
    }

    private void ValidateCursorParameters()
    {
        if (!parameters.ContainsKey("after")) return;
        
        if (!parameters.ContainsKey("limit"))
        {
            throw new InvalidOperationException("Both after and limit must be specified to use cursor.");
        }

        if (parameters.Count > 2)
        {
            throw new InvalidOperationException("Only one of after and limit can be specified to use cursor.");
        }
    }
}

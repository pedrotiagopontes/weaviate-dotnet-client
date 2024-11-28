namespace WeaviateClient.GraphQL.QueryBuilder;

using System.Text;

public class QueryBuilder: IQueryBuilder
{
    private string? root;
    private string? className;
    private readonly List<string> fields = [];
    private readonly List<string> additionalFields = [];
    private readonly Dictionary<string, string> parameters = new();
    private bool hasSearchQuery;

    public IQueryBuilder Operation(string operationName)
    {
        this.root = operationName;
        return this;
    }

    public IQueryBuilder WithClassName(string name)
    {
        this.className = name;
        return this;
    }

    public IQueryBuilder WithFields(string[] requestedFields)
    {
        this.fields.AddRange(requestedFields);
        return this;
    }
    
    public IQueryBuilder WithAdditionalFields(string[] requestedFields)
    {
        this.additionalFields.AddRange(requestedFields);
        return this;
    }

    public IQueryBuilder WithLimit(int limit)
    {
        return WithParameter("limit", limit);
    }

    public IQueryBuilder WithOffset(int offset)
    {
        return WithParameter("offset", offset);
    }

    public IQueryBuilder WithAfter(string cursor)
    {
        return WithParameter("after", $"\"{cursor}\"");
    }
    
    public IQueryBuilder WithSearch(ISearchQueryBuilder searchQueryBuilder)
    {
        if (hasSearchQuery)
        {
            throw new ArgumentException("can only configure one search parameter per query");
        }
        WithParameter(searchQueryBuilder.SearchType(), searchQueryBuilder.Build());
        hasSearchQuery = true;
        return this;
    }
    
    private IQueryBuilder WithParameter(string key, object value)
    {
        parameters[key] = value.ToString() ?? throw new ArgumentException("value cannot be null");
        return this;
    }

    public string Build()
    {
        var queryBuilder = new StringBuilder();
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

        // open fields 
        queryBuilder.AppendLine(" {");
        foreach (var field in fields)
        {
            queryBuilder.AppendLine($"        {field}");
        }

        if (additionalFields.Count > 0)
        {
            queryBuilder.Append("        _additional { ");
            foreach (var additionalField in additionalFields)
            {
                queryBuilder.Append($"{additionalField} ");
            }
            queryBuilder.AppendLine("}");
        }
        
        //close fields
        queryBuilder.AppendLine("    }");
        
        //close query
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

namespace WeaviateClient.GraphQL;

using System.Text;

public class GraphQlQueryBuilder
{
    private readonly StringBuilder queryBuilder = new StringBuilder();
    private string? root;
    private string? className;
    private readonly List<string> proprieties = [];
    private readonly Dictionary<string, string> parameters = new Dictionary<string, string>();

    public GraphQlQueryBuilder Operation(string operationName)
    {
        this.root = operationName;
        return this;
    }

    public GraphQlQueryBuilder WithClassName(string className)
    {
        this.className = className;
        return this;
    }

    public GraphQlQueryBuilder WithProprieties(string[] fields)
    {
        this.proprieties.AddRange(fields);
        return this;
    }

    public GraphQlQueryBuilder WithParameter(string key, object value)
    {
        parameters[key] = value is string ? $"\"{value}\"" : value.ToString();
        return this;
    }

    public GraphQlQueryBuilder WithLimit(int limit)
    {
        return WithParameter("limit", limit);
    }

    public GraphQlQueryBuilder WithOffset(int offset)
    {
        return WithParameter("offset", offset);
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
        foreach (var field in proprieties)
        {
            queryBuilder.AppendLine($"        {field}");
        }

        // Close blocks
        queryBuilder.AppendLine("    }");
        queryBuilder.AppendLine("}");

        return $"{{\n{queryBuilder.ToString()}}}";
    }
}

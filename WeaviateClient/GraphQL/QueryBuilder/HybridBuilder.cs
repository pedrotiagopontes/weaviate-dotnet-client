namespace WeaviateClient.GraphQL.QueryBuilder;

public class HybridBuilder:ISearchQueryBuilder
{
    private string? hybridQuery;
    private float? alpha;
    private float[]? vector;
    private readonly List<string> properties = new();
    private string? fusionType;

    public HybridBuilder WithQuery(string query)
    {
        hybridQuery = query;
        return this;
    }

    public HybridBuilder WithAlpha(float alphaValue)
    {
        if (alphaValue is < 0 or > 1)
        {
            throw new ArgumentException("Alpha must be between 0 and 1.", nameof(alphaValue));
        }

        alpha = alphaValue;
        return this;
    }

    public HybridBuilder WithVector(float[] vectorValue)
    {
        vector = vectorValue;
        return this;
    }

    public HybridBuilder WithProperties(string[] fields)
    {
        properties.AddRange(fields);
        return this;
    }

    public HybridBuilder WithFusionType(string fusionTypeValue)
    {
        fusionType = fusionTypeValue;
        return this;
    }

    public string SearchType()
    {
        return "hybrid";
    }

    public string Build()
    {
        if (string.IsNullOrEmpty(hybridQuery))
        {
            throw new InvalidOperationException("Hybrid query string must be specified.");
        }

        var queryParts = new List<string>
        {
            $"query: \"{hybridQuery}\""
        };

        if (alpha.HasValue)
        {
            queryParts.Add($"alpha: {alpha.Value.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture)}");
        }

        if (vector is { Length: > 0 })
        {
            var formattedVector = string.Join(", ", vector.Select(v => v.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture)));
            queryParts.Add($"vector: [{formattedVector}]");
        }

        if (properties.Count > 0)
        {
            var formattedProperties = string.Join(", ", properties.Select(p => $"\"{p}\""));
            queryParts.Add($"properties: [{formattedProperties}]");
        }

        if (!string.IsNullOrEmpty(fusionType))
        {
            queryParts.Add($"fusionType: \"{fusionType}\"");
        }

        return $"{{ {string.Join(", ", queryParts)} }}";
    }
}


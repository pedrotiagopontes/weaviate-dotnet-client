namespace WeaviateClient.GraphQL.QueryBuilder;

public class NearVectorBuilder:ISearchQueryBuilder
{
    private float[]? vector;
    private float? distance;
    private float? certainty;

    public NearVectorBuilder WithVector(float[] vectorValue)
    {
        if (vectorValue == null || vectorValue.Length == 0)
        {
            throw new ArgumentException("Vector must not be null or empty.", nameof(vectorValue));
        }

        vector = vectorValue;
        return this;
    }

    public NearVectorBuilder WithDistance(float distanceValue)
    {
        if (certainty.HasValue)
        {
            throw new InvalidOperationException("Distance and certainty cannot be used together.");
        }

        if (distanceValue < 0)
        {
            throw new ArgumentException("Distance must be non-negative.", nameof(distanceValue));
        }

        distance = distanceValue;
        return this;
    }

    public NearVectorBuilder WithCertainty(float certaintyValue)
    {
        if (distance.HasValue)
        {
            throw new InvalidOperationException("Distance and certainty cannot be used together.");
        }

        if (certaintyValue is < 0 or > 1)
        {
            throw new ArgumentException("Certainty must be between 0 and 1.", nameof(certaintyValue));
        }

        certainty = certaintyValue;
        return this;
    }

    public string SearchType()
    {
        return "nearVector";
    }

    public string Build()
    {
        if (vector == null || vector.Length == 0)
        {
            throw new InvalidOperationException("A vector must be specified.");
        }

        var queryParts = new List<string>
        {
            $"vector: [{string.Join(", ", vector.Select(v => v.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture)))}]"
        };

        if (distance.HasValue)
        {
            queryParts.Add($"distance: {distance.Value.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture)}");
        }

        if (certainty.HasValue)
        {
            queryParts.Add($"certainty: {certainty.Value.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture)}");
        }

        return $"{{ {string.Join(", ", queryParts)} }}";
    }
}


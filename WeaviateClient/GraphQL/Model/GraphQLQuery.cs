namespace WeaviateClient.GraphQL.Model;

using System.Text.Json.Serialization;

public class GraphQLQuery
{
    [JsonPropertyName("query")]
    public string Query { get; set; }

    public GraphQLQuery(string rawQuery)
    {
        var cleanedQuery = rawQuery
            .Replace("\n", " ")  // Replace newlines with spaces
            .Replace("\r", "")   // Remove carriage returns
            .Trim();             // Trim leading and trailing whitespace
        
        // Collapse multiple spaces into a single space
        cleanedQuery = System.Text.RegularExpressions.Regex.Replace(cleanedQuery, @"\s+", " ");
        
        Query = cleanedQuery;
    }
}
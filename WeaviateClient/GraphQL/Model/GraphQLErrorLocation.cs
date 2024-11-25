namespace WeaviateClient.GraphQL.Model;

using System.Text.Json.Serialization;

public class GraphQLErrorLocation
{
    [JsonPropertyName("line")]
    public int Line { get; set; }
    
    [JsonPropertyName("column")]
    public int Column { get; set; }
}
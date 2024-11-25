namespace WeaviateClient.GraphQL.Model;

using System.Text.Json.Serialization;

public class GrapQLError
{
    [JsonPropertyName("message")]
    public String Message { get; set; }
    
    [JsonPropertyName("path")]
    public String[] Path { get; set; }
    
    [JsonPropertyName("location")]
    public GraphQLErrorLocation[] Locations { get; set; }
}
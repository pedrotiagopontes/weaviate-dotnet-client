namespace WeaviateClient.GraphQL.Model;

using System.Text.Json.Serialization;

public class GraphQLResponse
{
    [JsonPropertyName("data")]
    public Dictionary<string,object> Data { get; set; }
    
    [JsonPropertyName("errors")]
    public GrapQLError[] Errors { get; set; }
}
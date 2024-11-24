namespace WeaviateClient.Models;

using System.Text.Json.Serialization;

public class ObjectResponse
{
    [JsonPropertyName("deprecations")]
    public object Deprecations { get; set; }

    [JsonPropertyName("objects")]
    public List<WeaviateObject> Objects { get; set; }

    [JsonPropertyName("totalResults")]
    public int TotalResults { get; set; }
}
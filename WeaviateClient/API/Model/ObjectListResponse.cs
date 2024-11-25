namespace WeaviateClient.API.Model;

using System.Text.Json.Serialization;

public class ObjectListResponse
{
    [JsonPropertyName("deprecations")]
    public object Deprecations { get; set; }

    [JsonPropertyName("objects")]
    public List<WeaviateObject> Objects { get; set; }

    [JsonPropertyName("totalResults")]
    public int TotalResults { get; set; }
}
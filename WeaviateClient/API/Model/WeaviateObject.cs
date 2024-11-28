namespace WeaviateClient.API.Model;

using System;
using System.Text.Json.Serialization;

public class WeaviateObject
{
    // ID of the Object (UUID format)
    [JsonPropertyName("id")]
    public Guid? Id { get; set; }
    
    // Class of the Object, defined in the schema
    [JsonPropertyName("class")]
    public string Class { get; set; }
    
    // Timestamp of creation of this Object in milliseconds since epoch UTC
    [JsonPropertyName("creationTimeUnix")]
    public long? CreationTimeUnix { get; set; }

    // Timestamp of the last Object update in milliseconds since epoch UTC
    [JsonPropertyName("lastUpdateTimeUnix")]
    public long? LastUpdateTimeUnix { get; set; }

    // Properties
    [JsonPropertyName("properties")]
    public Dictionary<string, object>? Properties { get; set; }
    
    // Vectors associated with the Object
    [JsonPropertyName("vector")]
    public float[]? Vector { get; set; }
}
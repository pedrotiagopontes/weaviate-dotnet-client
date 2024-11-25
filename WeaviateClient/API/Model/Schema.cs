namespace WeaviateClient.API.Model;

using System;
using System.Text.Json.Serialization;

public class Schema
{
    [JsonPropertyName("classes")]
    public SchemaClass[] Classes { get; set; }
}

public class SchemaClass
{
    [JsonPropertyName("class")]
    public string Class { get; set; }
    
    [JsonPropertyName("description")]
    public string Description { get; set; }
}
using System.Text.Json.Serialization;

public class Person
{
    [JsonPropertyName("age")]
    public int Age { get; set; }

    [JsonPropertyName("counter")]
    public int Counter { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("_additional")]
    public Dictionary<string, string> AddicitionalFields { get; set; }
}
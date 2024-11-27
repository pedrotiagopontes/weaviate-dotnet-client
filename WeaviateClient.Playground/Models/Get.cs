using System.Text.Json.Serialization;

public class Get
{
    [JsonPropertyName("Person")]
    public List<Person> Person { get; set; }
}
namespace WeaviateClient.API.Object;

using System.Text.Json;
using Model;

public class SchemaGetter(HttpClient httpClient, string baseUrl)
{
    private const string ResourcePath = "schema";

    public async Task<Schema> GetAsync()
    {
        var requestUri = $"{baseUrl}/{ResourcePath}/";
        var responseStream = await httpClient.GetStreamAsync(requestUri);
        var obj = await JsonSerializer.DeserializeAsync<Schema>(responseStream);

        return obj ?? new Schema();
    }
}
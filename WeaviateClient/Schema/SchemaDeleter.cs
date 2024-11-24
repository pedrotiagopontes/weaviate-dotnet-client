namespace WeaviateClient.Schema;

public class SchemaDeleter(HttpClient httpClient, string baseUrl)
{
    private const string ResourcePath = "schema";

    public async Task DeleteAsync(string className)
    {
        var requestUri = $"{baseUrl}/{ResourcePath}/{className}";
        var response = await httpClient.DeleteAsync(requestUri);
        response.EnsureSuccessStatusCode();
    }
}
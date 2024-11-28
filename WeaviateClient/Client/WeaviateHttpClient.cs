namespace WeaviateClient.Client;

using System.Text;
using System.Text.Json;

public class WeaviateHttpClient(HttpClient client)
{
    public async Task<TResponseType> GetAllAsync<TResponseType>(string path) where TResponseType : new()
    {
        var requestUri = $"{client.BaseAddress}/{path}";
        var responseStream = await client.GetStreamAsync(requestUri);
        var obj = await JsonSerializer.DeserializeAsync<TResponseType>(responseStream);

        return obj ?? new TResponseType();
    }
    
    public async Task<TResponseType> GetAsync<TResponseType>(string path, Guid uuid) where TResponseType : new()
    {
        var requestUri = $"{client.BaseAddress}/{path}/{uuid}";
        var responseStream = await client.GetStreamAsync(requestUri);
        var obj = await JsonSerializer.DeserializeAsync<TResponseType>(responseStream);

        return obj ?? new TResponseType();
    }
    
    public async Task<TResponseType> PostAsync<TResquestType, TResponseType>(string path, TResquestType request) 
        where TResponseType : new()
    {
        var url = $"{client.BaseAddress}/{path}";
        var jsonContent = new StringContent(JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");
        var response = await client.PostAsync(url, jsonContent);

        response.EnsureSuccessStatusCode();
        var responseStream = await response.Content.ReadAsStreamAsync();
        var graphQlResponse = await JsonSerializer.DeserializeAsync<TResponseType>(responseStream);

        return graphQlResponse ?? new TResponseType();
    }
    
    public async Task DeleteAsync(string path, string className)
    {
        var requestUri = $"{client.BaseAddress}/{path}/{className}";
        var response = await client.DeleteAsync(requestUri);
        response.EnsureSuccessStatusCode();
    }
}
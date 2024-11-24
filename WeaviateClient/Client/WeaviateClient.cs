namespace weaviate_dotnet_client_poc.Client;

using System.Net.Http.Headers;

public class WeaviateClient(HttpClient client)
{
    private string baseUrl;
    
    public WeaviateClient WithBaseURl(string url)
    {
        this.baseUrl = url;
        return this;
    }
    public WeaviateClient WithApikey(string token)
    {
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        return this;
    }
    
    public WeaviateClient WithUserAgent(string userAgent)
    {
        client.DefaultRequestHeaders.Add("User-Agent", $"{userAgent}");
        return this;
    }
    
    public WeaviateClient AcceptHeaders(string[] headers)
    {
        client.DefaultRequestHeaders.Accept.Clear();

        foreach (var header in headers)
        {
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue($"{header}"));
        }
        return this;
    }

    public Data Data()
    {
        return new Data(client, baseUrl);
    }
}
namespace WeaviateClient.Client;

using System.Net.Http.Headers;
using API.Object;
using API.Schema;
using GraphQL;

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

    public ObjectData Data()
    {
        return new ObjectData(client, baseUrl);
    }
    
    public SchemaDeleter SchemaDeleter()
    {
        return new SchemaDeleter(client, baseUrl);
    }
    
    public GraphQL GraphQL()
    {
        return new GraphQL(client, baseUrl);
    }
}
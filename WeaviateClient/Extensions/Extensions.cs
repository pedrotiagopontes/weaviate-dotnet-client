namespace WeaviateClient.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Client;
using WeaviateClient.API.Object;
using API.Schema;
using GraphQL;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWeaviateClient(this IServiceCollection services, Action<WeaviateClientOptions> configureOptions)
    {
        services.Configure(configureOptions);

        services.AddHttpClient<WeaviateHttpClient>((provider, client) =>
        {
            var options = provider.GetRequiredService<IOptions<WeaviateClientOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {options.ApiKey}");
            client.DefaultRequestHeaders.Add("User-Agent", options.UserAgent);
        });

        services.AddTransient<IObjectDataClient, ObjectDataClient>();
        services.AddTransient<IGraphQLClient, GraphQLClient>();
        services.AddTransient<ISchemaClient, SchemaClient>();
        services.AddTransient<IGraphQLGetter, GraphQLGetter>();
        services.AddTransient<IWeaviateClient, WeaviateClient>();
        
        return services;
    }
}
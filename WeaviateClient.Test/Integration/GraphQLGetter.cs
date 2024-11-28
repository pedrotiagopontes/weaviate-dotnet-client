﻿namespace WeaviateClient.Test.Integration;

using Client;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[TestClass]
public sealed class GraphQLGetter
{
    private static IServiceProvider serviceProvider;
    private static IConfiguration Configuration { get; set; }

    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
        var serviceCollection = new ServiceCollection();
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        serviceCollection.AddSingleton<IConfiguration>(Configuration);

        serviceCollection.AddWeaviateClient(options =>
        {
            options.BaseUrl = Configuration["WCD_HOST_NAME"] ?? string.Empty;
            options.ApiKey = Configuration["WCD_API_KEY"] ?? string.Empty;
            options.UserAgent = "local-test";
            options.OpenAIKey = Configuration["OPENAI_API_KEY"] ?? string.Empty;
        });

        serviceProvider = serviceCollection.BuildServiceProvider();
    }
    
    [TestMethod]
    [TestCategory("Integration")]
    public async Task GraphQLGetter_BaseCase_ShouldSucceed()
    {   // Arrange
        var client = serviceProvider.GetRequiredService<IWeaviateClient>();
        
        // Act
        var result = await client.GraphQL().Get().
            WithClassName("Person").
            WithFields(["name", "age"]).
            RunAsync();
        
        // Assert
        Assert.IsNotNull(result);
        Assert.IsNull(result.Errors);
        Assert.IsNotNull(result.Data);
        //TODO: better assert
    }
    
    [TestMethod]
    [TestCategory("Integration")]
    public async Task GraphQLGetter_WithLimitAndOffset_ShouldSucceed()
    {   // Arrange
        var client = serviceProvider.GetRequiredService<IWeaviateClient>();
        
        // Act
        var result = await client.GraphQL().Get().
            WithClassName("Person").
            WithFields(["name", "age"]).
            WithLimit(1).
            WithOffset(1).
            RunAsync();
        
        // Assert
        Assert.IsNotNull(result);
        Assert.IsNull(result.Errors);
        Assert.IsNotNull(result.Data);
        //TODO: better assert
    }
}
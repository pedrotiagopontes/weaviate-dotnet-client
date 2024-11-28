# Weaviate .NET Client
A native .NET client for Weaviate.

## Usage
1. Add the dependency to your project;
2. Create a new instance of the client:
```csharp
var options = new WeaviateClientOptions
    {
        BaseUrl = "localhost:8080" // your weaviate instance
        ApiKey = "akey", // your api key
        UserAgent = "local-app",// your user agent
    };

// To create a new instance of the client with the default http client. 
// An overload is available to pass a custom http client.
var client = new WeaviateClient CreateDefaultClient(WeaviateClientOptions options)
```
#### Extensions: DI container Support
The client is designed to support being used together with DI container. You can register it in your application using:

```csharp
 services.AddWeaviateClient(options =>
    {
        options.BaseUrl = ...
    });
```

### Creating a new Object
```csharp
await client.Data().Creator().
            WithClassName("Person").
            WithProperties(["name"]).
            WithVector([1.0f,2.0f,3.0f]).
            CreateAsync();
```

### Searching Objects using GraphQL
This client supports BM25, NearVector and Hybrid search. 
Please refer to [Weaviate - How to search](https://weaviate.io/developers/weaviate/search) for details on each of this search types.

Bellow you can find an example of how to search using BM25. Other search types are similar but require a specific search builder for each type:
```csharp
var searchPersonWithBm25 = new BM25Builder().
    WithQuery("Person 10").
    FilterOn(["name"]);

var result = await searchPersonWithBm25Query = client.GraphQL().Get().
        WithClassName("Person").
        WithFields(["name", "age"]).
        WithAdditionalFields(["id", "vector"]).
        WithSearch(searchPersonWithBm25).
        WithLimit(1).
        RunAsync();
```

### More Examples
Please refer to the script on the `Program.cs` file for more examples on how to use the client.
It contains examples on how the following:
- Delete any existing classes 
- Add 250 objects with random data
- Arbitrary text properties to differentiate objects for demo searches 
- 2 search examples for each search type
- Get all objects using the cursor api and sum up the counter property
- Create a client with a default http client or using the DI container


## Design Considerations
 - The client lybrary exposes a Fluent API to make it easier to use.
 - The search queries are built using a builder pattern to make it easier to create complex queries.
 - It adheres to the SOLID principles and is designed to be easily testable. To do so each class is designed to have a 
single responsibility and dependencies are injected through the constructor. Most of the dependencies are interfaces, 
so they can be easily mocked and replaced.
 - The client is designed to be easy to use with DI container and each dependency can be easily replaced by a custom 
implementation if the user of SDK needs to do so.
 - For this PoC, error handling is done by throwing generic exceptions. Further work is required.
 - Regarding tests:
   - A minimal set of tests is written to demonstrate how to test the client and prove that it works. A higher coverage would be required on a real project. 
     - Unit tests cover mostly the query builders since they are where most of the logic client specific logic lives and the remaining classes are mostly wrappers to provide a better UX 
     around the HttpClient.
     - Integration tests are written for one successful scenario of each type of operation required to support on this client.

## Future Work
- Improve test coverage;
- Implement error handling;
- Implement logging, preferably using a structured logging approach;
- Make the client more resilient - e.g. retry policy, circuit breaker. Since the http client is injected, it is possible 
to use a custom http client with these features (e.g. Polly);
- Support the remaing weaviate features - authentication, other search types, etc;
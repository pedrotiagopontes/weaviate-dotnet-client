# Weaviate .NET Client
A native .NET client for Weaviate.

## Usage
1. Add the dependency to your project;
2. Create a new instance of the client:
```csharp
var options = new WeaviateClientOptions
    {
        BaseUrl = "localhost:8080", // your weaviate instance
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
        options.BaseUrl = ...;
    });
```

### Creating a new Object
```csharp
var objectProperties = new Dictionary<string, object>
        {
            {"name", $"John Doe"},
            {"age", 43}
        };

await client.Data().Creator().
            WithClassName("Person").
            WithProperties(objectProperties).
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
- Add 250 objects with
  - Random data and arbitrary text properties to differentiate objects for demo searches 
- 2 search examples for each search type: BM25, NearVector and Hybrid
- Get all objects using the cursor api and sum up the counter property
- Create a client with a default http client or using the DI container


## Design Considerations
 - The client library exposes a Fluent API to make it easier to use.
 - The search queries are built using a builder pattern to make it easier to create complex queries.
 - It adheres to the SOLID principles and is designed to be easily testable. To do so each class is designed to have a 
single responsibility and dependencies are injected through the constructor. Most of the dependencies are interfaces, 
so they can be easily mocked and replaced.
 - The client is designed to be easy to use with DI container and each dependency can be easily replaced by a custom 
implementation if the user of SDK needs to do so.
 - For this PoC, error handling is done by throwing generic exceptions. Further work is required. 

### Design considerations regarding tests:
A minimal set of tests is written to demonstrate how to test the client and prove that it works. A higher coverage would be required on a real project.
   - **Unit tests**: 
     - For the unit tests it was prioritized covering the query builders, since it is where most of this client specific logic lives.
All the other classes are mostly wrappers around the HttpClient designed to provide a better UX and where not covered in the context of this PoC. 
     - The following classes are covered with unit tests:
       - Bm25Builder
       - HybridBuilder
       - NearVectorBuilder
       - GraphQL Query with and without search parameters
   - **Integration tests**
     - A successful scenario of each type of operation required to support on this client was implemented. A few design decisions were made in the context of this PoC:
       - To have data for the tests, it was required to seed some data into a weaviate instance. To do so, we are using this client to create data, creating a coupling between the test and the client implementation. 
       This means that if a bug is present on the client the tests may not notice it, for a real app a seeding mechanism independent of this client is advisable.
       - To guarantee that the test execution is repeatable and independent of previous runs, we are deleting the schema after each test using this client too, this suffers from the same problem as the seeding mechanism described above. 
       - The integration tests lack coverage of the different parameters supported for each query type. These could be extended for better coverage, however only if the execution cost is low and without going until the point where we are testing the internal DB behavior on the client. 
       For the different parameters supported by each query type, the unit tests should already provide a good coverage.
     - The assertions done on the results of the integration tests are minimal and checking if the data "contains" the expected data. This is not ideal and should be improved to guarantee that the data is correct.
     - An area that is not covered by the tests is error handling, either internal and how the client handles errors coming from the weaviate instance. This is a critical part of the client and should be covered by tests but it was not in scope of this PoC.

## Future Work
- Improve test coverage;
- Implement error handling;
- Implement logging, preferably using a structured logging approach;
- Make the client more resilient - e.g. retry policy, circuit breaker. Since the http client is injected, it is possible 
to use a custom http client with these features (e.g. Polly);
- Support the remaining weaviate features - authentication, other search types, etc;
namespace WeaviateClient.Test.Unit;

using WeaviateClient.GraphQL;

[TestClass]
public class BM25QueryBuilderTests
{
    [TestMethod]
    public void BuildBM25Query_WithQueryAndProperties_ShouldReturnValidQuery()
    {
        // Arrange
        var builder = new BM25QueryBuilder()
            .WithQuery("fox")
            .FilterOn(new[] { "title", "content" });

        // Act
        var result = builder.Build();

        // Assert
        var expected = @"{ query: ""fox"", properties: [""title"", ""content""] }";
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void BuildBM25Query_WithQueryOnly_ShouldReturnValidQuery()
    {
        // Arrange
        var builder = new BM25QueryBuilder()
            .WithQuery("fox");

        // Act
        var result = builder.Build();

        // Assert
        var expected = @"{ query: ""fox"" }";
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void BuildBM25Query_WithoutQuery_ShouldThrowException()
    {
        // Arrange
        var builder = new BM25QueryBuilder();

        // Act
        builder.Build();

        // Assert - Expects exception
    }

    [TestMethod]
    public void BuildBM25Query_WithEmptyProperties_ShouldReturnValidQueryWithoutProperties()
    {
        // Arrange
        var builder = new BM25QueryBuilder()
            .WithQuery("fox")
            .FilterOn(Array.Empty<string>());

        // Act
        var result = builder.Build();

        // Assert
        var expected = @"{ query: ""fox"" }";
        Assert.AreEqual(expected, result);
    }
}
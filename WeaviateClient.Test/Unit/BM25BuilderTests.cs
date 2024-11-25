namespace WeaviateClient.Test.Unit;

using GraphQL.QueryBuilder;

[TestClass]
public class Bm25BuilderTests
{
    [TestMethod]
    [TestCategory("Unit")]
    public void BuildBM25Query_WithQueryAndProperties_ShouldReturnValidQuery()
    {
        // Arrange
        var builder = new BM25Builder()
            .WithQuery("fox")
            .FilterOn(["title", "content"]);

        // Act
        var result = builder.Build();

        // Assert
        var expected = @"{ query: ""fox"", properties: [""title"", ""content""] }";
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void BuildBM25Query_WithQueryOnly_ShouldReturnValidQuery()
    {
        // Arrange
        var builder = new BM25Builder()
            .WithQuery("fox");

        // Act
        var result = builder.Build();

        // Assert
        var expected = @"{ query: ""fox"" }";
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [ExpectedException(typeof(InvalidOperationException))]
    public void BuildBM25Query_WithoutQuery_ShouldThrowException()
    {
        // Arrange
        var builder = new BM25Builder();

        // Act
        builder.Build();

        // Assert - Expects exception
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void BuildBM25Query_WithEmptyProperties_ShouldReturnValidQueryWithoutProperties()
    {
        // Arrange
        var builder = new BM25Builder()
            .WithQuery("fox")
            .FilterOn(Array.Empty<string>());

        // Act
        var result = builder.Build();

        // Assert
        var expected = @"{ query: ""fox"" }";
        Assert.AreEqual(expected, result);
    }
}
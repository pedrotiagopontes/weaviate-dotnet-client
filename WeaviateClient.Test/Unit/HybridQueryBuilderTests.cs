namespace WeaviateClient.Test.Unit;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GraphQL;

[TestClass]
public class HybridQueryBuilderTests
{
    [TestMethod]
    public void BuildHybridQuery_WithAllParameters_ShouldReturnValidQuery()
    {
        // Arrange
        var builder = new HybridQueryBuilder()
            .WithQuery("fox")
            .WithAlpha(0.85f)
            .WithVector([0.1f, 0.2f, 0.3f])
            .WithProperties(["title", "content"])
            .WithFusionType("ranked");

        // Act
        var result = builder.Build();

        // Assert
        var expected = @"{ query: ""fox"", alpha: 0.85, vector: [0.1, 0.2, 0.3], properties: [""title"", ""content""], fusionType: ""ranked"" }";
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void BuildHybridQuery_WithMandatoryQueryOnly_ShouldReturnValidQuery()
    {
        // Arrange
        var builder = new HybridQueryBuilder()
            .WithQuery("fox");

        // Act
        var result = builder.Build();

        // Assert
        var expected = @"{ query: ""fox"" }";
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void BuildHybridQuery_WithAlphaAndProperties_ShouldReturnValidQuery()
    {
        // Arrange
        var builder = new HybridQueryBuilder()
            .WithQuery("fox")
            .WithAlpha(0.75f)
            .WithProperties(["title"]);

        // Act
        var result = builder.Build();

        // Assert
        var expected = @"{ query: ""fox"", alpha: 0.75, properties: [""title""] }";
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void BuildHybridQuery_WithVectorOnly_ShouldReturnValidQuery()
    {
        // Arrange
        var builder = new HybridQueryBuilder()
            .WithQuery("fox")
            .WithVector([0.4f, 0.5f]);

        // Act
        var result = builder.Build();

        // Assert
        var expected = @"{ query: ""fox"", vector: [0.4, 0.5] }";
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void BuildHybridQuery_WithoutQuery_ShouldThrowException()
    {
        // Arrange
        var builder = new HybridQueryBuilder();

        // Act
        builder.Build();

        // Assert - Expects exception
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void BuildHybridQuery_WithInvalidAlpha_ShouldThrowException()
    {
        // Arrange
        var builder = new HybridQueryBuilder()
            .WithQuery("fox")
            .WithAlpha(1.5f); // Invalid alpha

        // Act
        builder.Build();

        // Assert - Expects exception
    }
}

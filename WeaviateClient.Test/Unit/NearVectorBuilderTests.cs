namespace WeaviateClient.Test.Unit;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GraphQL;
using GraphQL.QueryBuilder;

[TestClass]
public class NearVectorBuilderTests
{
    [TestMethod]
    [TestCategory("Unit")]
    public void BuildNearVector_WithVectorAndCertainty_ShouldReturnValidQuery()
    {
        // Arrange
        var builder = new NearVectorBuilder()
            .WithVector([0.1f, 0.2f, 0.3f])
            .WithCertainty(0.9f);

        // Act
        var result = builder.Build();

        // Assert
        var expected = @"{ vector: [0.1, 0.2, 0.3], certainty: 0.9 }";
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void BuildNearVector_WithVectorAndDistance_ShouldReturnValidQuery()
    {
        // Arrange
        var builder = new NearVectorBuilder()
            .WithVector([0.4f, 0.5f, 0.6f])
            .WithDistance(1.5f);

        // Act
        var result = builder.Build();

        // Assert
        var expected = @"{ vector: [0.4, 0.5, 0.6], distance: 1.5 }";
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void BuildNearVector_WithOnlyVector_ShouldReturnValidQuery()
    {
        // Arrange
        var builder = new NearVectorBuilder()
            .WithVector([0.7f, 0.8f]);

        // Act
        var result = builder.Build();

        // Assert
        var expected = @"{ vector: [0.7, 0.8] }";
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [ExpectedException(typeof(InvalidOperationException))]
    public void BuildNearVector_WithVectorAndBothDistanceAndCertainty_ShouldThrowException()
    {
        // Arrange
        var builder = new NearVectorBuilder()
            .WithVector([0.1f, 0.2f, 0.3f])
            .WithDistance(1.0f)
            .WithCertainty(0.8f);

        // Act
        builder.Build();

        // Assert - Expects exception
    }

    [TestMethod]
    [TestCategory("Unit")]
    [ExpectedException(typeof(InvalidOperationException))]
    public void BuildNearVector_WithoutVector_ShouldThrowException()
    {
        // Arrange
        var builder = new NearVectorBuilder()
            .WithCertainty(0.9f);

        // Act
        builder.Build();

        // Assert - Expects exception
    }

    [TestMethod]
    [TestCategory("Unit")]
    [ExpectedException(typeof(ArgumentException))]
    public void BuildNearVector_WithNegativeDistance_ShouldThrowException()
    {
        // Arrange
        var builder = new NearVectorBuilder()
            .WithVector([0.1f, 0.2f])
            .WithDistance(-1.0f);

        // Act
        builder.Build();

        // Assert - Expects exception
    }

    [TestMethod]
    [TestCategory("Unit")]
    [ExpectedException(typeof(ArgumentException))]
    public void BuildNearVector_WithInvalidCertainty_ShouldThrowException()
    {
        // Arrange
        var builder = new NearVectorBuilder()
            .WithVector([0.1f, 0.2f])
            .WithCertainty(1.5f); // Invalid certainty

        // Act
        builder.Build();

        // Assert - Expects exception
    }
}

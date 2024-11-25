namespace WeaviateClient.Test.Unit;

using GraphQL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class GraphQLQueryBuilderTests
{
    [TestMethod]
    public void BuildQuery_WithBasicFields_ShouldReturnValidQuery()
    {
        var query = new GraphQlQueryBuilder()
            .Operation("Get")
            .WithClassName("Person")
            .WithProprieties(["name", "age"])
            .Build();

        var expected = """
                       {
                       Get {
                           Person {
                               name
                               age
                           }
                       }
                       }
                       """;
        AssertAreEqualQuery(expected, query);
    }

    [TestMethod]
    public void BuildQuery_WithParameters_ShouldReturnValidQuery()
    {
        var query = new GraphQlQueryBuilder()
            .Operation("Get")
            .WithClassName("JeopardyQuestion")
            .WithProprieties(["question", "answer", "points"])
            .WithLimit(1)
            .Build();

        var expected = @"{
Get {
    JeopardyQuestion (limit: 1) {
        question
        answer
        points
    }
}
}";
        AssertAreEqualQuery(expected, query);
    }

    [TestMethod]
    public void BuildQuery_WithParametersInAnyOrder_ShouldReturnValidQuery()
    {
        var query1 = new GraphQlQueryBuilder()
            .Operation("Get")
            .WithClassName("JeopardyQuestion")
            .WithLimit(1)
            .WithProprieties(["question", "answer", "points"])
            .Build();

        var query2 = new GraphQlQueryBuilder()
            .Operation("Get")
            .WithLimit(1)
            .WithClassName("JeopardyQuestion")
            .WithProprieties(["question", "answer", "points"])
            .Build();

        Assert.AreEqual(query1, query2);
    }

    [TestMethod]
    public void BuildQuery_WithOffsetAndLimit_ShouldReturnValidQuery()
    {
        var query = new GraphQlQueryBuilder()
            .Operation("Get")
            .WithClassName("JeopardyQuestion")
            .WithProprieties(["question", "answer"])
            .WithLimit(10)
            .WithOffset(5)
            .Build();

        var expected = @"{
Get {
    JeopardyQuestion (limit: 10, offset: 5) {
        question
        answer
    }
}
}";
        AssertAreEqualQuery(expected, query);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void BuildQuery_WithoutRootOrClassName_ShouldThrowException()
    {
        var query = new GraphQlQueryBuilder()
            .WithProprieties(["name", "age"])
            .Build();
    }
    
    private void AssertAreEqualQuery(string expected, string actual)
    {
        Assert.AreEqual(NormalizeString(expected), NormalizeString(actual));
    }
    private string NormalizeString(string input)
    {
        return input.Replace("\r", "");
    }
}

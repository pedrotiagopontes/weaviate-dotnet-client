namespace WeaviateClient.Test.Unit;

using GraphQL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class QueryBuilderTests
{
    [TestMethod]
    public void BuildQuery_WithBasicFields_ShouldReturnValidQuery()
    {
        var query = new QueryBuilder()
            .Operation("Get")
            .WithClassName("Person")
            .WithFields(["name", "age"])
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
    public void BuildQuery_WithLimit_ShouldReturnValidQuery()
    {
        var query = new QueryBuilder()
            .Operation("Get")
            .WithClassName("JeopardyQuestion")
            .WithFields(["question", "answer", "points"])
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
    public void BuildQuery_WithOffsetAndLimit_ShouldReturnValidQuery()
    {
        var query = new QueryBuilder()
            .Operation("Get")
            .WithClassName("JeopardyQuestion")
            .WithFields(["question", "answer"])
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
    public void BuildQuery_WithBM25_ShouldReturnValidQuery()
    {
        var query = new QueryBuilder()
            .Operation("Get")
            .WithClassName("JeopardyQuestion")
            .WithFields(["question", "answer"])
            .WithLimit(3)
            .WithBM25("food")
            .Build();

        var expected = @"{
Get {
    JeopardyQuestion (limit: 3, bm25: { query: ""food"" }) {
        question
        answer
    }
}
}";
        AssertAreEqualQuery(expected, query);
    }
    
    [TestMethod]
    public void BuildQuery_WithBM25WithProperty_ShouldReturnValidQuery()
    {
        var query = new QueryBuilder()
            .Operation("Get")
            .WithClassName("JeopardyQuestion")
            .WithFields(["question", "answer"])
            .WithLimit(3)
            .WithBM25("food", ["answer", "question"])
            .Build();

        var expected = @"{
Get {
    JeopardyQuestion (limit: 3, bm25: { query: ""food"", properties: [""answer"", ""question""] }) {
        question
        answer
    }
}
}";
        AssertAreEqualQuery(expected, query);
    }
    
    [TestMethod]
    public void BuildQuery_WithParametersInAnyOrder_ShouldReturnValidQuery()
    {
        var query1 = new QueryBuilder()
            .Operation("Get")
            .WithClassName("JeopardyQuestion")
            .WithLimit(1)
            .WithFields(["question", "answer", "points"])
            .Build();

        var query2 = new QueryBuilder()
            .Operation("Get")
            .WithLimit(1)
            .WithClassName("JeopardyQuestion")
            .WithFields(["question", "answer", "points"])
            .Build();

        Assert.AreEqual(query1, query2);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void BuildQuery_WithoutOperationOrClassName_ShouldThrowException()
    {
        var query = new QueryBuilder()
            .WithFields(["name", "age"])
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

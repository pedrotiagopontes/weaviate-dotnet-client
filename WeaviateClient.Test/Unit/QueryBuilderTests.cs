namespace WeaviateClient.Test.Unit;

using GraphQL.QueryBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class QueryBuilderTests
{
    [TestMethod]
    [TestCategory("Unit")]
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
    [TestCategory("Unit")]
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
    [TestCategory("Unit")]
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
    [TestCategory("Unit")]
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
    [TestCategory("Unit")]
    [ExpectedException(typeof(InvalidOperationException))]
    public void BuildQuery_WithoutOperationOrClassName_ShouldThrowException()
    {
        new QueryBuilder()
            .WithFields(["name", "age"])
            .Build();
    }
    
    [TestMethod]
    [TestCategory("Unit")]
    public void BuildQuery_WithBM25_ShouldReturnValidQuery()
    {
        var search = new BM25Builder().WithQuery("food").FilterOn(["answer", "question"]);
        var query = new QueryBuilder()
            .Operation("Get")
            .WithClassName("JeopardyQuestion")
            .WithFields(["question", "answer"])
            .WithLimit(3)
            .WithSearch(search)
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
    [TestCategory("Unit")]
    public void BuildQuery_WithHybrid_ShouldReturnValidQuery()
    {
        var search = new HybridBuilder().WithQuery("food").WithAlpha(0.5f);
        var query = new QueryBuilder()
            .Operation("Get")
            .WithClassName("JeopardyQuestion")
            .WithFields(["question", "answer"])
            .WithLimit(3)
            .WithSearch(search)
            .Build();

        var expected = @"{
Get {
    JeopardyQuestion (limit: 3, hybrid: { query: ""food"", alpha: 0.5 }) {
        question
        answer
    }
}
}";
        AssertAreEqualQuery(expected, query);
    }
    
    [TestMethod]
    [TestCategory("Unit")]
    public void BuildQuery_WithNearVector_ShouldReturnValidQuery()
    {
        var search = new NearVectorBuilder().WithVector([0.1f, 0.2f, 0.3f]);
        var query = new QueryBuilder()
            .Operation("Get")
            .WithClassName("JeopardyQuestion")
            .WithFields(["question", "answer"])
            .WithLimit(3)
            .WithSearch(search)
            .Build();

        var expected = @"{
Get {
    JeopardyQuestion (limit: 3, nearVector: { vector: [0.1, 0.2, 0.3] }) {
        question
        answer
    }
}
}";
        AssertAreEqualQuery(expected, query);
    }
    
    [TestMethod]
    [TestCategory("Unit")]
    public void BuildQuery_WithAfter_ShouldReturnValidQuery()
    {
        var query = new QueryBuilder()
            .Operation("Get")
            .WithClassName("JeopardyQuestion")
            .WithFields(["question", "answer"])
            .WithLimit(3)
            .WithAfter("002d5cb3-298b-380d-addb-2e026b76c8ed")
            .Build();

        var expected = @"{
Get {
    JeopardyQuestion (limit: 3, after: ""002d5cb3-298b-380d-addb-2e026b76c8ed"") {
        question
        answer
    }
}
}";
        AssertAreEqualQuery(expected, query);
    }
    
    [TestMethod]
    [TestCategory("Unit")]
    [ExpectedException(typeof(ArgumentException))]
    public void BuildQuery_WithMultipleSearch_ShouldThrowException()
    {
        var searchHybrid = new HybridBuilder().WithQuery("food").WithAlpha(0.5f);
        var searchNear = new NearVectorBuilder().WithVector([0.1f, 0.2f, 0.3f]);
        new QueryBuilder()
            .Operation("Get")
            .WithClassName("JeopardyQuestion")
            .WithFields(["question", "answer"])
            .WithLimit(3)
            .WithSearch(searchHybrid)
            .WithSearch(searchNear)
            .Build();
        
    }
    
    [TestMethod]
    [TestCategory("Unit")]
    [ExpectedException(typeof(InvalidOperationException))]
    public void BuildQuery_WithAfterAndSearch_ShouldThrowException()
    {
        var searchNear = new NearVectorBuilder().WithVector([0.1f, 0.2f, 0.3f]);
        new QueryBuilder()
            .Operation("Get")
            .WithClassName("JeopardyQuestion")
            .WithFields(["question", "answer"])
            .WithLimit(3)
            .WithAfter("id")
            .WithSearch(searchNear)
            .Build();
    }
    
    [TestMethod]
    [TestCategory("Unit")]
    [ExpectedException(typeof(InvalidOperationException))]
    public void BuildQuery_WithAfterNoLimit_ShouldThrowException()
    {
        new NearVectorBuilder().WithVector([0.1f, 0.2f, 0.3f]);
        new QueryBuilder()
            .Operation("Get")
            .WithClassName("JeopardyQuestion")
            .WithFields(["question", "answer"])
            .WithAfter("id")
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

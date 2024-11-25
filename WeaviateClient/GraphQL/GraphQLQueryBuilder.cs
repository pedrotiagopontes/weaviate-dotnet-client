namespace WeaviateClient.GraphQL;

using System.Text;

public class GraphQlQueryBuilder
{
    protected readonly StringBuilder QueryBuilder = new StringBuilder();
    
    protected GraphQlQueryBuilder Root(string root)
    {
        QueryBuilder.AppendLine($"{root} {{");
        return this;
    }
    
    protected GraphQlQueryBuilder CloseBlock()
    {
        QueryBuilder.AppendLine("    }");
        return this;
    }
    
    protected GraphQlQueryBuilder CloseAll()
    {
        QueryBuilder.AppendLine("}");
        return this;
    }
}
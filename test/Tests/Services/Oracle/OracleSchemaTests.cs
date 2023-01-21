using App.Services.Oracle;
using FluentAssertions;

namespace Tests.Services.Oracle;

public class OracleSchemaTests
{
    [Theory]
    [InlineData("SYS", "01/06/2020")]
    [InlineData("OPS", "01/08/2022")]
    public void Should_Oracle_Schemas_Be_Equals(string schemaName, string creationDate)
    {
        // arrange
        var schemaDate = DateTime.Parse(creationDate);
        var schema1 = new OracleSchema
        {
            SchemaName = schemaName,
            CreationDate = schemaDate
        };
        var schema2 = new OracleSchema
        {
            SchemaName = schemaName,
            CreationDate = schemaDate
        };

        // act
        var schemasEquals = schema1.Equals(schema2);
        var objectsEquals = schema1.Equals((object)schema2);

        // assert
        schemasEquals.Should().BeTrue();
        objectsEquals.Should().BeTrue();
    }
    
    [Fact]
    public void Should_Get_Distinct_Oracle_Schemas()
    {
        // arrange
        var list = new List<OracleSchema>
        {
            new()
            {
                SchemaName = "schema"
            },
            new()
            {
                SchemaName = "schema"
            },
            new(),
            new(),
            null
        };

        var hashset = new HashSet<OracleSchema>(list);

        // act
        var hashsetCount = hashset.Count;
        var listDistinctCount = list.Distinct().Count();

        // assert
        hashsetCount.Should().Be(3);
        listDistinctCount.Should().Be(3);
    }
}
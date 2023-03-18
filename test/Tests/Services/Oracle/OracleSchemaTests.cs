using App.Services.Oracle;
using FluentAssertions;

namespace Tests.Services.Oracle;

public class OracleSchemaTests
{
    [Theory]
    [InlineData("SYS", "Y", "01/06/2020")]
    [InlineData("OPS", "Y", "01/08/2022")]
    public void Should_Oracle_Schemas_Be_Equals(string schemaName, string isMaintainedByOracle, string creationDate)
    {
        // arrange
        var schemaDate = DateTime.Parse(creationDate);
        var obj1 = new OracleSchema
        {
            SchemaName = schemaName,
            IsMaintainedByOracle = isMaintainedByOracle,
            CreationDate = schemaDate
        };
        var obj2 = new OracleSchema
        {
            SchemaName = schemaName,
            IsMaintainedByOracle = isMaintainedByOracle,
            CreationDate = schemaDate
        };

        // act
        var equals1 = obj1.Equals(obj1);
        var equals2 = obj1.Equals(obj2);
        var equals3 = obj1.Equals((object)obj2);

        // assert
        equals1.Should().BeTrue();
        equals2.Should().BeTrue();
        equals3.Should().BeTrue();
    }
    
    [Fact]
    public void Should_Oracle_Schemas_Not_Be_Equals()
    {
        // arrange
        var date1 = DateTime.Now.AddDays(-1);
        var obj1 = new OracleSchema
        {
            SchemaName = "schema",
            IsMaintainedByOracle = "Y",
            CreationDate = date1
        };
        
        var date2 = DateTime.Now.AddDays(-2);
        var obj2 = new OracleSchema
        {
            SchemaName = "schema",
            IsMaintainedByOracle = "Y",
            CreationDate = date2
        };
        
        // act
        var equals1 = obj1.Equals(obj2);
        var equals2 = obj1.Equals((object)obj2);
        var equals3 = obj1.Equals(null);
        var equals4 = obj1.Equals((object)null);

        // assert
        equals1.Should().BeFalse();
        equals2.Should().BeFalse();
        equals3.Should().BeFalse();
        equals4.Should().BeFalse();
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
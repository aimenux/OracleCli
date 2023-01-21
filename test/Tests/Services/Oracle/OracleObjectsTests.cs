using App.Services.Oracle;
using FluentAssertions;

namespace Tests.Services.Oracle;

public class OracleObjectsTests
{
    [Theory]
    [InlineData("PROCEDURE", "GET_INFOS", "USR")]
    [InlineData("PROCEDURE", "GET_SYS_DATE", "SYS")]
    public void Should_Oracle_Objects_Be_Equals(string objectName, string objectType, string ownerName)
    {
        // arrange
        var date = DateTime.Now.Date;
        var obj1 = new OracleObject
        {
            ObjectName = objectName,
            ObjectType = objectType,
            OwnerName = ownerName,
            CreationDate = date,
            ModificationDate = date
        };
        var obj2 = new OracleObject
        {
            ObjectName = objectName,
            ObjectType = objectType,
            OwnerName = ownerName,
            CreationDate = date,
            ModificationDate = date
        };

        // act
        var equals = obj1.Equals(obj2);
        var objectsEquals = obj1.Equals((object)obj2);

        // assert
        equals.Should().BeTrue();
        objectsEquals.Should().BeTrue();
    }
    
    [Fact]
    public void Should_Get_Distinct_Oracle_Objects()
    {
        // arrange
        var list = new List<OracleObject>
        {
            new()
            {
                ObjectName = "name",
                ObjectType = "type",
                OwnerName = "owner",
            },
            new()
            {
                ObjectName = "name",
                ObjectType = "type",
                OwnerName = "owner",
            },
            new(),
            new(),
            null
        };

        var hashset = new HashSet<OracleObject>(list);

        // act
        var hashsetCount = hashset.Count;
        var listDistinctCount = list.Distinct().Count();

        // assert
        hashsetCount.Should().Be(3);
        listDistinctCount.Should().Be(3);
    }
}
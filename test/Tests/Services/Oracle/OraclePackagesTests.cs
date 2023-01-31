using App.Services.Oracle;
using FluentAssertions;

namespace Tests.Services.Oracle;

public class OraclePackagesTests
{
    [Theory]
    [InlineData("Owner1", "Package1")]
    [InlineData("Owner2", "Package2")]
    public void Should_Oracle_Packages_Be_Equals(string ownerName, string packageName)
    {
        // arrange
        var date = DateTime.Now.Date;
        var obj1 = new OraclePackage
        {
            OwnerName = ownerName,
            PackageName = packageName,
            CreationDate = date,
            ModificationDate = date
        };
        var obj2 = new OraclePackage
        {
            OwnerName = ownerName,
            PackageName = packageName,
            CreationDate = date,
            ModificationDate = date
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
    public void Should_Oracle_Packages_Not_Be_Equals()
    {
        // arrange
        var date1 = DateTime.Now.AddDays(-1);
        var obj1 = new OraclePackage
        {
            OwnerName = "owner",
            PackageName = "package",
            CreationDate = date1,
            ModificationDate = date1
        };
        
        var date2 = DateTime.Now.AddDays(-2);
        var obj2 = new OraclePackage
        {
            OwnerName = "owner",
            PackageName = "package",
            CreationDate = date2,
            ModificationDate = date2
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
    public void Should_Get_Distinct_Oracle_Packages()
    {
        // arrange
        var date = DateTime.Now.Date;
        var list = new List<OraclePackage>
        {
            new()
            {
                OwnerName = "owner",
                PackageName = "package",
                CreationDate = date,
                ModificationDate = date
            },
            new()
            {
                OwnerName = "owner",
                PackageName = "package",
                CreationDate = date,
                ModificationDate = date
            },
            new(),
            new(),
            null
        };

        var hashset = new HashSet<OraclePackage>(list);

        // act
        var hashsetCount = hashset.Count;
        var listDistinctCount = list.Distinct().Count();

        // assert
        hashsetCount.Should().Be(3);
        listDistinctCount.Should().Be(3);
    }
}
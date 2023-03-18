using App.Services.Oracle;
using FluentAssertions;

namespace Tests.Services.Oracle;

public class OracleInfosTests
{
    [Theory]
    [InlineData("instanceName1", "hostName1", "description1", "version1", "logMode1", "openMode1", "protectionMode1", "instanceStatus1", "databaseStatus1")]
    [InlineData("instanceName2", "hostName2", "description2", "version2", "logMode2", "openMode2", "protectionMode2", "instanceStatus2", "databaseStatus2")]
    public void Should_Oracle_Infos_Be_Equals(string instanceName, string hostName, string description, string version, string logMode, string openMode, string protectionMode, string instanceStatus, string databaseStatus)
    {
        // arrange
        var date = DateTime.Now.Date;
        var obj1 = new OracleInfo
        {
            InstanceName = instanceName,
            HostName = hostName,
            Description = description,
            Version = version,
            LogMode = logMode,
            OpenMode = openMode,
            ProtectionMode = protectionMode,
            InstanceStatus = instanceStatus,
            DatabaseStatus = databaseStatus,
            CreationDate = date,
            StartupDate = date
        };
        var obj2 = new OracleInfo
        {
            InstanceName = instanceName,
            HostName = hostName,
            Description = description,
            Version = version,
            LogMode = logMode,
            OpenMode = openMode,
            ProtectionMode = protectionMode,
            InstanceStatus = instanceStatus,
            DatabaseStatus = databaseStatus,
            CreationDate = date,
            StartupDate = date
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
    public void Should_Oracle_Infos_Not_Be_Equals()
    {
        // arrange
        var date1 = DateTime.Now.AddDays(-1);
        var obj1 = new OracleInfo
        {
            InstanceName = "instanceName",
            HostName = "hostName",
            Description = "description",
            Version = "version",
            LogMode = "logMode",
            OpenMode = "openMode",
            ProtectionMode = "protectionMode",
            InstanceStatus = "instanceStatus",
            DatabaseStatus = "databaseStatus",
            CreationDate = date1,
            StartupDate = date1
        };
        
        var date2 = DateTime.Now.AddDays(-2);
        var obj2 = new OracleInfo
        {
            InstanceName = "instanceName",
            HostName = "hostName",
            Description = "description",
            Version = "version",
            LogMode = "logMode",
            OpenMode = "openMode",
            ProtectionMode = "protectionMode",
            InstanceStatus = "instanceStatus",
            DatabaseStatus = "databaseStatus",
            CreationDate = date2,
            StartupDate = date2
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
    public void Should_Get_Distinct_Oracle_Infos()
    {
        // arrange
        var date = DateTime.Now.Date;
        var list = new List<OracleInfo>
        {
            new()
            {
                InstanceName = "instanceName",
                HostName = "hostName",
                Description = "description",
                Version = "version",
                LogMode = "logMode",
                OpenMode = "openMode",
                ProtectionMode = "protectionMode",
                InstanceStatus = "instanceStatus",
                DatabaseStatus = "databaseStatus",
                CreationDate = date,
                StartupDate = date
            },
            new()
            {
                InstanceName = "instanceName",
                HostName = "hostName",
                Description = "description",
                Version = "version",
                LogMode = "logMode",
                OpenMode = "openMode",
                ProtectionMode = "protectionMode",
                InstanceStatus = "instanceStatus",
                DatabaseStatus = "databaseStatus",
                CreationDate = date,
                StartupDate = date
            },
            new(),
            new(),
            null
        };

        var hashset = new HashSet<OracleInfo>(list);

        // act
        var hashsetCount = hashset.Count;
        var listDistinctCount = list.Distinct().Count();

        // assert
        hashsetCount.Should().Be(3);
        listDistinctCount.Should().Be(3);
    }
}
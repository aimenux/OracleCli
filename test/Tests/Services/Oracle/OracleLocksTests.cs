using App.Services.Oracle;
using FluentAssertions;

namespace Tests.Services.Oracle;

public class OracleLocksTests
{
    [Theory]
    [InlineData("schema1", "user1", "machine1", "program1", "blocking1", "blocked1", "text1", 1)]
    [InlineData("schema2", "user2", "machine2", "program2", "blocking2", "blocked2", "text2", 2)]
    public void Should_Oracle_Locks_Be_Equals(string schema, string user, string machine, string program, string blockingSession, string blockedSession, string text, int time)
    {
        // arrange
        var date = DateTime.Now.Date;
        var obj1 = new OracleLock
        {
            SchemaName = schema,
            UserName = user,
            MachineName = machine,
            ProgramName = program,
            BlockingSession = blockingSession,
            BlockedSession = blockedSession,
            BlockedSqlText = text,
            BlockingStartDate = date,
            BlockingTime = time
        };
        var obj2 = new OracleLock
        {
            SchemaName = schema,
            UserName = user,
            MachineName = machine,
            ProgramName = program,
            BlockingSession = blockingSession,
            BlockedSession = blockedSession,
            BlockedSqlText = text,
            BlockingStartDate = date,
            BlockingTime = time
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
    public void Should_Oracle_Locks_Not_Be_Equals()
    {
        // arrange
        var date1 = DateTime.Now.AddDays(-1);
        var obj1 = new OracleLock
        {
            SchemaName = "schema",
            UserName = "user",
            MachineName = "machine",
            ProgramName = "program",
            BlockingSession = "blockingSession",
            BlockedSession = "blockedSession",
            BlockedSqlText = "text",
            BlockingStartDate = date1,
            BlockingTime = 1
        };
        
        var date2 = DateTime.Now.AddDays(-2);
        var obj2 = new OracleLock
        {
            SchemaName = "schema",
            UserName = "user",
            MachineName = "machine",
            ProgramName = "program",
            BlockingSession = "blockingSession",
            BlockedSession = "blockedSession",
            BlockedSqlText = "text",
            BlockingStartDate = date2,
            BlockingTime = 1
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
    public void Should_Get_Distinct_Oracle_Locks()
    {
        // arrange
        var date = DateTime.Now.Date;
        var list = new List<OracleLock>
        {
            new()
            {
                SchemaName = "schema",
                UserName = "user",
                MachineName = "machine",
                ProgramName = "program",
                BlockingSession = "blockingSession",
                BlockedSession = "blockedSession",
                BlockedSqlText = "text",
                BlockingStartDate = date,
                BlockingTime = 1
            },
            new()
            {
                SchemaName = "schema",
                UserName = "user",
                MachineName = "machine",
                ProgramName = "program",
                BlockingSession = "blockingSession",
                BlockedSession = "blockedSession",
                BlockedSqlText = "text",
                BlockingStartDate = date,
                BlockingTime = 1
            },
            new(),
            new(),
            null
        };

        var hashset = new HashSet<OracleLock>(list);

        // act
        var hashsetCount = hashset.Count;
        var listDistinctCount = list.Distinct().Count();

        // assert
        hashsetCount.Should().Be(3);
        listDistinctCount.Should().Be(3);
    }
}
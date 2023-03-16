using App.Services.Oracle;
using FluentAssertions;

namespace Tests.Services.Oracle;

public class OracleSessionsTests
{
    [Theory]
    [InlineData("schema1", "user1", "machine1", "program1", "module1", "state1", "text1")]
    [InlineData("schema2", "user2", "machine2", "program2", "module2", "state2", "text2")]
    public void Should_Oracle_Sessions_Be_Equals(string schema, string user, string machine, string program, string module, string state, string text)
    {
        // arrange
        var date = DateTime.Now.Date;
        var obj1 = new OracleSession
        {
            SchemaName = schema,
            UserName = user,
            MachineName = machine,
            ProgramName = program,
            ModuleName = module,
            State = state,
            SqlText = text,
            LogonDate = date,
            StartDate = date
        };
        var obj2 = new OracleSession
        {
            SchemaName = schema,
            UserName = user,
            MachineName = machine,
            ProgramName = program,
            ModuleName = module,
            State = state,
            SqlText = text,
            LogonDate = date,
            StartDate = date
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
    public void Should_Oracle_Sessions_Not_Be_Equals()
    {
        // arrange
        var date1 = DateTime.Now.AddDays(-1);
        var obj1 = new OracleSession
        {
            SchemaName = "schema",
            UserName = "user",
            MachineName = "machine",
            ProgramName = "program",
            ModuleName = "module",
            State = "state",
            SqlText = "text",
            LogonDate = date1,
            StartDate = date1
        };
        
        var date2 = DateTime.Now.AddDays(-2);
        var obj2 = new OracleSession
        {
            SchemaName = "schema",
            UserName = "user",
            MachineName = "machine",
            ProgramName = "program",
            ModuleName = "module",
            State = "state",
            SqlText = "text",
            LogonDate = date2,
            StartDate = date2
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
    public void Should_Get_Distinct_Oracle_Sessions()
    {
        // arrange
        var date = DateTime.Now.Date;
        var list = new List<OracleSession>
        {
            new()
            {
                SchemaName = "schema",
                UserName = "user",
                MachineName = "machine",
                ProgramName = "program",
                ModuleName = "module",
                State = "state",
                SqlText = "text",
                LogonDate = date,
                StartDate = date
            },
            new()
            {
                SchemaName = "schema",
                UserName = "user",
                MachineName = "machine",
                ProgramName = "program",
                ModuleName = "module",
                State = "state",
                SqlText = "text",
                LogonDate = date,
                StartDate = date
            },
            new(),
            new(),
            null
        };

        var hashset = new HashSet<OracleSession>(list);

        // act
        var hashsetCount = hashset.Count;
        var listDistinctCount = list.Distinct().Count();

        // assert
        hashsetCount.Should().Be(3);
        listDistinctCount.Should().Be(3);
    }
}
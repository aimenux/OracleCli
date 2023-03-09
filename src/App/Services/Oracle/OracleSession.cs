namespace App.Services.Oracle;

public sealed class OracleSession : IEquatable<OracleSession>
{
    public string SchemaName { get; init; }
    public string UserName { get; init; }
    public string MachineName { get; init; }
    public string ProgramName { get; init; }
    public string State { get; init; }
    public DateTime LogonDate { get; init; }
    public DateTime StartDate { get; init; }
    public string SqlText { get; init; }

    public bool Equals(OracleSession other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return SchemaName == other.SchemaName &&
               UserName == other.UserName &&
               MachineName == other.MachineName &&
               ProgramName == other.ProgramName &&
               State == other.State &&
               LogonDate == other.LogonDate &&
               StartDate == other.StartDate &&
               SqlText == other.SqlText;
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is OracleSession other && Equals(other);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(SchemaName);
        hashCode.Add(UserName);
        hashCode.Add(MachineName);
        hashCode.Add(ProgramName);
        hashCode.Add(State);
        hashCode.Add(LogonDate);
        hashCode.Add(StartDate);
        hashCode.Add(SqlText);
        return hashCode.ToHashCode();
    }
}
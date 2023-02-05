namespace App.Services.Oracle;

public sealed class OracleLock : IEquatable<OracleLock>
{
    public string SchemaName { get; init; }
    public string UserName { get; init; }
    public string MachineName { get; init; }
    public string ProgramName { get; init; }
    public string BlockingSession { get; init; }
    public string BlockedSession { get; init; }
    public string BlockedSqlText { get; init; }
    public DateTime BlockingStartDate { get; init; }
    public int BlockingTime { get; init; }

    public bool Equals(OracleLock other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return SchemaName == other.SchemaName &&
               UserName == other.UserName &&
               MachineName == other.MachineName &&
               ProgramName == other.ProgramName &&
               BlockingSession == other.BlockingSession &&
               BlockedSession == other.BlockedSession &&
               BlockedSqlText == other.BlockedSqlText &&
               BlockingStartDate == other.BlockingStartDate &&
               BlockingTime == other.BlockingTime;
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is OracleLock other && Equals(other);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(SchemaName);
        hashCode.Add(UserName);
        hashCode.Add(MachineName);
        hashCode.Add(ProgramName);
        hashCode.Add(BlockingSession);
        hashCode.Add(BlockedSession);
        hashCode.Add(BlockedSqlText);
        hashCode.Add(BlockingStartDate);
        hashCode.Add(BlockingTime);
        return hashCode.ToHashCode();
    }
}
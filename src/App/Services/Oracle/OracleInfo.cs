namespace App.Services.Oracle;

public class OracleInfo : IEquatable<OracleInfo>
{
    public string InstanceName { get; init; }  
    public string HostName { get; init; }
    public string Description { get; init; }
    public string Version { get; init; }
    public string LogMode { get; init; }
    public string OpenMode { get; init; }
    public string ProtectionMode { get; init; }
    public string InstanceStatus { get; init; }
    public string DatabaseStatus { get; init; }
    public DateTime CreationDate { get; init; }
    public DateTime StartupDate { get; init; }

    public bool Equals(OracleInfo other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return InstanceName == other.InstanceName &&
               HostName == other.HostName &&
               Description == other.Description &&
               Version == other.Version && 
               LogMode == other.LogMode && 
               OpenMode == other.OpenMode &&
               ProtectionMode == other.ProtectionMode && 
               InstanceStatus == other.InstanceStatus && 
               DatabaseStatus == other.DatabaseStatus && 
               CreationDate.Equals(other.CreationDate) &&
               StartupDate.Equals(other.StartupDate);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((OracleInfo)obj);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(InstanceName);
        hashCode.Add(HostName);
        hashCode.Add(Description);
        hashCode.Add(Version);
        hashCode.Add(LogMode);
        hashCode.Add(OpenMode);
        hashCode.Add(ProtectionMode);
        hashCode.Add(InstanceStatus);
        hashCode.Add(DatabaseStatus);
        hashCode.Add(CreationDate);
        hashCode.Add(StartupDate);
        return hashCode.ToHashCode();
    }

    public static bool operator ==(OracleInfo left, OracleInfo right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(OracleInfo left, OracleInfo right)
    {
        return !Equals(left, right);
    }
}
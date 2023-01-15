namespace App.Services.Oracle;

public sealed class OracleSource : IEquatable<OracleSource>
{
    public long Line { get; set; }
    
    public string Text { get; set; }

    public bool Equals(OracleSource other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Line == other.Line && Text == other.Text;
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is OracleSource other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Line, Text);
    }
}
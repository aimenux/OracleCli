using App.Configuration;

namespace Tests;

public class SettingsBuilder
{
    private Database _database;
    private string _defaultSchemaToUse;
    private string _defaultDatabaseToUse;

    public SettingsBuilder()
    {
        _database = new Database
        {
            DatabaseName = "oracle-for-tests",
            ConnectionString = "oracle-connection-string"
        };
    }

    public SettingsBuilder WithDatabase(string databaseName, string connectionString)
    {
        _database = new Database
        {
            DatabaseName = databaseName,
            ConnectionString = connectionString
        };
        return this;
    }

    public SettingsBuilder WithDefaultSchemaToUse(string defaultSchemaToUse)
    {
        _defaultSchemaToUse = defaultSchemaToUse;
        return this;
    }
    
    public SettingsBuilder WithDefaultDatabaseToUse(string defaultDatabaseToUse)
    {
        _defaultDatabaseToUse = defaultDatabaseToUse;
        return this;
    }

    public Settings Build()
    {
        return new Settings
        {
            Databases = new List<Database>
            {
                _database
            },
            DefaultSchemaToUse = _defaultSchemaToUse,
            DefaultDatabaseToUse = _defaultDatabaseToUse
        };
    }
}
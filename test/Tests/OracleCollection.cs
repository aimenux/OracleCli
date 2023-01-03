namespace Tests;

[CollectionDefinition(Collections.OracleCollectionName)]
public class OracleCollection : ICollectionFixture<OracleFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
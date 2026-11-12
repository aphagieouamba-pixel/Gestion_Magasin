using Microsoft.Data.Sqlite;

namespace MagasinGestion.Data;

public sealed class SqliteConnectionFactory
{
    public string DatabasePath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MagasinGestion", "magasin.db");
    public SqliteConnection Create()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(DatabasePath)!);
        var connection = new SqliteConnection($"Data Source={DatabasePath};Foreign Keys=True");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA foreign_keys = ON;";
        command.ExecuteNonQuery();
        return connection;
    }
}

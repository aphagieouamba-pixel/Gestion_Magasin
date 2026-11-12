using MagasinGestion.Data;
using MagasinGestion.Forms;

namespace MagasinGestion;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        var db = new SqliteConnectionFactory();
        new MigrationRunner(db).Migrate();
        try { new Services.BackupService(db).Backup(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MagasinGestion", "Backups"), true); } catch { }
        Application.Run(new LoginForm(db));
    }
}

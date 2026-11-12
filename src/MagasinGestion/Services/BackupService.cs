using MagasinGestion.Data;

namespace MagasinGestion.Services;
public sealed class BackupService(SqliteConnectionFactory factory)
{
    public string Backup(string folder,bool automatic){Directory.CreateDirectory(folder);var path=Path.Combine(folder,$"magasin_{DateTime.Now:yyyyMMdd_HHmmss}.db");File.Copy(factory.DatabasePath,path,true);using var db=factory.Create();using var c=db.CreateCommand();c.CommandText="PRAGMA integrity_check";if(Convert.ToString(c.ExecuteScalar())!="ok")throw new InvalidDataException("Sauvegarde invalide.");if(automatic){var files=Directory.GetFiles(folder,"magasin_*.db").OrderByDescending(File.GetCreationTimeUtc).Skip(7);foreach(var f in files)File.Delete(f);}return path;}
    public void Restore(string backup){if(!File.Exists(backup))throw new FileNotFoundException("Sauvegarde introuvable.",backup);var temp=factory.DatabasePath+".restore";File.Copy(backup,temp,true);File.Copy(temp,factory.DatabasePath,true);File.Delete(temp);}
}

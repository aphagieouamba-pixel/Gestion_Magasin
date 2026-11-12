using MagasinGestion.Data;
using MagasinGestion.Models;
using MagasinGestion.Security;

namespace MagasinGestion.Services;
public sealed class UserAdministrationService(SqliteConnectionFactory factory)
{
    public long Create(Utilisateur actor,string login,string fullName,string password,RoleUtilisateur role){new PermissionService().RequireAdministrator(actor);if(string.IsNullOrWhiteSpace(password))throw new ArgumentException("Mot de passe obligatoire.");var h=new PasswordHasher().Hash(password);using var db=factory.Create();using var c=db.CreateCommand();c.CommandText="INSERT INTO Utilisateur(NomUtilisateur,NomComplet,MotDePasseHash,MotDePasseSel,Iterations,Role) VALUES($l,$n,$h,$s,$i,$r);SELECT last_insert_rowid()";c.Parameters.AddWithValue("$l",login);c.Parameters.AddWithValue("$n",fullName);c.Parameters.AddWithValue("$h",h.Hash);c.Parameters.AddWithValue("$s",h.Salt);c.Parameters.AddWithValue("$i",h.Iterations);c.Parameters.AddWithValue("$r",role.ToString());return Convert.ToInt64(c.ExecuteScalar());}
    public void SetActive(Utilisateur actor,long target,bool active){new PermissionService().RequireAdministrator(actor);using var db=factory.Create();using var c=db.CreateCommand();c.CommandText="SELECT COUNT(*) FROM Utilisateur WHERE Role='Administrateur' AND Actif=1";if(!active&&Convert.ToInt32(c.ExecuteScalar())<=1){c.CommandText="SELECT Role,Actif FROM Utilisateur WHERE Id=$i";c.Parameters.AddWithValue("$i",target);using var r=c.ExecuteReader();if(r.Read()&&r.GetString(0)=="Administrateur"&&r.GetBoolean(1))throw new InvalidOperationException("Le dernier administrateur actif ne peut pas être désactivé.");r.Close();}c.Parameters.Clear();c.CommandText="UPDATE Utilisateur SET Actif=$a WHERE Id=$i";c.Parameters.AddWithValue("$a",active);c.Parameters.AddWithValue("$i",target);c.ExecuteNonQuery();}
}

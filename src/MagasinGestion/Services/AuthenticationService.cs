using MagasinGestion.Data;
using MagasinGestion.Models;
using MagasinGestion.Security;

namespace MagasinGestion.Services;
public sealed class AuthenticationService(SqliteConnectionFactory factory)
{
    public bool HasUsers(){using var db=factory.Create();using var c=db.CreateCommand();c.CommandText="SELECT EXISTS(SELECT 1 FROM Utilisateur)";return Convert.ToBoolean(c.ExecuteScalar());}
    public long CreateFirstAdministrator(string login,string name,string password){if(HasUsers())throw new InvalidOperationException("Un administrateur existe déjà.");var h=new PasswordHasher().Hash(password);using var db=factory.Create();using var c=db.CreateCommand();c.CommandText="INSERT INTO Utilisateur(NomUtilisateur,NomComplet,MotDePasseHash,MotDePasseSel,Iterations,Role) VALUES($l,$n,$h,$s,$i,'Administrateur');SELECT last_insert_rowid()";c.Parameters.AddWithValue("$l",login);c.Parameters.AddWithValue("$n",name);c.Parameters.AddWithValue("$h",h.Hash);c.Parameters.AddWithValue("$s",h.Salt);c.Parameters.AddWithValue("$i",h.Iterations);return Convert.ToInt64(c.ExecuteScalar());}
    public Utilisateur? Authenticate(string login,string password){using var db=factory.Create();using var c=db.CreateCommand();c.CommandText="SELECT Id,NomUtilisateur,NomComplet,MotDePasseHash,MotDePasseSel,Iterations,Role,Actif FROM Utilisateur WHERE NomUtilisateur=$l";c.Parameters.AddWithValue("$l",login);using var r=c.ExecuteReader();if(!r.Read()||!r.GetBoolean(7))return null;var hash=(byte[])r[3];var salt=(byte[])r[4];if(!new PasswordHasher().Verify(password,hash,salt,r.GetInt32(5)))return null;return new Utilisateur(r.GetInt64(0),r.GetString(1),r.GetString(2),Enum.Parse<RoleUtilisateur>(r.GetString(6)),true);}
}

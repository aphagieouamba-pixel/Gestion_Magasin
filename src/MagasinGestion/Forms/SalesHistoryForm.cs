using MagasinGestion.Data;

namespace MagasinGestion.Forms;
public sealed class SalesHistoryForm : Form
{
    public SalesHistoryForm(SqliteConnectionFactory db,long userId,bool admin){Text="Historique des ventes";Font=new Font("Segoe UI",10);var grid=new DataGridView{Dock=DockStyle.Fill,ReadOnly=true,AutoGenerateColumns=true};Controls.Add(grid);using var connection=db.Create();using var c=connection.CreateCommand();c.CommandText=$"SELECT v.Numero,v.DateVente,u.NomComplet Vendeur,v.ModePaiement,v.Total,CASE WHEN v.Annulee=1 THEN 'Annulée' ELSE 'Validée' END Statut FROM Vente v JOIN Utilisateur u ON u.Id=v.UtilisateurId WHERE {(admin?"1=1":"v.UtilisateurId=$u")} ORDER BY v.DateVente DESC";if(!admin)c.Parameters.AddWithValue("$u",userId);using var r=c.ExecuteReader();var rows=new List<object>();while(r.Read())rows.Add(new{Numero=r.GetString(0),Date=r.GetString(1),Vendeur=r.GetString(2),Paiement=r.GetString(3),Total=r.GetDecimal(4),Statut=r.GetString(5)});grid.DataSource=rows;}
}

using MagasinGestion.Data;
using Microsoft.Data.Sqlite;

namespace MagasinGestion.Services;

public sealed class StockService(SqliteConnectionFactory factory)
{
    public decimal GetStock(long produitId, SqliteConnection? existing = null)
    { if (existing is not null) return ReadStock(existing, produitId); using var db = factory.Create(); return ReadStock(db, produitId); }
    private static decimal ReadStock(SqliteConnection db, long produitId) { using var c = db.CreateCommand(); c.CommandText = "SELECT COALESCE(SUM(Quantite),0) FROM MouvementStock WHERE ProduitId=$id"; c.Parameters.AddWithValue("$id", produitId); return Convert.ToDecimal(c.ExecuteScalar()); }
    public string GetStatus(decimal stock, decimal seuil) => stock == 0 ? "En rupture" : stock <= seuil ? "Stock faible" : "Disponible";
    public long AddMovement(long produitId, long? auteurId, string type, decimal quantity, string motif, string origine = "Manuelle", decimal? counted = null)
    {
        if (quantity == 0) throw new ArgumentException("La quantité doit être différente de zéro.");
        using var db = factory.Create(); using var tx = db.BeginTransaction(); var before = GetStock(produitId, db); var after = before + quantity;
        if (after < 0) throw new InvalidOperationException("Stock insuffisant : le stock négatif est interdit.");
        using var c = db.CreateCommand(); c.Transaction = tx; c.CommandText = "INSERT INTO MouvementStock(ProduitId,UtilisateurId,Type,Quantite,StockAvant,StockApres,Motif,DateMouvement,Origine,QuantiteComptee,Ecart) VALUES($p,$u,$t,$q,$a,$n,$m,$d,$o,$c,$e); SELECT last_insert_rowid();";
        c.Parameters.AddWithValue("$p", produitId); c.Parameters.AddWithValue("$u", (object?)auteurId ?? DBNull.Value); c.Parameters.AddWithValue("$t", type); c.Parameters.AddWithValue("$q", quantity); c.Parameters.AddWithValue("$a", before); c.Parameters.AddWithValue("$n", after); c.Parameters.AddWithValue("$m", motif); c.Parameters.AddWithValue("$d", DateTime.UtcNow.ToString("O")); c.Parameters.AddWithValue("$o", origine); c.Parameters.AddWithValue("$c", (object?)counted ?? DBNull.Value); c.Parameters.AddWithValue("$e", counted.HasValue ? quantity : DBNull.Value);
        var id = Convert.ToInt64(c.ExecuteScalar()); Audit(db, tx, auteurId, type, produitId, $"Avant={before};Après={after};Écart={quantity};Motif={motif}"); tx.Commit(); return id;
    }
    public long Receive(long productId, long? userId, decimal quantity, string reason) => AddMovement(productId, userId, "Réapprovisionnement", Math.Abs(quantity), reason, "Réapprovisionnement");
    public long CorrectInventory(long productId, long? userId, decimal counted, string reason)
    { using var db = factory.Create(); var current = GetStock(productId, db); if (counted < 0) throw new ArgumentOutOfRangeException(nameof(counted)); return AddMovement(productId, userId, "Correction inventaire", counted - current, reason, "Inventaire", counted); }
    public IReadOnlyList<(long Id, string Reference, string Name, decimal Stock, decimal Threshold, string Status)> GetAvailability(string? search = null, bool onlyOut = false)
    { using var db = factory.Create(); using var c = db.CreateCommand(); c.CommandText = "SELECT p.Id,p.Reference,p.Nom,p.StockMinimum,COALESCE(SUM(m.Quantite),0) FROM Produit p LEFT JOIN MouvementStock m ON m.ProduitId=p.Id WHERE ($s IS NULL OR p.Reference LIKE $q OR p.Nom LIKE $q) GROUP BY p.Id,p.Reference,p.Nom,p.StockMinimum"; c.Parameters.AddWithValue("$s", (object?)search ?? DBNull.Value); c.Parameters.AddWithValue("$q", $"%{search}%"); var result = new List<(long,string,string,decimal,decimal,string)>(); using var r = c.ExecuteReader(); while(r.Read()){var stock=r.GetDecimal(4); var threshold=r.GetDecimal(3); var status=GetStatus(stock,threshold); if(!onlyOut || stock==0) result.Add((r.GetInt64(0),r.GetString(1),r.GetString(2),stock,threshold,status));} return result; }
    private static void Audit(SqliteConnection db, SqliteTransaction tx, long? user, string action, long entityId, string details) { using var c=db.CreateCommand(); c.Transaction=tx; c.CommandText="INSERT INTO JournalAudit(UtilisateurId,Action,Entite,EntiteId,Details,DateAction) VALUES($u,$a,'MouvementStock',$i,$d,$t)"; c.Parameters.AddWithValue("$u",(object?)user??DBNull.Value); c.Parameters.AddWithValue("$a",action); c.Parameters.AddWithValue("$i",entityId); c.Parameters.AddWithValue("$d",details); c.Parameters.AddWithValue("$t",DateTime.UtcNow.ToString("O")); c.ExecuteNonQuery(); }
}

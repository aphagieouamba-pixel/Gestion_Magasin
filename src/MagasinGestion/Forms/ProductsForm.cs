using MagasinGestion.Data;
using MagasinGestion.Services;

namespace MagasinGestion.Forms;

public sealed class ProductsForm : Form
{
    private readonly ProductService service;
    private readonly DataGridView grid = new() { Dock = DockStyle.Fill, AutoGenerateColumns = true, ReadOnly = true, AllowUserToAddRows = false };
    public ProductsForm(SqliteConnectionFactory db) { service = new(db); Text = "Produits"; Font = new Font("Segoe UI", 10); var search = new TextBox { PlaceholderText = "Rechercher par référence ou nom", Dock = DockStyle.Top, Height = 34 }; search.TextChanged += (_, _) => LoadProducts(search.Text); Controls.Add(grid); Controls.Add(search); Load += (_, _) => LoadProducts(null); }
    private void LoadProducts(string? search) { using var db = new SqliteConnectionFactory().Create(); using var c = db.CreateCommand(); c.CommandText = "SELECT p.Id,p.Reference,p.Nom,COALESCE(c.Nom,'-') Categorie,p.PrixVente,p.StockMinimum,p.Actif,p.DateCreation,COALESCE(SUM(m.Quantite),0) Stock FROM Produit p LEFT JOIN Categorie c ON c.Id=p.CategorieId LEFT JOIN MouvementStock m ON m.ProduitId=p.Id WHERE ($s IS NULL OR p.Reference LIKE $q OR p.Nom LIKE $q) GROUP BY p.Id,p.Reference,p.Nom,c.Nom,p.PrixVente,p.StockMinimum,p.Actif,p.DateCreation"; c.Parameters.AddWithValue("$s", (object?)search ?? DBNull.Value); c.Parameters.AddWithValue("$q", $"%{search}%"); using var r=c.ExecuteReader(); var rows=new List<object>(); while(r.Read()){var stock=r.GetDecimal(8); var threshold=r.GetDecimal(5); rows.Add(new { Id=r.GetInt64(0), Reference=r.GetString(1), Nom=r.GetString(2), Categorie=r.GetString(3), PrixVente=r.GetDecimal(4), Stock=stock, Statut=stock==0?"En rupture":stock<=threshold?"Stock faible":"Disponible", Actif=r.GetBoolean(6), DateCreation=r.GetString(7)});} grid.DataSource=rows; }
}

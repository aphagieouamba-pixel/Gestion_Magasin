namespace MagasinGestion.Forms;
using MagasinGestion.Data;
using MagasinGestion.Models;

public sealed class MainForm : Form
{
    private readonly Utilisateur session;
    public MainForm(SqliteConnectionFactory db, Utilisateur session)
    {
        this.session = session;
        Text = "Magasin Gestion"; WindowState = FormWindowState.Maximized; MinimumSize = new Size(1024, 640); Font = new Font("Segoe UI", 10); BackColor = Color.FromArgb(245, 247, 250);
        var nav = new Panel { Dock = DockStyle.Left, Width = 220, BackColor = Color.FromArgb(27, 55, 88) };
        var brand = new Label { Text = "MAGASIN\nGESTION", ForeColor = Color.White, Font = new Font("Segoe UI", 16, FontStyle.Bold), AutoSize = true, Location = new Point(24, 24) }; nav.Controls.Add(brand);
        string[] items = ["Tableau de bord", "Produits", "Stocks", "Nouvelle vente", "Historique", "Rapports", "Administration"];
        for (var i = 0; i < items.Length; i++) { var b = new Button { Text = items[i], Tag = items[i], FlatStyle = FlatStyle.Flat, ForeColor = Color.White, BackColor = i == 0 ? Color.FromArgb(47, 87, 130) : Color.Transparent, TextAlign = ContentAlignment.MiddleLeft, Location = new Point(12, 105 + i * 48), Size = new Size(195, 42) }; b.FlatAppearance.BorderSize = 0; b.Click += (_, _) => OpenModule((string)b.Tag!); nav.Controls.Add(b); }
        var content = new Panel { Dock = DockStyle.Fill, Padding = new Padding(35) }; content.Controls.Add(new Label { Text = "Tableau de bord", Font = new Font("Segoe UI", 24, FontStyle.Bold), AutoSize = true, Location = new Point(35, 35) });
        Controls.Add(content); Controls.Add(nav);
    }
    private void OpenModule(string module)
        { using var db = new SqliteConnectionFactory(); Form? form = module switch { "Produits" => new ProductsForm(db), "Stocks" => new StocksForm(db), "Nouvelle vente" => new NewSaleForm(db, session.Id), "Historique" => new SalesHistoryForm(db, session.Id, session.Role == RoleUtilisateur.Administrateur), "Rapports" => new ReportsForm(db), "Administration" when session.Role == RoleUtilisateur.Administrateur => new AdministrationForm(db), _ => null }; if (form is not null) using (form) form.ShowDialog(this); }
}

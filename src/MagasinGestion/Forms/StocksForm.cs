using MagasinGestion.Data;
using MagasinGestion.Services;

namespace MagasinGestion.Forms;

public sealed class StocksForm : Form
{
    private readonly StockService service;
    private readonly DataGridView grid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };
    public StocksForm(SqliteConnectionFactory db) { service = new(db); Text = "Stocks"; Font = new Font("Segoe UI", 10); var onlyOut = new CheckBox { Text = "Afficher uniquement les ruptures", Dock = DockStyle.Top, Height = 34 }; onlyOut.CheckedChanged += (_, _) => LoadStock(onlyOut.Checked); Controls.Add(grid); Controls.Add(onlyOut); Load += (_, _) => LoadStock(false); }
    private void LoadStock(bool onlyOut) { grid.DataSource = service.GetAvailability(onlyOut: onlyOut).Select(x => new { x.Reference, Produit=x.Name, x.Stock, x.Threshold, x.Status }).ToList(); }
}

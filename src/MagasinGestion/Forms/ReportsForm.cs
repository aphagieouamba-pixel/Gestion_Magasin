using MagasinGestion.Data;
using MagasinGestion.Services;

namespace MagasinGestion.Forms;
public sealed class ReportsForm : Form
{
    public ReportsForm(SqliteConnectionFactory db){Text="Rapports";Font=new Font("Segoe UI",10);var grid=new DataGridView{Dock=DockStyle.Fill,ReadOnly=true,AutoGenerateColumns=true};var date=new DateTimePicker{Dock=DockStyle.Top,Value=DateTime.Today};date.ValueChanged+=(_,_)=>LoadReport(grid,db,date.Value);Controls.Add(grid);Controls.Add(date);Load+=(_,_)=>LoadReport(grid,db,date.Value);}
    private static void LoadReport(DataGridView grid,SqliteConnectionFactory db,DateTime day){grid.DataSource=new ReportingService(db).Daily(day,day);}
}

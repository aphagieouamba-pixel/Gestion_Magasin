using MagasinGestion.Data;
using MagasinGestion.Services;

namespace MagasinGestion.Forms;
public sealed class AdministrationForm : Form
{
    public AdministrationForm(SqliteConnectionFactory db){Text="Administration";Font=new Font("Segoe UI",10);var backup=new Button{Text="Sauvegarder maintenant",Dock=DockStyle.Top,Height=40};var export=new Button{Text="Exporter les ventes CSV",Dock=DockStyle.Top,Height=40};backup.Click+=(_,_)=>{try{new BackupService(db).Backup(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"MagasinGestion","Sauvegardes"),false);MessageBox.Show("Sauvegarde terminée.");}catch(Exception ex){MessageBox.Show(ex.Message,"Échec");}};export.Click+=(_,_)=>{using var dialog=new SaveFileDialog{Filter="CSV UTF-8|*.csv",FileName="ventes.csv"};if(dialog.ShowDialog()==DialogResult.OK)new CsvExportService(db).ExportSales(dialog.FileName,1);};Controls.Add(export);Controls.Add(backup);}
}

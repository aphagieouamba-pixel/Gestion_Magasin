using System.Globalization;
using System.Text;
using MagasinGestion.Data;

namespace MagasinGestion.Services;
public sealed class CsvExportService(SqliteConnectionFactory factory)
{
    public string ExportSales(string path,long adminId){using var db=factory.Create();using var c=db.CreateCommand();c.CommandText="SELECT Numero,DateVente,ModePaiement,Total,Annulee FROM Vente ORDER BY DateVente DESC";using var r=c.ExecuteReader();using var w=new StreamWriter(path,false,new UTF8Encoding(true));w.WriteLine("Numero;Date;Paiement;Montant;Statut");while(r.Read())w.WriteLine(string.Join(';',Esc(r.GetString(0)),Esc(r.GetString(1)),Esc(r.GetString(2)),r.GetDecimal(3).ToString(CultureInfo.InvariantCulture),r.GetBoolean(4)?"Annulée":"Validée"));return path;}
    private static string Esc(string value){if(value.Length>0&&"=+-@".Contains(value[0]))value="'"+value;return '"'+value.Replace("\"","\"\"")+'"';}
}

using MagasinGestion.Data;
using MagasinGestion.Services;

namespace MagasinGestion.Forms;

public sealed class LoginForm : Form
{
    public LoginForm(SqliteConnectionFactory db)
    {
        Text = "Magasin Gestion - Connexion"; StartPosition = FormStartPosition.CenterScreen; ClientSize = new Size(430, 270); Font = new Font("Segoe UI", 10); BackColor = Color.White;
        var title = new Label { Text = "Gestion Magasin", Font = new Font("Segoe UI", 20, FontStyle.Bold), AutoSize = true, Location = new Point(35, 28) };
        var info = new Label { Text = "Connexion à votre espace de travail", AutoSize = true, Location = new Point(38, 72), ForeColor = Color.DimGray };
        var user = new TextBox { PlaceholderText = "Nom d'utilisateur", Location = new Point(38, 112), Width = 350 };
        var password = new TextBox { PlaceholderText = "Mot de passe", UseSystemPasswordChar = true, Location = new Point(38, 150), Width = 350 };
        var button = new Button { Text = "Se connecter", Location = new Point(38, 200), Width = 160, Height = 36, BackColor = Color.FromArgb(30, 90, 160), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        button.Click += (_, _) => { try { var auth = new AuthenticationService(db); if (!auth.HasUsers()) { using var setup = new FirstAdminForm(auth); setup.ShowDialog(this); } var session = auth.Authenticate(user.Text, password.Text); if (session is null) { MessageBox.Show("Identifiants invalides.", "Connexion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; } Hide(); using var main = new MainForm(db, session); main.ShowDialog(this); Show(); } catch (Exception ex) { MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error); } };
        Controls.AddRange([title, info, user, password, button]);
    }
}

internal sealed class FirstAdminForm : Form
{
    public FirstAdminForm(AuthenticationService auth){Text="Création du premier administrateur";ClientSize=new Size(420,220);var login=new TextBox{PlaceholderText="Nom d'utilisateur",Location=new Point(25,25),Width=350};var name=new TextBox{PlaceholderText="Nom complet",Location=new Point(25,65),Width=350};var pass=new TextBox{PlaceholderText="Mot de passe",UseSystemPasswordChar=true,Location=new Point(25,105),Width=350};var ok=new Button{Text="Créer",Location=new Point(25,155),Width=120};ok.Click+=(_,_)=>{try{auth.CreateFirstAdministrator(login.Text,name.Text,pass.Text);DialogResult=DialogResult.OK;Close();}catch(Exception ex){MessageBox.Show(ex.Message);}};Controls.AddRange([login,name,pass,ok]);}
}

using Microsoft.Data.Sqlite;

namespace MagasinGestion.Data;

public sealed class MigrationRunner(SqliteConnectionFactory factory)
{
    public void Migrate()
    {
        using var db = factory.Create();
        using var tx = db.BeginTransaction();
        Execute(db, tx, "CREATE TABLE IF NOT EXISTS VersionSchema (Version INTEGER PRIMARY KEY, AppliqueeLe TEXT NOT NULL)");
        var version = Convert.ToInt32(Scalar(db, tx, "SELECT COALESCE(MAX(Version),0) FROM VersionSchema"));
        if (version < 1) { ApplyV1(db, tx); Execute(db, tx, "INSERT INTO VersionSchema VALUES (1, datetime('now'))"); version = 1; }
        if (version < 2) { ApplyV2(db, tx); Execute(db, tx, "INSERT INTO VersionSchema VALUES (2, datetime('now'))"); }
        if (version < 3) { ApplyV3(db, tx); Execute(db, tx, "INSERT INTO VersionSchema VALUES (3, datetime('now'))"); }
        if (version < 4) { ApplyV4(db, tx); Execute(db, tx, "INSERT INTO VersionSchema VALUES (4, datetime('now'))"); }
        tx.Commit();
    }
    private static void ApplyV1(SqliteConnection db, SqliteTransaction tx)
    {
        Execute(db, tx, @"
CREATE TABLE Utilisateur (Id INTEGER PRIMARY KEY AUTOINCREMENT, NomUtilisateur TEXT NOT NULL UNIQUE, NomComplet TEXT NOT NULL, MotDePasseHash BLOB NOT NULL, MotDePasseSel BLOB NOT NULL, Iterations INTEGER NOT NULL, Role TEXT NOT NULL CHECK(Role IN ('Administrateur','Vendeur')), Actif INTEGER NOT NULL DEFAULT 1, CreeLe TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP);
CREATE TABLE Categorie (Id INTEGER PRIMARY KEY AUTOINCREMENT, Nom TEXT NOT NULL UNIQUE, Description TEXT);
CREATE TABLE Produit (Id INTEGER PRIMARY KEY AUTOINCREMENT, CategorieId INTEGER NULL, Reference TEXT NOT NULL UNIQUE, Nom TEXT NOT NULL, Description TEXT, PrixAchat NUMERIC NOT NULL DEFAULT 0 CHECK(PrixAchat >= 0), PrixVente NUMERIC NOT NULL CHECK(PrixVente >= 0), StockMinimum NUMERIC NOT NULL DEFAULT 0 CHECK(StockMinimum >= 0), Actif INTEGER NOT NULL DEFAULT 1, FOREIGN KEY(CategorieId) REFERENCES Categorie(Id));
CREATE TABLE Vente (Id INTEGER PRIMARY KEY AUTOINCREMENT, Numero TEXT NOT NULL UNIQUE, UtilisateurId INTEGER NOT NULL, DateVente TEXT NOT NULL, Total NUMERIC NOT NULL CHECK(Total >= 0), Annulee INTEGER NOT NULL DEFAULT 0, FOREIGN KEY(UtilisateurId) REFERENCES Utilisateur(Id));
CREATE TABLE LigneVente (Id INTEGER PRIMARY KEY AUTOINCREMENT, VenteId INTEGER NOT NULL, ProduitId INTEGER NOT NULL, Quantite NUMERIC NOT NULL CHECK(Quantite > 0), PrixUnitaire NUMERIC NOT NULL CHECK(PrixUnitaire >= 0), Total NUMERIC NOT NULL CHECK(Total >= 0), FOREIGN KEY(VenteId) REFERENCES Vente(Id), FOREIGN KEY(ProduitId) REFERENCES Produit(Id));
CREATE TABLE AnnulationVente (Id INTEGER PRIMARY KEY AUTOINCREMENT, VenteId INTEGER NOT NULL UNIQUE, UtilisateurId INTEGER NOT NULL, Motif TEXT NOT NULL, DateAnnulation TEXT NOT NULL, FOREIGN KEY(VenteId) REFERENCES Vente(Id), FOREIGN KEY(UtilisateurId) REFERENCES Utilisateur(Id));
CREATE TABLE MouvementStock (Id INTEGER PRIMARY KEY AUTOINCREMENT, ProduitId INTEGER NOT NULL, UtilisateurId INTEGER, Type TEXT NOT NULL, Quantite NUMERIC NOT NULL, StockAvant NUMERIC NOT NULL, StockApres NUMERIC NOT NULL, Motif TEXT, DateMouvement TEXT NOT NULL, FOREIGN KEY(ProduitId) REFERENCES Produit(Id), FOREIGN KEY(UtilisateurId) REFERENCES Utilisateur(Id));
CREATE TABLE JournalAudit (Id INTEGER PRIMARY KEY AUTOINCREMENT, UtilisateurId INTEGER, Action TEXT NOT NULL, Entite TEXT, EntiteId INTEGER, Details TEXT, DateAction TEXT NOT NULL, FOREIGN KEY(UtilisateurId) REFERENCES Utilisateur(Id));
CREATE TABLE Parametre (Cle TEXT PRIMARY KEY, Valeur TEXT NOT NULL);
CREATE INDEX IX_Produit_CategorieId ON Produit(CategorieId); CREATE INDEX IX_Vente_DateVente ON Vente(DateVente); CREATE INDEX IX_LigneVente_VenteId ON LigneVente(VenteId); CREATE INDEX IX_MouvementStock_ProduitId ON MouvementStock(ProduitId); CREATE INDEX IX_JournalAudit_DateAction ON JournalAudit(DateAction);");
    }
    private static void ApplyV2(SqliteConnection db, SqliteTransaction tx)
    {
        Execute(db, tx, @"
ALTER TABLE Produit ADD COLUMN DateCreation TEXT NOT NULL DEFAULT '1970-01-01T00:00:00Z';
ALTER TABLE MouvementStock ADD COLUMN Origine TEXT NOT NULL DEFAULT 'Manuelle';
ALTER TABLE MouvementStock ADD COLUMN QuantiteComptee NUMERIC NULL;
ALTER TABLE MouvementStock ADD COLUMN Ecart NUMERIC NULL;
CREATE INDEX IX_Produit_Nom ON Produit(Nom);
CREATE INDEX IX_MouvementStock_Date ON MouvementStock(DateMouvement);");
    }
    private static void ApplyV3(SqliteConnection db, SqliteTransaction tx)
    {
        Execute(db, tx, @"
ALTER TABLE Vente ADD COLUMN ModePaiement TEXT NOT NULL DEFAULT 'Espèces';
ALTER TABLE Vente ADD COLUMN MontantRecu NUMERIC NOT NULL DEFAULT 0;
ALTER TABLE Vente ADD COLUMN Monnaie NUMERIC NOT NULL DEFAULT 0;
ALTER TABLE Vente ADD COLUMN TentativeId TEXT NOT NULL DEFAULT '';
ALTER TABLE LigneVente ADD COLUMN ReferenceEnregistree TEXT NOT NULL DEFAULT '';
ALTER TABLE LigneVente ADD COLUMN NomEnregistre TEXT NOT NULL DEFAULT '';
CREATE UNIQUE INDEX IX_Vente_TentativeId ON Vente(TentativeId) WHERE TentativeId <> '';");
    }
    private static void ApplyV4(SqliteConnection db, SqliteTransaction tx)
    { Execute(db, tx, "CREATE TABLE IF NOT EXISTS Sauvegarde (Id INTEGER PRIMARY KEY AUTOINCREMENT, Chemin TEXT NOT NULL, DateSauvegarde TEXT NOT NULL, Automatique INTEGER NOT NULL, Resultat TEXT NOT NULL); CREATE INDEX IF NOT EXISTS IX_Sauvegarde_Date ON Sauvegarde(DateSauvegarde);"); }
    private static void Execute(SqliteConnection db, SqliteTransaction tx, string sql) { using var c = db.CreateCommand(); c.Transaction = tx; c.CommandText = sql; c.ExecuteNonQuery(); }
    private static object? Scalar(SqliteConnection db, SqliteTransaction tx, string sql) { using var c = db.CreateCommand(); c.Transaction = tx; c.CommandText = sql; return c.ExecuteScalar(); }
}

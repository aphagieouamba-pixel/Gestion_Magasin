namespace MagasinGestion.Models;

public enum RoleUtilisateur { Administrateur, Vendeur }
public sealed record Utilisateur(long Id, string NomUtilisateur, string NomComplet, RoleUtilisateur Role, bool Actif);
public sealed record Categorie(long Id, string Nom, string? Description);
public sealed record Produit(long Id, long? CategorieId, string Reference, string Nom, string? Description, decimal PrixVente, decimal StockMinimum, bool Actif, DateTime DateCreation);
public sealed record StockSnapshot(long ProduitId, decimal Stock, decimal Seuil, string Statut);
public sealed record Vente(long Id, string Numero, long UtilisateurId, DateTime DateVente, decimal Total, bool Annulee);
public sealed record LigneVente(long Id, long VenteId, long ProduitId, decimal Quantite, decimal PrixUnitaire, decimal Total);

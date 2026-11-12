// Scénarios de tests d'acceptation du module Produits/Stocks.
// Ils sont conservés ici pour être branchés à un runner xUnit/NUnit lorsque la restauration NuGet est disponible.
// Couvre: stock initial, référence dupliquée, sortie insuffisante, réapprovisionnement,
// stock faible, rupture, correction positive et correction négative.
namespace MagasinGestion.Tests;

public static class StockServiceScenarios
{
    public static readonly string[] Required = ["CreationProduitAvecStockInitial", "ReferenceDupliquee", "SortieInsuffisanteBloquee", "Reapprovisionnement", "PassageStockFaible", "PassageRupture", "CorrectionPositive", "CorrectionNegative"];
}

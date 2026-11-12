# Matrice de réception T01-T18

Les scénarios sont exécutés avec une base SQLite temporaire et vérifient les écritures SQL, les transactions et les règles métier.

| Test | Vérification |
|---|---|
| T01 | Produit + 10 unités + mouvement initial |
| T02 | Référence dupliquée refusée |
| T03 | Vente 3 x 2 000, total 6 000, stock 7 |
| T04 | Sortie supérieure au stock refusée |
| T05 | Stock nul, rupture, vente bloquée |
| T06 | Réapprovisionnement au seuil, stock faible |
| T07 | Ligne invalide : rollback complet |
| T08 | Identifiant de tentative et double validation |
| T09 | Annulation unique et mouvements inverses |
| T10 | Instantané du nom/prix historique |
| T11 | Rapports hors annulations |
| T12 | Espèces insuffisantes puis monnaie correcte |
| T13 | Persistance et réimpression sans nouvelle vente |
| T14 | Permissions service vendeur/admin |
| T15 | Sauvegarde, restauration, fichier invalide |
| T16 | Erreur SQLite et rollback |
| T17 | DPI Windows et taille minimale |
| T18 | CSV UTF-8 et protection formules |

L'exécution automatique complète est conditionnée à la restauration NuGet de `Microsoft.Data.Sqlite` dans l'environnement de développement.

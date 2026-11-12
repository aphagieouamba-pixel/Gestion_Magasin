# Magasin Gestion

Fondations d'une application de gestion de magasin hors ligne en C# WinForms, .NET 10 LTS et SQLite.

```powershell
dotnet restore
dotnet build .\src\MagasinGestion\MagasinGestion.csproj
dotnet run --project .\src\MagasinGestion\MagasinGestion.csproj
```

La base est créée dans `%LOCALAPPDATA%\MagasinGestion\magasin.db`. Aucun mot de passe administrateur n'est créé par défaut.

## Architecture

- `Forms` : écrans WinForms
- `Models` : entités métier
- `Services` : règles métier, permissions, ventes, stocks, rapports et sauvegardes
- `Data` : connexion SQLite et migrations
- `Security` : PBKDF2 et authentification

## Publication Windows x64

```powershell
dotnet publish .\src\MagasinGestion\MagasinGestion.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

La publication est destinée à produire un dossier autonome lançable sans Visual Studio. La base utilisateur n'est jamais publiée avec l'exécutable.

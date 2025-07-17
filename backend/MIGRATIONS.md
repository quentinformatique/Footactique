# Gestion des migrations Entity Framework Core

Ce projet utilise **Entity Framework Core** pour gérer l'évolution du schéma de la base de données PostgreSQL.

## Prérequis

- .NET 8 SDK installé
- Outil CLI EF Core installé (si besoin) :
  ```bash
  dotnet tool install --global dotnet-ef
  ```
- La base de données doit être accessible (voir `appsettings.json` pour la chaîne de connexion)

## Ajouter une migration

À chaque modification de modèle (C#), crée une migration :

```bash
# Syntaxe générale
# --project : projet où se trouvent les modèles/DbContext
# --startup-project : projet qui démarre l'app (API)
dotnet ef migrations add NomDeLaMigration --project Footactique.Services --startup-project Footactique.ApiService
```

Exemple :

```bash
dotnet ef migrations add AddPlayerNumber --project Footactique.Services --startup-project Footactique.ApiService
```

## Appliquer les migrations à la base

Pour mettre à jour la base de données avec toutes les migrations :

```bash
dotnet ef database update --project Footactique.Services --startup-project Footactique.ApiService
```

## Migration automatique en développement

En environnement **Development**, les migrations sont appliquées automatiquement au démarrage de l'API (voir `Program.cs`).

- **En production**, il est recommandé d'appliquer les migrations manuellement (ou via CI/CD) pour garder le contrôle.

## Bonnes pratiques

- **Crée une migration à chaque changement de modèle** (pas besoin de le faire à chaque commit, mais à chaque évolution du schéma).
- **Teste tes migrations en local** avant de les pousser.
- **Ne modifie jamais une migration déjà appliquée en prod**. Si besoin, crée une nouvelle migration pour corriger.
- **Versionne tes migrations** (elles sont dans le dossier `Migrations/` du projet `Footactique.Services`).

## Commandes utiles

- Voir la liste des migrations :
  ```bash
  dotnet ef migrations list --project Footactique.Services --startup-project Footactique.ApiService
  ```
- Supprimer la dernière migration (si non appliquée) :
  ```bash
  dotnet ef migrations remove --project Footactique.Services --startup-project Footactique.ApiService
  ```
- Générer un script SQL pour la migration :
  ```bash
  dotnet ef migrations script --project Footactique.Services --startup-project Footactique.ApiService
  ```

---

Pour toute question, consulte la doc officielle EF Core : https://learn.microsoft.com/fr-fr/ef/core/managing-schemas/migrations/

# Déploiement gratuit Azure App Service (F1)

Cette procédure publie la vitrine MOBI NARDO sur une URL `azurewebsites.net`. Le site tourne dans Azure, donc le PC local peut être éteint.

## Limites du forfait gratuit

- Le plan F1 est gratuit, partagé et limité en ressources. Microsoft le destine au développement et aux essais; les quotas CPU ou bande passante peuvent suspendre temporairement l’application. Il ne fournit pas de garantie de disponibilité ni l’option Always On. Le premier chargement après inactivité peut être lent. Voir [les quotas App Service](https://learn.microsoft.com/en-us/azure/app-service/web-sites-monitor) et [les niveaux des plans](https://learn.microsoft.com/en-us/azure/app-service/overview-hosting-plans).
- La base SQLite et les photos téléversées sont écrites sous `/home/data` (`/home/data/moto-mobi-nardo.db` et `/home/data/uploads`), dans le stockage persistant App Service, et non dans l’espace éphémère de l’application. Les deux doivent être sauvegardés ensemble. Voir [le stockage App Service](https://learn.microsoft.com/en-us/azure/app-service/operating-system-functionality).
- Le domaine existant `mobi-nardo.com` n’est pas modifié. Il reste servi par son hébergement actuel. Cette version aura sa propre URL Azure; le F1 gratuit ne comprend pas toutes les options de domaine personnalisé.
- Le fichier SQLite local n’est jamais déployé. Il peut contenir des clients, ventes et identifiants. La nouvelle base démarre vide, puis ajoute seulement la fiche publique Yamaha de `InitialMotorcycles`; les cinq annonces tierces viennent de `MarketReferences`.

## Prérequis

1. Installer [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli).
2. Ouvrir PowerShell dans le dossier du projet.
3. S’authentifier avec `az login` et vérifier que le compte possède une souscription Azure active.
4. Lancer le script:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\deploy\Deploy-AzureFree.ps1
```

Le script ne choisit que le plan F1 gratuit. Il publie l’application .NET 10, active `/home/data` comme dossier persistant, crée une clé aléatoire pour protéger la première configuration admin, puis vérifie l’URL. Il affiche ensuite:

- l’URL publique permanente `https://<nom>.azurewebsites.net`;
- un lien de configuration admin à usage unique.

Ouvre le lien de configuration toi-même, crée le compte administrateur avec ton e-mail et un mot de passe robuste, puis supprime `MOBI_NARDO_BOOTSTRAP_KEY` dans **Azure Portal → App Service → Environment variables**. Le compte initial doit être créé avant de partager l’URL admin.

## Références

- [Déployer une application ASP.NET Core sur Azure App Service](https://learn.microsoft.com/en-us/azure/app-service/quickstart-dotnetcore)
- [Commande Azure CLI `az webapp`](https://learn.microsoft.com/en-us/cli/azure/webapp)
- [Stockage persistant d’App Service](https://learn.microsoft.com/en-us/azure/app-service/operating-system-functionality)

Si Azure CLI n’est pas installé ou si aucune souscription n’est liée, la publication s’arrête avant toute création de ressource. Le site reste disponible sur le tunnel temporaire uniquement tant que le PC est allumé.

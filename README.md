# Moto Mobi Nardo

Édition commerciale pour showroom moto au Maroc · site concessionnaire de démonstration basé à Bouskoura.

## Hébergement public sans PC local

Consultez [DEPLOYMENT.md](DEPLOYMENT.md) et `deploy/Deploy-AzureFree.ps1` pour préparer Azure App Service F1 avec base SQLite et photos dans `/home/data`. Le compte Azure doit être authentifié avec `az login`. F1 est limité et n’offre pas de garantie de disponibilité; le domaine personnalisé doit être configuré sur une offre compatible et n’est pas modifié par ce script.

Application web de gestion d’une concession moto, réalisée en ASP.NET Core MVC et .NET 10.

## Démarrage

Dans le dossier du projet :

```powershell
dotnet run
```

Au premier lancement, ouvrez `/Account/Setup` pour créer le compte propriétaire, puis utilisez `/Account/Login`. En production, l’installation demande la clé ponctuelle `MOBI_NARDO_BOOTSTRAP_KEY`; elle est fermée après la création du premier compte.

## Vitrine publique

La page d’accueil affiche les motos dont le stock est supérieur à zéro. Les visiteurs peuvent filtrer les annonces, ouvrir WhatsApp ou envoyer une demande structurée pour disponibilité, prix, rappel, visite, financement ou demande de réservation. Leur accord de contact est requis. Les demandes sont limitées en fréquence, protégées contre les soumissions intersites et visibles dans « Prospects web ».

Le profil initial du concessionnaire est Bouskoura. Modifiez le nom, ville, contacts, logo, couleurs, carte, horaires et réseaux depuis **Profil du showroom** dans l’administration. Le numéro WhatsApp doit inclure l’indicatif international (`+212…`). Ajoutez une moto, indiquez sa catégorie et son état, son prix et son stock, puis téléversez une photo autorisée en JPG, PNG ou WebP (5 Mo maximum).

Les références de marché externes sont dans `MarketReferences` du même fichier. Elles restent séparées du stock géré et doivent être vérifiées régulièrement; elles ne représentent pas des motos vendues par la concession.

Les références externes configurées pointent vers des annonces publiées à Casablanca et restent séparées du stock du showroom de Bouskoura. Elles ne sont ni des motos vendues par la concession ni des prix garantis; disponibilité, papiers et prix sont à confirmer auprès des vendeurs. Les photos des annonces restent sur leur site source, faute de licence d’utilisation pour les republier.

## Offres commerciales

La page `/acquisition` présente les forfaits de mise en place proposés : Starter 2 500 DH, Business 3 500 DH et Premium 5 000 DH. Ce sont des montants ponctuels indicatifs configurés dans `ProjectOffer`; les frais de domaine, d’hébergement et la maintenance continue sont séparés. La page décrit les fonctions incluses et les éléments qui restent à développer ou à convenir.

Les demandes concernant les forfaits sont enregistrées dans `AcquisitionInquiries` et visibles aux utilisateurs authentifiés. Les demandes d’acheteurs et les demandes de clients du showroom restent séparées.

## Fonctionnalités

- Tableau de bord : chiffres d’affaires des ventes enregistrées, stock bas, demandes actives et demandes au stade réservé; marge nette non calculée faute de coûts d’achat et de dépenses enregistrés.
- Catalogue motos : recherche, ajout, modification, catégories moto/scooter, état neuf/occasion, stock et téléversement de photos.
- CRM léger : formulaire public avec consentement, étapes Nouveau → Contacté → Intéressé → Réservé → Vente conclue/Perdu, notes internes et réponse WhatsApp.
- Profil entreprise : personnalisation des coordonnées, de l’identité, des couleurs, des horaires, de la carte et des réseaux.
- Clients : coordonnées et recherche.
- Ventes : contrôle du stock, remise, référence unique, annulation avec restitution du stock.
- Facture de vente imprimable ou enregistrable en PDF depuis le navigateur. Vérifier les informations fiscales avec le concessionnaire avant usage comptable.
- Connexion obligatoire, mot de passe renforcé et verrouillage temporaire après plusieurs tentatives échouées. Un seul compte propriétaire est géré; rôles et comptes employés ne sont pas encore inclus.
- Stockage SQLite local dans `App_Data/moto-mobi-nardo.db`; les photos sont dans `App_Data/uploads` et ignorées par Git.
- Schéma de base versionné avec les migrations EF Core (`dotnet ef migrations add NomDeMigration`, puis `dotnet ef database update`).

## Diagrammes pour le rapport

Le dossier `docs` contient les diagrammes de cas d’utilisation, classes, données, séquence, activité, états, composants et déploiement : [docs/diagrammes-uml.md](docs/diagrammes-uml.md).

## Démonstration isolée

Lancez `tools/Start-Demo.ps1`. Le script utilise `App_Data/mobi-nardo-demo`, ajoute le label **DÉMONSTRATION** et crée un petit jeu de données fictives pour la vitrine, le CRM et le tableau de bord. Le code refuse d’activer ce mode si le dossier de données n’est pas précisément celui du démo. Créez un compte propriétaire avec `/Account/Setup`; aucun mot de passe de démonstration partagé n’est livré. N’importez pas de données personnelles dans cette base d’exemple.

## Documentation de livraison

- [Proposition commerciale en français](docs/PROPOSITION-COMMERCIALE-FR.md)
- [Script de démonstration en cinq minutes](docs/DEMO-5-MIN.md)
- [Guide utilisateur](docs/GUIDE-CLIENT-FR.md)
- [Checklist de livraison](docs/CHECKLIST-CLIENT.md)
- [Rapport de vérification](docs/TEST-REPORT.md)

## Équipe

## Avant une mise en production

Confirmez le stock réel, les prix, les photos autorisées, les coordonnées et l’adresse du showroom. Configurez un hébergement HTTPS, des sauvegardes testées et des alertes. La version actuelle ne contient pas de comptes employés, export Excel, rapprochement des dépenses, bénéfice net, paiement en ligne, notifications marketing, langues arabe/anglaise ni intégration IA externe. Faites valider les données fiscales avant d’utiliser une facture comme document officiel.


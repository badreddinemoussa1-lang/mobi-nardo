# Diagrammes UML — Moto Mobi Nardo

**Projet :** application de gestion d’une concession de motos  
**Technologies :** ASP.NET Core MVC, Entity Framework Core, SQLite, ASP.NET Core Identity  
**Équipe :** Ishaq · Jaouad Ouhammou · Badreddine

Les huit diagrammes sont écrits en **PlantUML**, avec un thème blanc, du texte foncé et des traits verts. Chaque lien ouvre le fichier `.puml` correspondant.

| N° | Diagramme | Fichier PlantUML |
|---:|---|---|
| 1 | Cas d’utilisation | [01-cas-utilisation.puml](plantuml/01-cas-utilisation.puml) |
| 2 | Classes | [02-classes.puml](plantuml/02-classes.puml) |
| 3 | Modèle de données (MCD) | [03-modele-donnees.puml](plantuml/03-modele-donnees.puml) |
| 4 | Séquence — enregistrer une vente | [04-sequence-vente.puml](plantuml/04-sequence-vente.puml) |
| 5 | Activité — cycle d’une vente | [05-activite-vente.puml](plantuml/05-activite-vente.puml) |
| 6 | États — cycle de vie d’une vente | [06-etats-vente.puml](plantuml/06-etats-vente.puml) |
| 7 | Composants | [07-composants.puml](plantuml/07-composants.puml) |
| 8 | Déploiement | [08-deploiement.puml](plantuml/08-deploiement.puml) |

## Comment les ouvrir

Ouvre un fichier `.puml` dans un éditeur compatible PlantUML pour visualiser ou exporter le diagramme en image. Le code source de chaque diagramme est aussi directement lisible dans le fichier.

## Périmètre représenté

Ces diagrammes décrivent l’application telle qu’elle est codée : création du premier compte propriétaire, authentification, gestion des motos et clients, ventes, annulation avec restitution du stock, historique et facture imprimable. L’application ne comprend pas encore la gestion de comptes employés, fournisseurs, achats ou réparations.

## Règles métier

1. Une vente utilise une moto et un client existants.
2. La quantité doit être positive et ne peut dépasser le stock disponible.
3. Le prix unitaire est enregistré au moment de la vente.
4. La vente et la diminution du stock sont sauvegardées dans la même opération.
5. L’annulation restitue le stock une fois et conserve la vente dans l’historique.
6. Une moto ou un client liés à des ventes ne peuvent pas être supprimés.

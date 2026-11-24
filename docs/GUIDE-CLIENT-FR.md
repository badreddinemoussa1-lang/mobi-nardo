# Guide rapide du concessionnaire

## Connexion et sécurité

1. Ouvrir `/Account/Login` avec le compte propriétaire créé lors de la livraison.
2. Garder le mot de passe privé. Ne pas utiliser le compte de démonstration pour les données réelles.
3. Se déconnecter après une session sur un appareil partagé.

## Profil du showroom

Dans **Profil du showroom**, renseigner nom, ville, adresse, téléphone, WhatsApp au format `+212…`, e-mail, logo, couleurs, Google Maps, horaires et réseaux. Les URL doivent commencer par `https://`. Les champs vides ne sont pas affichés.

## Ajouter ou modifier une moto

Dans **Catalogue motos**, saisir marque, modèle, année, cylindrée, prix en DH, stock, catégorie (moto/scooter), état (neuve/occasion), description et photo autorisée JPG/PNG/WebP de 4 Mo maximum. Une moto dont le stock est nul n’apparaît pas dans le catalogue public. Après une modification, vérifier la vitrine.

## Répondre aux demandes du site

Dans **Prospects web**, lire le motif, la moto, le numéro et le message. Répondre, changer l’étape et ajouter une note. « Demande de réservation » doit être confirmée par l’équipe : le site ne bloque pas le stock et n’encaisse pas d’acompte. Pour conclure, enregistrer la transaction dans **Ventes**.

## Vente et facture

Dans **Nouvelle vente**, choisir le client, la moto, la quantité, le canal et la remise. Vérifier le récapitulatif avant de valider. La vente diminue le stock; une annulation restitue la quantité. Imprimer la facture depuis le navigateur ou l’enregistrer en PDF. Faire valider les informations fiscales avant tout usage officiel.

## Tableau de bord

Les totaux utilisent les ventes actives enregistrées et les filtres d’année/marque. Les demandes web affichent le suivi ouvert. Le bénéfice net n’est pas calculé : les coûts d’achat et dépenses ne sont pas gérés dans cette édition.

## Sauvegarde et restauration

Arrêter l’application avant de copier la base SQLite (`App_Data/moto-mobi-nardo.db`) et `App_Data/uploads`. Conserver les deux dans un emplacement protégé à accès limité. Pour restaurer, arrêter l’application, remettre les fichiers, vérifier les permissions, puis redémarrer et valider catalogue, photos et ventes. Tester la restauration avant la mise en production.

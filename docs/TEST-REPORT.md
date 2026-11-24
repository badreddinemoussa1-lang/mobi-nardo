# Rapport de vérification technique

Date de vérification : 24 novembre 2026.

## Vérifications exécutées

- `dotnet build --no-restore` — réussi, 0 avertissement et 0 erreur.
- Lecture de `appsettings.json` par `ConvertFrom-Json` — réussi.
- GET `/` — HTTP 200; Bouskoura et formulaire de demande visibles.
- GET `/Acquisition` — HTTP 200; prix 2 500, 3 500 et 5 000 DH visibles.
- GET `/Leads` sans connexion — redirection vers la connexion observée.
- Envoi du formulaire public avec jeton antifalsification sur une base isolée — confirmation affichée.
- Connexion administrateur sur cette base, affichage du prospect — réussi.
- Mise à jour du prospect vers « Réservé » avec note interne — valeurs conservées après relecture.
- Téléversement d’un PNG de test puis GET `/Media/Photo/1` — HTTP 200, `image/png`.
- Mode démo avec chemin `mobi-nardo-demo` — HTTP 200, bannière visible et cinq fiches au catalogue; base distincte.
- Création du compte propriétaire de démo, affichage du tableau de bord et des demandes d’exemple — réussi. Les indicateurs jour/semaine/mois sont présents.

## Limites de la vérification

- Aucun contrôle visuel manuel dans un navigateur ni essai physique sur téléphone; les pages ont été contrôlées par HTTP local.
- Aucun vrai e-mail, paiement ou service externe n’a été testé.
- Export Excel, bénéfice net, dépenses, rôles employés, langues arabe/anglaise et assistant IA ne sont pas présents.
- L’application n’a pas été publiée sur Azure. L’authentification Azure demandée auparavant n’a pas été complétée; les URL Cloudflare précédentes étaient des tunnels temporaires.

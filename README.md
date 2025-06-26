# Blackjack Casino - Plugin pour Final Fantasy XIV

Ce projet est un plugin pour [Dalamud](https://github.com/goatcorp/Dalamud), qui ajoute une interface de jeu de Blackjack entièrement fonctionnelle dans Final Fantasy XIV.

## Description

Ce plugin permet aux joueurs de jouer au Blackjack (21) contre un croupier virtuel directement en jeu. Il comprend un système de sauvegarde persistant, le suivi des statistiques et des options de configuration pour personnaliser l'expérience de jeu. L'interface est conçue pour s'intégrer de manière transparente à l'esthétique du jeu.

## Fonctionnalités

- **Jeu de Blackjack Classique** : Jouez selon les règles traditionnelles du Blackjack. Tirez des cartes ("Hit"), restez ("Stand"), et tentez d'atteindre 21 sans dépasser.
- **Système de Sauvegarde** : 3 emplacements de sauvegarde pour conserver votre progression, incluant votre argent (Gils virtuels) et vos statistiques.
- **Suivi des Statistiques** : Pour chaque sauvegarde, le plugin enregistre :
    - L'argent du joueur
    - Le nombre de parties jouées
    - Le nombre de parties gagnées
    - Le taux de victoire
    - Le nombre de Blackjacks obtenus
- **Configuration Personnalisable** :
    - Gérez vos sauvegardes (créer, charger, réinitialiser, supprimer).
    - Définissez une mise par défaut.
- **Support Multilingue** : Le plugin prend en charge le Français et l'Anglais.

## Comment Utiliser

Pour commencer à jouer, utilisez les commandes de chat suivantes :

- `/blackjack` ou `/bj` : Ouvre la fenêtre principale du jeu.

Pour accéder à la configuration, vous pouvez passer par le menu des plugins de Dalamud et sélectionner "Blackjack Casino".

## Fenêtres du Plugin

### Fenêtre Principale

C'est ici que le jeu se déroule. Vous pouvez placer votre mise, jouer votre main et voir les cartes du croupier.

### Fenêtre de Configuration

La fenêtre de configuration vous permet de gérer les aspects suivants :

1.  **Gestion des Sauvegardes** :
    - Affiche les 3 emplacements de sauvegarde.
    - Permet de créer une nouvelle sauvegarde dans un emplacement vide.
    - Permet de charger, réinitialiser ou supprimer une sauvegarde existante.
    - Le slot actuellement chargé est mis en surbrillance.

2.  **Options de Jeu** :
    - **Mise par défaut** : Définissez le montant de Gils que vous souhaitez miser par défaut au début de chaque partie.

3.  **Paramètres de Langue** :
    - Changez la langue de l'interface entre le Français et l'Anglais.


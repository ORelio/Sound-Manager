=================================================================
==== Gestionnaire de Sons v3.4.0 - Par ORelio - Microzoom.fr ====
=========== https://microzoom.fr/gestionnaire-de-sons ===========
=================================================================

Merci d'avoir choisi le Gestionnaire de Sons !

Le Gestionnaire de Sons est un logiciel gratuit permettant de créer et partager des thèmes sonores pour Windows.
Toutes les versions de Windows de Windows XP SP3 à Windows 11 sont prises en charge.

Les principales fonctionnalités sont :
 - Lecture des sons manquants sous Windows 8 et supérieur
 - Exporter et importer les thème sonore via des fichiers archives
 - Importer les thèmes sonores créés via l'option "Sons" du panneau de configuration
 - Importer les archives propriétaires au format "soundpack"
 - Charger et tester des sons pour chaque évènement système
 - Définir des informations telles qu'une image, auteur, description pour votre thème
 - Conversion automatique des sons au format WAV (Windows 7+)
 - Patch du son de démarrage de Windows (droits Admin requis, Windows Vista+)

=============
 Utilisation
=============

Installation (si vous utilisez la version portable)
 - Extraire l'archive dans un endroit fixe tel que C:\Program Files\SoundManager ou Documents\SoundManager
 - Lancer SoundManager.exe, et activer les intégrations avec le système de votre choix dans l'onglet Paramètres
 - Si vous le souhaitez, créez un raccourci vers SoundManager.exe et placez-le sur votre Bureau

Désinstallation (si vous utilisez la version portable)
 - Lancer SoundManager.exe, et cliquer sur le bouton Désinstaller dans l'onglet Paramètres
 - Supprimer le répertoire SoundManager créé précédemment dans C:\Program Files ou Documents
 - Si créé précédemment, supprimer le raccourci présent sur le Bureau

Créer un thème sonore
 - Préparez des fichiers son, de préférence en WAV, via un programme d'édition tel qu'Audacity
 - Charger un thème sonore ou cliquer sur Réinitialiser pour partir du jeu de sons par défaut
 - Charger les sons un par un par glisser-déposer ou un clic droit > Remplacer
 - Définir les informations de votre thème : image, auteur, description...
 - Cliquer sur Exporter lorsque le résultat vous convient :)

Télécharger des thèmes
 - Obtenez des thèmes sur https://github.com/ORelio/Sound-Manager-Schemes
 - Ou alternativement, utilisez l'outil de téléchargement fourni via l'onglet À propos
   Dans ce cas, le dossier des thèmes est créé à côté de SoundManager.exe (mode portable) ou dans Musique (mode installé)
 - N'hésitez pas à m'envoyer les vôtres pour ajout sur le dépôt de thèmes :)

============================================
 Astuces pour la création de thèmes sonores
============================================

Un thème sonore fait partie intégrante de l'expérience utilisateur du système d'exploitation.
Voici quelques conseils pour que votre thème sonore joigne l'utile à l'agréable :

1. Fréquence et durée

   Choisissez une durée adaptée à la fréquence à laquelle un évènement sonore se produit.
   Vous éviterez ainsi la fatigue induite par la lecture trop fréquente d'un son trop long :

    Fréquence | Exemples de sons                       | Durée maximum conseillée
    ----------+----------------------------------------+--------------------------
    Très Rare | Démarrage, Arrêt, Erreur USB, Batterie | 10 secondes
    Rare      | Connexion, Déconnexion, Corbeille      | 5 secondes
    Régulie   | Information, Erreur, USB, Accès Admin  | 1 seconde
    Fréquent  | Par défaut, Lancer/Fermer App          | 300 millisecondes
    Répétitif | Navigation, Menu, Menu Clic            | 100 millisecondes

   En cas de doute, vous pouvez vous référer à la durée des sons du thème par défaut de Windows XP ou Vista/7.
   Pensez à retirer le silence avant et après l'effet sonore s'il y en a un dans le fichier son.

2. Volume sonore

   Gardez un volume cohérent entre vos différents fichiers, et par rapport au thème sonore par défaut de Windows :
   - L'évènement "Par défaut" sert de référence : Il est lu en ajustant le volume du PC depuis la zone de notification.
   - Le thème sonore doit avoir un volume raisonnablement bas pour être entendu sans gêner l'utilisation du PC.
   - Certains évènements très fréquents comme Navigation peuvent avoir un volume plus faible que les autres.

3. Groupes de sons

   Certains évènements peuvent être regroupés par similarité.
   Votre thème semblera plus cohérent à l'usage si les sons d'un même groupe présentent une similitude :
    - Allumage du PC : Démarrage, Arrêt
    - Session : Connexion, Déconnexion
    - Dialogues : Information, Question, Avertissement, Erreur
    - Périphériques : Ajout USB, Retrait USB, Erreur USB
    - Programmes : Lancer, Fermer, Minimiser, Restaurer, Agrandir, Réduire
    - Messagerie : Email, Rappel
    - Batterie : Faible, Critique
    - Menu déroulant : Menu, Menu Clic

4. Recycler les sons

   Idéalement, il faut définir au moins les sons allant de Démarrage à Accès Admin pour avoir un thème à peu près complet.
   Mais si vous créez un thème à partir de fichiers existants, il peut vous en manquer pour compléter votre thème.
   Plutôt que de laisser des évènements vides, vous pouvez essayer de recycler les sons à votre disposition :
    - Découper une portion d'un plus long et l'utiliser sur un évènement sonore plus court
    - Lire un son à l'envers, ex Ajout USB -> Retrait USB (à l'envers), Connexion -> Déconnexion (à l'envers)
    - En dernier recours, vous pouvez copier-coller des sons. Quelques conseils pour que cela se remarque moins :
       - Garder des sons différents au sein d'un même groupe, par exemple Evènements de session ou Boites de dialogue
       - Faire les copier-coller sur des évènements de teneur similaires mais de groupes différents :
          - Information -> Impression
          - Avertissement -> Accès Admin
          - Erreur -> Batterie critique
          - Infobulle -> Email

===================
 Notes de versions
===================

 - 1.0   : Version initiale du programme pour Windows XP SP2 FR, remplaçant les fichiers dans C:\Windows\Media
 - 1.1   : Prise en charge de Windows Vista, noms de fichiers différents dans C:\Windows\Media
 - 1.1b  : Correction de bug sur le chargement de thèmes
 - 1.1c  : Nouvelle icône et amélioration de la police d'écriture
 - 1.2   : Ajout de l'éditeur de thèmes directement dans le programme
 - 2.0   : Prise en charge de Windows 7, utilisation d'un thème sonore dédié dans le registre
 - 2.1   : Possibilité d'appliquer un fichier .ths directement en cliquant dessus
 - 3.0   : Réécriture en C#, passage en open source, traduction anglaise, prise en charge de Windows 8 et 10
 - 3.0.1 : Réduction du délai du son de démarrage sous Win 8+, correction de sons ne se lisant pas sous Win 10
 - 3.0.2 : Suppression du délai du son de démarrage sous Win 10, et prise en charge de Windows 11
 - 3.1.0 : Ajout de sons, correctif pour les fichiers en lecture seule, catégories sur l'outil de téléchargement
 - 3.1.1 : Correction du son de démarrage lorsqu'il y a plusieurs comptes utilisateur, bug apparu dans la v3.0.2
 - 3.2.0 : Ajout du support pour lire le format d'archive propriétaire "soundpack"
 - 3.2.1 : Refonte de l'icône du programme, correction de crash si lancé depuis un \\partage\réseau
 - 3.3.0 : Retravaillé le système de patch du son de démarrage, ajoutant le support pour Windows 8, 10 et 11
 - 3.3.1 : Accès rapide au fichier de configuration avancé, meilleure compatibilité avec les lecteurs d'écran
 - 3.4.0 : Ajout de la possibilité de désactiver un son sur votre PC. Correction Email sous Win 8+, Ajout Rappel

=====
 FAQ
=====

Q: Lorsque le patch du son de démarrage est activé, celui-ci n'est pas correctement mis à jour ?
R: Les fichiers système peuvent être en cours d'utilisation, essayez de redémarrer et réappliquer le thème sonore.
R: Une mise à jour majeure du système peut également enlever le patch, essayez de désactiver et réactiver l'option.

Q: Il y a des évènements sonores qui ne me plaisent pas. Comment les enlever ?
R: Clic droit sur l'évènement sonore non souhaité > Désactiver sur ce PC

Q : En utilisant le thème Windows XP, le son de démarrage devrait se lire à l'ouverture de session. Comment faire ?
R : Paramètres > Fichier de config. > régler « PreferStartupSoundOnLogon=True » et enregistrer
R : Si vous ne voyez pas le paramètre, vous pouvez l'ajouter en dessous des autres

Q: Y a-t-il un code source pour les versions 1.x et 2.x ?
R: Non, les premières versions étaient conçues avec Game Maker et plein de scripts batch.

================
 Remerciements
================

le Gestionnaire de Sons a été conçu en utilisant les ressources suivantes :

 - Bibliothèque Privilege20 du magazine MSDN, Mars 2005
 - Bibliothèque Tri-State Tree View, CodeProject no. 202435
 - Police Teko par Manushi Parikh (Logo)
 - Police Dancing Script par Pablo Impallari (Logo)
 - Icône Téléchargement par Microsoft Corporation
 - Icône Clipping Sound par RAD.E8

+--------------------+
| © 2009-2025 ORelio |
+--------------------+

=================================================================
==== Gestionnaire de Sons v3.0.0 - Par ORelio - Microzoom.fr ====
=================================================================

Merci d'avoir choisi le Gestionnaire de Sons !

Le Gestionnaire de Sons est un logiciel gratuit permettant de créer et partager des thèmes sonores pour Windows.
Toutes les versions de Windows de Windows XP SP3 à Windows 10 sont prises en charge.

Les principales fonctionnalités sont :
 - Charger et tester des sons pour chaque évènement système
 - Définir des informations telles qu'une image, auteur, description pour votre thème
 - Exporter et importer un thème sonore via un fichier d'archive
 - Importer les thèmes sonores créés via l'option "Sons" du panneau de configuration
 - Conversion automatique des sons au format WAV (Windows 7+)
 - Patch du son de démarrage sous Windows Vista/7 (droits Admin requis)
 - Lecture du son de démarrage/arrêt sous Windows 8 et 10

=============
 Utilisation
=============

Installation :
 - Extraire l'archive dans un endroit fixe tel que C:\Program Files\SoundManager ou Documents\SoundManager
 - Lancer SoundManager.exe, et activier les intégrations avec le système de votre choix dans l'onglet Paramètres
 - Si vous le souhaitez, créez un raccourci vers SoundManager.exe et placez-le sur votre Bureau

Désinstallation :
 - Lancer SoundManager.exe, et cliquer sur le bouton Désinstaller dans l'onglet Paramètres
 - Supprimer le répertoire SoundManager créé précédemment dans C:\Program Files ou Documents
 - Si créé précédemment, supprimer le raccourci présent sur le Bureau

Créer un thème sonore :
 - Préparez des fichiers son, de préférence en WAV, via un programme d'édition tel qu'Audacity
 - Il est recommandé de s'assurer que le volume est cohérent entre vos différents fichiers
 - Charger un thème sonore ou cliquer sur Réinitialiser pour partir du jeu de sons par défaut
 - Charger les sons un par un par glisser-déposer ou un clic droit > Remplacer
 - Définir les informations de votre thème : image, auteur, description...
 - Cliquer sur Exporter lorsque le résultat vous convient :)

Télécharger des thèmes
 - Obtenez des thèmes sur https://github.com/ORelio/Sound-Manager-Schemes
 - N'hésitez pas à m'envoyer les vôtres pour ajout sur le dépôt de thèmes :)

===================
 Notes de versions
===================

 - 1.0  : Version initiale du programme pour Windows XP SP2 FR, remplaçant les fichiers dans C:\Windows\Media
 - 1.1  : Prise en charge de Windows Vista, noms de fichiers différents dans C:\Windows\Media
 - 1.1b : Correction de bug sur le chargement de thèmes
 - 1.1c : Nouvelle icône et amélioration de la police d'écriture
 - 1.2  : Ajout de l'éditeur de thèmes directement dans le programme
 - 2.0  : Prise en charge de Windows 7, utilisation d'un thème sonore dédié dans le registre
 - 2.1  : Possibilité d'appliquer un fichier .ths directement en cliquant dessus
 - 3.0  : Réécriture en C#, passage en open source, traduction anglaise, prise en charge de Windows 8 et 10

=====
 FAQ
=====

Q: Sur Windows 7, le son de démarrage n'est pas toujours mis à jour?
R: Les fichiers DLL peuvent être en cours d'utilisation, redémarrez Windows et re-chargez le thème.

Q: Sur Windows 8 et 10, le son de démarrage met du temps à être lu?
R: Ces versions de Windows ajoutent un délai avant de lancer les applications au démarrage.

Q: Y a-t-il un code source pour les versions 1.x et 2.x ?
R: Non, les premières versions étaient conçues avec Game Maker et plein de scripts batch.

================
 Remerciements
================

le Gestionnaire de Sons a été conçu en utilisant les ressources suivantes :

 - Bibliothèque Privilege20 du magazine MSDN, Mars 2005
 - Icône Windows Media Player par Microsoft Corporation
 - Resource Hacker par Angus Johnson
 - Icône Clipping Sound par RAD.E8

+--------------------+
| © 2009-2018 ORelio |
+--------------------+
using System;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SharpTools
{
    /// <summary>
    /// Allows to localize the app in different languages
    /// </summary>
    /// <remarks>
    /// By ORelio (c) 2015-2018 - CDDL 1.0
    /// </remarks>
    public static class Translations
    {
        private static Dictionary<string, string> translations;
        public static readonly string SystemLanguage = CultureInfo.CurrentCulture.ThreeLetterISOLanguageName;

        /// <summary>
        /// Return a tranlation for the requested text
        /// </summary>
        /// <param name="msg_name">text identifier</param>
        /// <returns>returns translation for this identifier</returns>
        public static string Get(string msg_name)
        {
            if (translations.ContainsKey(msg_name))
                return translations[msg_name];

            return msg_name.ToUpper();
        }

        /// <summary>
        /// Initialize translations depending on system language.
        /// English is the default for all unknown system languages.
        /// </summary>
        static Translations()
        {
            translations = new Dictionary<string, string>();

            /*
             * External translation files
             * These files are loaded from the installation directory as:
             * Lang/abc.ini, e.g. Lang/eng.ini which is the default language file
             * Useful for adding new translations of fixing typos without recompiling
             */

            string langDir = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "Lang" + Path.DirectorySeparatorChar;
            string langFileSystemLanguage = langDir + SystemLanguage + ".ini";
            string langFile = File.Exists(langFileSystemLanguage) ? langFileSystemLanguage : langDir + "eng.ini";

            if (File.Exists(langFile))
            {
                foreach (string lineRaw in File.ReadAllLines(langFile, Encoding.UTF8))
                {
                    //This only handles a subset of the INI format:
                    //key=value pairs, no sections, no inline comments.
                    string line = lineRaw.Trim();
                    string translationName = line.Split('=')[0];
                    if (line.Length > (translationName.Length + 1))
                    {
                        string translationValue = line.Substring(translationName.Length + 1);
                        translations[translationName] = translationValue;
                    }
                }
            }

            /* 
             * Hardcoded translation data
             * This data is used as fallback if no translation file could be loaded
             * Useful for standalone exe portable apps
             */

            else if (SystemLanguage == "fra")
            {
                translations["app_name"] = "Gestionnaire de Sons";
                translations["app_desc"] = "Créez et partagez des thèmes sonores pour Windows !";
                translations["translation_author"] = ""; //"Traduit par VOUS"
                translations["no_image"] = "(Aucun visuel)";
                translations["meta_name"] = "Nom";
                translations["meta_author"] = "Auteur";
                translations["meta_about"] = "À propos";
                translations["button_open"] = "Ouvrir";
                translations["button_import"] = "Importer";
                translations["button_export"] = "Exporter";
                translations["button_reset"] = "Réinitialiser";
                translations["button_exit"] = "Quitter";
                translations["event_startup_name"] = "Démarrage";
                translations["event_startup_desc"] = "Démarrage de Windows";
                translations["event_shutdown_name"] = "Arrêt";
                translations["event_shutdown_desc"] = "Arrêt de Windows";
                translations["event_logon_name"] = "Connexion";
                translations["event_logon_desc"] = "Ouverture de session";
                translations["event_logoff_name"] = "Déconnexion";
                translations["event_logoff_desc"] = "Fermeture de session";
                translations["event_information_name"] = "Information";
                translations["event_information_desc"] = "Message d'information";
                translations["event_warning_name"] = "Avertissement";
                translations["event_warning_desc"] = "Message d'avertissement";
                translations["event_error_name"] = "Erreur";
                translations["event_error_desc"] = "Message d'erreur";
                translations["event_question_name"] = "Question";
                translations["event_question_desc"] = "Message interrogatif";
                translations["event_deviceconnect_name"] = "Ajout USB";
                translations["event_deviceconnect_desc"] = "Connexion d'un périphérique USB";
                translations["event_devicedisconnect_name"] = "Retrait USB";
                translations["event_devicedisconnect_desc"] = "Déconnexion d'un périphérique USB";
                translations["event_devicefail_name"] = "Erreur USB";
                translations["event_devicefail_desc"] = "Échec d'un périphérique USB";
                translations["event_default_name"] = "Par défaut";
                translations["event_default_desc"] = "Son par défaut de Windows";
                translations["event_balloon_name"] = "Infobulle";
                translations["event_balloon_desc"] = "Notifications";
                translations["event_navigate_name"] = "Navigation";
                translations["event_navigate_desc"] = "Navigation dans l'Explorateur et Internet Explorer";
                translations["event_recyclebin_name"] = "Corbeille";
                translations["event_recyclebin_desc"] = "Son de vidage de la corbeille";
                translations["event_uac_name"] = "Accès Admin";
                translations["event_uac_desc"] = "Lorsqu'une application demande les droits Administrateur (Vista et supérieur)";
                translations["tab_current_scheme"] = "Thème sonore";
                translations["tab_settings"] = "Paramètres";
                translations["default_scheme_name"] = "Sons Windows par défaut";
                translations["default_scheme_author"] = "Microsoft Corporation";
                translations["default_scheme_about"] = "https://www.microsoft.com/";
                translations["sound_load_failed_text"] = "Le chargement du fichier son a échoué :";
                translations["sound_load_failed_title"] = "Echec de chargement du son";
                translations["sound_file_too_long"] = "Le fichier son a une durée trop élevée.";
                translations["image_load_failed_text"] = "Le chargement de l'image a échoué :";
                translations["image_load_failed_title"] = "Echec de chargement de l'image";
                translations["scheme_load_prompt_text"] = "Souhaitez-vous charger ce fichier de thème ?";
                translations["scheme_load_prompt_title"] = "Chargement du thème";
                translations["scheme_load_failed_text"] = "Le chargement du thème a échoué :";
                translations["scheme_load_failed_title"] = "Echec de chargement du thème";
                translations["scheme_export_failed_text"] = "L'export du thème a échoué :";
                translations["scheme_export_failed_title"] = "Echec de l'export du thème";
                translations["browse_failed_text"] = "Le démarrage de l'Explorateur Windows a échoué :";
                translations["browse_failed_title"] = "Echec de la navigation";
                translations["reset_warn_text"] = "Le thème par défaut va être chargé et les sons actuels seront perdus.";
                translations["reset_warn_title"] = "Réinitialisation du thème";
                translations["browse_wave_files"] = "Fichiers audio Wave";
                translations["browse_media_files"] = "Fichiers multimédia";
                translations["browse_image_files"] = "Images";
                translations["browse_scheme_files"] = "Thèmes sonores";
                translations["image_change"] = "Changer d'image";
                translations["image_remove"] = "Supprimer l'image";
                translations["sound_change"] = "Remplacer le son";
                translations["sound_play"] = "Jouer le son";
                translations["sound_open_location"] = "Accéder au fichier";
                translations["sound_reset"] = "Réinitialiser le son";
                translations["sound_remove"] = "Supprimer le son";
                translations["webpage_open_prompt_text"] = "Souhaitez-vous visiter le lien suivant ?";
                translations["webpage_open_prompt_title"] = "Confirmation d'ouverture de lien";
                translations["windows7_not_elevated_title"] = "Droits Administrateur manquants";
                translations["windows7_not_elevated_text"] = "Relancez l'application en tant qu'Administrateur pour mettre à jour le son de démarrage de Windows Vista/7.";
                translations["startup_patch_not_admin"] = "Le son de démarrage de Windows Vista/7 ne peut être modifié car l'application ne dispose pas des droits Administrateur.";
                translations["startup_patch_no_imageres_dll"] = "Le son de démarrage de Windows Vista/7 ne peut être modifié car le fichier imageres.dll est introuvable.";
                translations["startup_patch_not_windows7"] = "Le son de démarrage de Windows Vista/7 ne peut pas être modifié car le système actuel n'est pas Windows NT 6.1";
                translations["startup_patch_no_resource_hacker"] = "Le son de démarrage de Windows Vista/7 ne peut pas être accédé car l'application ResHacker.exe est introuvable.";
                translations["playing_shutdown_sound"] = "Lecture du son d'arrêt de l'ordinateur";
                translations["playing_logoff_sound"] = "Lecture du son de fermeture de session";
                translations["scheme_file_desc"] = "Fichier de Thème Sonore";
                translations["box_import_system_scheme"] = "Importer un thème sonore du système";
                translations["box_system_integration"] = "Intégration avec le système";
                translations["check_box_imageres_patch"] = "Patcher le son de démarrage de Windows Vista/7";
                translations["check_box_bg_sound_player"] = "Démarrer avec Windows pour jouer le son de démarrage/arrêt (Win 8+)";
                translations["check_box_file_assoc"] = "Associer avec les fichiers de thème sonores";
                translations["check_box_reset_missing_on_load"] = "Laisser le son du système lorsqu'il manque un son dans un thème";
                translations["reinstall_confirm_title"] = "Réinitialiser l'application";
                translations["reinstall_confirm_text"] = "Tous les paramètres de l'application vont être réinitialisés et le thème sonore actuel sera perdu.";
                translations["uninstall_confirm_title"] = "Désinstaller l'application";
                translations["uninstall_confirm_text"] = "Les paramètres du système vont être restaurés, les fichiers de l'application supprimés l'application va se quitter.";
                translations["box_maintenance"] = "Maintenance";
                translations["button_reinstall"] = "Réinstaller";
                translations["button_uninstall"] = "Désinstaller";
                translations["tab_about"] = "À propos...";
                translations["help_file"] = "Readme-Fr.txt";
                translations["help_file_not_found_title"] = "Fichier d'aide non trouvé";
                translations["help_file_not_found_text"] = "Le fichier d'aide est manquant. Essayez de consulter le site Internet pour l'obtenir.";
                translations["button_help"] = "Consulter l'aide";
                translations["button_website"] = "Voir mon site";
                translations["box_system_info"] = "Informations système";
                translations["supported_system_version"] = "Cette version de Windows est prise en charge par l'application.";
                translations["unsupported_system_version"] = "L'application n'a pas été testée sur cette version de Windows et *POURRAIT* ne pas fonctionner comme prévu.";
                translations["drag_drop_no_target_sound_title"] = "Aucun son sélectionné";
                translations["drag_drop_no_target_sound_text"] = "Veuillez sélectionner un son avant de déposer un fichier.";
                translations["drag_drop_sound_confirm_title"] = "Confirmer le remplacement du son";
                translations["drag_drop_sound_confirm_text"] = "Remplacer ce son ?";
                translations["download_schemes_no_tls_title"] = "Configuration TLS/HTTPS insuffisante";
                translations["download_schemes_no_tls_text"] = "La configuration HTTPS ne permet pas le téléchargement des thèmes. Afficher la liste dans votre navigateur ?";
                translations["download_schemes_failed_title"] = "Échec du téléchargement";
                translations["download_schemes_failed_text"] = "Le téléchargement des thèmes a échoué. Afficher la liste dans votre navigateur ?";
                //Ajouter de nouvelles traductions ici
            }
            //Add new languages here as 'else if' blocks
            //English is the default language in 'else' block below
            else
            {
                translations["app_name"] = "Sound Manager";
                translations["app_desc"] = "Manage and share Windows sound schemes !";
                translations["translation_author"] = ""; //"Translated by YOU"
                translations["no_image"] = "(No image)";
                translations["meta_name"] = "Name";
                translations["meta_author"] = "Author";
                translations["meta_about"] = "About";
                translations["button_open"] = "Open";
                translations["button_import"] = "Import";
                translations["button_export"] = "Export";
                translations["button_reset"] = "Reset";
                translations["button_exit"] = "Exit";
                translations["event_startup_name"] = "Startup";
                translations["event_startup_desc"] = "System start";
                translations["event_shutdown_name"] = "Shutdown";
                translations["event_shutdown_desc"] = "System shutdown";
                translations["event_logon_name"] = "Log On";
                translations["event_logon_desc"] = "User session resume";
                translations["event_logoff_name"] = "Log Off";
                translations["event_logoff_desc"] = "User session suspend";
                translations["event_information_name"] = "Information";
                translations["event_information_desc"] = "Information message sound";
                translations["event_warning_name"] = "Warning";
                translations["event_warning_desc"] = "Warning message";
                translations["event_error_name"] = "Error";
                translations["event_error_desc"] = "Error message";
                translations["event_question_name"] = "Question";
                translations["event_question_desc"] = "Interrogative message";
                translations["event_deviceconnect_name"] = "USB Connect";
                translations["event_deviceconnect_desc"] = "Connecting a USB device";
                translations["event_devicedisconnect_name"] = "USB Remove";
                translations["event_devicedisconnect_desc"] = "Removing a USB device";
                translations["event_devicefail_name"] = "USB Error";
                translations["event_devicefail_desc"] = "USB device failure";
                translations["event_default_name"] = "Default";
                translations["event_default_desc"] = "Default Windows sound";
                translations["event_balloon_name"] = "Balloon";
                translations["event_balloon_desc"] = "Notification";
                translations["event_navigate_name"] = "Navigate";
                translations["event_navigate_desc"] = "Navigate in Explorer and Internet Explorer";
                translations["event_recyclebin_name"] = "Recycle Bin";
                translations["event_recyclebin_desc"] = "Emptying the recycle bin";
                translations["event_uac_name"] = "Admin Access";
                translations["event_uac_desc"] = "When an application requests Administrator privileges (Vista and greater)";
                translations["tab_current_scheme"] = "Sound scheme";
                translations["tab_settings"] = "Settings";
                translations["default_scheme_name"] = "Default Windows sounds";
                translations["default_scheme_author"] = "Microsoft Corporation";
                translations["default_scheme_about"] = "https://www.microsoft.com/";
                translations["sound_load_failed_text"] = "This sound file failed to load:";
                translations["sound_load_failed_title"] = "Sound load failure";
                translations["sound_file_too_long"] = "The sound file duration is too long.";
                translations["image_load_failed_text"] = "This image failed to load:";
                translations["image_load_failed_title"] = "Image load failure";
                translations["scheme_load_prompt_text"] = "Would you like to load this sound scheme?";
                translations["scheme_load_prompt_title"] = "Load a sound scheme";
                translations["scheme_load_failed_text"] = "This sound scheme failed to load:";
                translations["scheme_load_failed_title"] = "Scheme load failure";
                translations["scheme_export_failed_text"] = "This sound scheme failed to export:";
                translations["scheme_export_failed_title"] = "Scheme load failure";
                translations["browse_failed_text"] = "Failed to launch Windows Explorer:";
                translations["browse_failed_title"] = "Failed to browse to location";
                translations["reset_warn_text"] = "The default sound scheme will be loaded and the current sounds will be lost.";
                translations["reset_warn_title"] = "Resetting the sound scheme";
                translations["browse_wave_files"] = "Wave audio files";
                translations["browse_media_files"] = "Media files";
                translations["browse_image_files"] = "Images";
                translations["browse_scheme_files"] = "Sound schemes";
                translations["image_change"] = "Change image";
                translations["image_remove"] = "Remove image";
                translations["sound_change"] = "Replace sound";
                translations["sound_play"] = "Play sound";
                translations["sound_open_location"] = "Show in Explorer";
                translations["sound_reset"] = "Reset sound";
                translations["sound_remove"] = "Remove sound";
                translations["webpage_open_prompt_text"] = "Do you wish to visit the following link?";
                translations["webpage_open_prompt_title"] = "Link opening confirmation";
                translations["windows7_not_elevated_title"] = "Missing Administrator privileges";
                translations["windows7_not_elevated_text"] = "Relaunch the application as Administrator in order to update the Windows 7 startup sound.";
                translations["startup_patch_not_admin"] = "Cannot modify the Windows 7 startup sound: missing Administrator permissions.";
                translations["startup_patch_no_imageres_dll"] = "Cannot modify the Windows 7 startup sound: cannot find imageres.dll.";
                translations["startup_patch_not_windows7"] = "Cannot modify the Windows 7 startup sound: the operating system is not Windows NT 6.1";
                translations["startup_patch_no_resource_hacker"] = "Cannot access the Windows 7 startup sound: cannot find ResHacker.exe.";
                translations["playing_shutdown_sound"] = "Playing Shutdown sound";
                translations["playing_logoff_sound"] = "Playing Logoff sound";
                translations["scheme_file_desc"] = "Sound Scheme File";
                translations["box_import_system_scheme"] = "Import a system sound scheme";
                translations["box_system_integration"] = "System integration";
                translations["check_box_imageres_patch"] = "Patch the Windows 7 startup sound";
                translations["check_box_bg_sound_player"] = "Start with Windows to play the system start/stop sounds (Windows 8+)";
                translations["check_box_file_assoc"] = "Associate with sound scheme files";
                translations["check_box_reset_missing_on_load"] = "Leave system sound when a scheme lacks a sound";
                translations["reinstall_confirm_title"] = "Reset the application";
                translations["reinstall_confirm_text"] = "All application settings will be reset and the current sound scheme will be lost.";
                translations["uninstall_confirm_title"] = "Désinstaller l'application";
                translations["uninstall_confirm_text"] = "System settings will be restored, all application data will be removed and the app will exit.";
                translations["box_maintenance"] = "Maintenance";
                translations["button_reinstall"] = "Reinstall";
                translations["button_uninstall"] = "Uninstall";
                translations["tab_about"] = "About...";
                translations["help_file"] = "Readme-En.txt";
                translations["help_file_not_found_title"] = "Missing help file";
                translations["help_file_not_found_text"] = "The help file is missing. Try visiting the website to grab it.";
                translations["button_help"] = "View Help";
                translations["button_website"] = "Visit my website";
                translations["box_system_info"] = "System information";
                translations["supported_system_version"] = "This Windows version is supported by the application.";
                translations["unsupported_system_version"] = "The application has not been tested on this Windows version and as such *MIGHT* not perform as expected.";
                translations["drag_drop_no_target_sound_title"] = "No selected sound";
                translations["drag_drop_no_target_sound_text"] = "Please select a target sound before dropping a file.";
                translations["drag_drop_sound_confirm_title"] = "Confirm sound replacement";
                translations["drag_drop_sound_confirm_text"] = "Replace this sound?";
                translations["download_schemes_no_tls_title"] = "TLS/HTTPS configuration is inadequate";
                translations["download_schemes_no_tls_text"] = "The current HTTPS configuration does not allow downloading. View scheme list in your web browser?";
                translations["download_schemes_failed_title"] = "Download failed";
                translations["download_schemes_failed_text"] = "Failed to download sound schemes. View scheme list in your web browser?";
                //Add new translations here
            }
        }
    }
}

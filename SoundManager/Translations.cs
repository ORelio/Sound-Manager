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
                translations["meta_name_desc"] = "Nom du thème sonore";
                translations["meta_author_desc"] = "Auteur du thème sonore";
                translations["meta_about_desc"] = "Description ou lien vers le site web";
                translations["button_open"] = "Ouvrir";
                translations["button_import"] = "Importer";
                translations["button_export"] = "Exporter";
                translations["button_reset"] = "Réinitialiser";
                translations["button_exit"] = "Quitter";
                translations["button_ok"] = "OK";
                translations["button_cancel"] = "Cancel";
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
                translations["event_batterylow_name"] = "Batterie faible";
                translations["event_batterylow_desc"] = "Avertissement de batterie faible";
                translations["event_batterycritical_name"] = "Batterie critique";
                translations["event_batterycritical_desc"] = "Avertissement de batterie critique";
                translations["event_email_name"] = "Email";
                translations["event_email_desc"] = "Réception d'un Email";
                translations["event_reminder_name"] = "Rappel";
                translations["event_reminder_desc"] = "Rappel de calendrier";
                translations["event_print_name"] = "Impression";
                translations["event_print_desc"] = "Impression terminée";
                translations["event_appopen_name"] = "Lancer App";
                translations["event_appopen_desc"] = "Lancement d'une application";
                translations["event_appclose_name"] = "Fermer App";
                translations["event_appclose_desc"] = "Fermeture d'une application";
                translations["event_minimize_name"] = "Minimiser App";
                translations["event_minimize_desc"] = "Réduire une application dans la barre des tâches";
                translations["event_unminimize_name"] = "Restaurer App";
                translations["event_unminimize_desc"] = "Restaurer une application depuis la barre des tâches";
                translations["event_maximize_name"] = "Agrandir app";
                translations["event_maximize_desc"] = "Agrandir une application pour occuper tout l'écran";
                translations["event_unmaximize_name"] = "Réduire app";
                translations["event_unmaximize_desc"] = "Réduire une application qui occupait tout l'écran";
                translations["event_menu_name"] = "Menu";
                translations["event_menu_desc"] = "Ouverture d'un menu contextuel";
                translations["event_menucommand_name"] = "Menu Clic";
                translations["event_menucommand_desc"] = "Sélection d'une option dans un menu contextuel";
                translations["event_select_name"] = "Sélection";
                translations["event_select_desc"] = "Sélection d'un élément sur le bureau ou dans l'explorateur de fichiers";
                translations["event_loadscheme_name"] = "Charger Thème";
                translations["event_loadscheme_desc"] = "Chargement du thème sonore";
                translations["system_event_disable_confirm_title"] = "Désactivation d'un son sur ce PC";
                translations["system_event_disable_confirm_text"] = "Lorsqu'un son est désactivé sur ce PC, vous pouvez toujours l'importer, modifier, exporter, mais il ne sera pas lu sur ce PC.";
                translations["system_event_disabled_label"] = "Désactivé";
                translations["system_event_disabled_desc"] = "Désactivé sur ce PC";
                translations["system_event_disable"] = "Désactiver sur ce PC";
                translations["system_event_enable"] = "Activer sur ce PC";
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
                translations["scheme_load_prompt_text"] = "Souhaitez-vous charger ce fichier de thème ?";
                translations["scheme_load_prompt_title"] = "Chargement du thème";
                translations["scheme_load_failed_text"] = "Le chargement du thème a échoué :";
                translations["scheme_load_failed_title"] = "Echec de chargement du thème";
                translations["scheme_export_failed_text"] = "L'export du thème a échoué :";
                translations["scheme_export_failed_title"] = "Echec de l'export du thème";
                translations["scheme_view_list"] = "Affichage Liste";
                translations["scheme_view_tiles"] = "Affichage Mosaïques";
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
                translations["webpage_open_prompt_text"] = "Souhaitez-vous visiter le lien suivant ?";
                translations["webpage_open_prompt_title"] = "Confirmation d'ouverture de lien";
                translations["startup_patch_not_elevated_title"] = "Droits Administrateur manquants";
                translations["startup_patch_not_elevated_text"] = "Relancez l'application en tant qu'Administrateur pour patcher le son de démarrage de Windows.";
                translations["startup_patch_not_admin"] = "Le son de démarrage de Windows ne peut être patché car l'application ne dispose pas des droits Administrateur.";
                translations["startup_patch_no_imageres_dll"] = "Le son de démarrage de Windows ne peut être patché car le fichier imageres.dll est introuvable.";
                translations["startup_patch_not_possible"] = "Le son de démarrage de Windows ne peut pas être patché: opération non gérée pour le système actuel.";
                translations["startup_patch_not_recommended_title"] = "Patch du son de démarrage non recommandé";
                translations["startup_patch_not_recommended_text"] = "Patcher le son de démarrage requiert les droits admin et pourrait ne plus marcher après une mise à jour majeure du système. Continuer ?";
                translations["playing_shutdown_sound"] = "Lecture du son d'arrêt de l'ordinateur";
                translations["playing_logoff_sound"] = "Lecture du son de fermeture de session";
                translations["scheme_file_desc"] = "Fichier de Thème Sonore";
                translations["scheme_file_proprietary_desc"] = "Fichier de Thème Sonore (Format propriétaire)";
                translations["box_import_system_scheme"] = "Importer un thème sonore du système";
                translations["box_system_integration"] = "Intégration avec le système";
                translations["check_box_imageres_patch"] = "Patcher le son de démarrage du système";
                translations["check_box_bg_sound_player"] = "Démarrer avec Windows pour jouer les sons manquants";
                translations["check_box_file_assoc"] = "Associer avec les fichiers de thème sonores";
                translations["check_box_reset_missing_on_load"] = "Laisser le son du système lorsqu'il manque un son dans un thème";
                translations["reinstall_confirm_title"] = "Réinitialiser l'application";
                translations["reinstall_confirm_text"] = "Tous les paramètres de l'application vont être réinitialisés et le thème sonore actuel sera perdu.";
                translations["uninstall_confirm_title"] = "Désinstaller l'application";
                translations["uninstall_confirm_text"] = "Les paramètres du système vont être restaurés, les fichiers de l'application supprimés l'application va se quitter.";
                translations["config_file_confirm_title"] = "Configuration avancée";
                translations["config_file_confirm_text"] = "N'éditez le fichier de configuration que si vous savez ce que vous faites. L'application va se quitter.";
                translations["box_maintenance"] = "Maintenance";
                translations["button_reinstall"] = "Réinstaller";
                translations["button_uninstall"] = "Désinstaller";
                translations["button_config_file"] = "Fichier de config.";
                translations["tab_about"] = "À propos...";
                translations["help_file"] = "Readme-Fr.txt";
                translations["help_file_not_found_title"] = "Fichier d'aide non trouvé";
                translations["help_file_not_found_text"] = "Le fichier d'aide est manquant. Essayez de consulter le site Internet pour l'obtenir.";
                translations["button_help"] = "Consulter l'aide";
                translations["button_website"] = "Voir mon site";
                translations["button_download_schemes"] = "Télécharger des thèmes sonores";
                translations["box_system_info"] = "Informations système";
                translations["supported_system_version"] = "Cette version de Windows est prise en charge par l'application.";
                translations["unsupported_system_version"] = "L'application n'a pas été testée sur cette version de Windows et *POURRAIT* ne pas fonctionner comme prévu.";
                translations["drag_drop_no_target_sound_title"] = "Aucun son sélectionné";
                translations["drag_drop_no_target_sound_text"] = "Veuillez sélectionner un son avant de déposer un fichier.";
                translations["drag_drop_sound_confirm_title"] = "Confirmer le remplacement du son";
                translations["drag_drop_sound_confirm_text"] = "Remplacer ce son ?";
                translations["download_schemes_selection_title"] = "Télécharger des thèmes";
                translations["download_schemes_selection_text"] = "Sélectionnez les thèmes à télécharger";
                translations["download_schemes_selection_fetching_list"] = "Récupération de la liste...";
                translations["download_schemes_selection_everything"] = "Tous les thèmes sonores";
                translations["download_schemes_no_tls_title"] = "Configuration TLS/HTTPS insuffisante";
                translations["download_schemes_no_tls_text"] = "La configuration HTTPS ne permet pas le téléchargement des thèmes. Afficher la liste dans votre navigateur ?";
                translations["download_schemes_failed_title"] = "Échec du téléchargement";
                translations["download_schemes_failed_text"] = "Le téléchargement des thèmes a échoué. Afficher la liste dans votre navigateur ?";
                translations["auto_import_offer_title"] = "Importer ce thème sonore ?";
                translations["auto_import_offer_text"] = "Le thème sonore du système a changé, souhaitez-vous l'importer ? Tout projet non sauvegardé sera perdu.";
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
                translations["meta_name_desc"] = "Sound scheme name";
                translations["meta_author_desc"] = "Sound scheme author";
                translations["meta_about_desc"] = "Description or website link";
                translations["button_open"] = "Open";
                translations["button_import"] = "Import";
                translations["button_export"] = "Export";
                translations["button_reset"] = "Reset";
                translations["button_exit"] = "Exit";
                translations["button_ok"] = "OK";
                translations["button_cancel"] = "Annuler";
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
                translations["event_batterylow_name"] = "Battery Low";
                translations["event_batterylow_desc"] = "Low battery warning";
                translations["event_batterycritical_name"] = "Battery Critical";
                translations["event_batterycritical_desc"] = "Critical battery warning";
                translations["event_email_name"] = "Email";
                translations["event_email_desc"] = "Receiving an Email";
                translations["event_reminder_name"] = "Reminder";
                translations["event_reminder_desc"] = "Calendar reminder";
                translations["event_print_name"] = "Print";
                translations["event_print_desc"] = "Print complete";
                translations["event_appopen_name"] = "Launch App";
                translations["event_appopen_desc"] = "Launching an application";
                translations["event_appclose_name"] = "Close App";
                translations["event_appclose_desc"] = "Closing an application";
                translations["event_minimize_name"] = "Minimize App";
                translations["event_minimize_desc"] = "Minimize an app to the taskbar";
                translations["event_unminimize_name"] = "Restore App";
                translations["event_unminimize_desc"] = "Restore an app from the taskbar";
                translations["event_maximize_name"] = "Maximize app";
                translations["event_maximize_desc"] = "Maximize an app to fill the screen";
                translations["event_unmaximize_name"] = "Reduce app";
                translations["event_unmaximize_desc"] = "Un-Maximize an app that took up the whole screen";
                translations["event_menu_name"] = "Menu";
                translations["event_menu_desc"] = "Opening a context menu";
                translations["event_menucommand_name"] = "Menu Click";
                translations["event_menucommand_desc"] = "Selecting an option from a context menu";
                translations["event_select_name"] = "Select";
                translations["event_select_desc"] = "Selecting an item on the desktop or in the file explorer";
                translations["event_loadscheme_name"] = "Load Scheme";
                translations["event_loadscheme_desc"] = "Loading the Sound Scheme";
                translations["system_event_disable_confirm_title"] = "Disable sound event";
                translations["system_event_disable_confirm_text"] = "When a sound event is disabled on this PC, you can still import, edit, export it, but it will not be played on this PC.";
                translations["system_event_disabled_label"] = "Disabled";
                translations["system_event_disabled_desc"] = "Disabled on this PC";
                translations["system_event_disable"] = "Disable on this PC";
                translations["system_event_enable"] = "Enable on this PC";
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
                translations["scheme_view_list"] = "Switch to List view";
                translations["scheme_view_tiles"] = "Switch to Tiles view";
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
                translations["startup_patch_not_elevated_title"] = "Missing Administrator privileges";
                translations["startup_patch_not_elevated_text"] = "Relaunch the application as Administrator in order to patch the startup sound.";
                translations["startup_patch_not_admin"] = "Cannot patch the startup sound: missing Administrator permissions.";
                translations["startup_patch_no_imageres_dll"] = "Cannot patch the startup sound: cannot find imageres.dll.";
                translations["startup_patch_not_possible"] = "Cannot patch the startup sound: operation not supported for this operating system.";
                translations["startup_patch_not_recommended_title"] = "Startup sound patch not recommended";
                translations["startup_patch_not_recommended_text"] = "Patching the startup sound requires admin privileges and might break after a major system update. Proceed?";
                translations["playing_shutdown_sound"] = "Playing Shutdown sound";
                translations["playing_logoff_sound"] = "Playing Logoff sound";
                translations["scheme_file_desc"] = "Sound Scheme File";
                translations["scheme_file_proprietary_desc"] = "Sound Scheme File (Proprietary file format)";
                translations["box_import_system_scheme"] = "Import a system sound scheme";
                translations["box_system_integration"] = "System integration";
                translations["check_box_imageres_patch"] = "Patch the built-in system startup sound";
                translations["check_box_bg_sound_player"] = "Start with Windows to play missing sounds";
                translations["check_box_file_assoc"] = "Associate with sound scheme files";
                translations["check_box_reset_missing_on_load"] = "Leave system sound when a scheme lacks a sound";
                translations["reinstall_confirm_title"] = "Reset the application";
                translations["reinstall_confirm_text"] = "All application settings will be reset and the current sound scheme will be lost.";
                translations["uninstall_confirm_title"] = "Uninstall the application";
                translations["uninstall_confirm_text"] = "System settings will be restored, all application data will be removed and the app will exit.";
                translations["config_file_confirm_title"] = "Advanced Configuration";
                translations["config_file_confirm_text"] = "Only edit the configuration file if you know what you are doing. The app will exit.";
                translations["box_maintenance"] = "Maintenance";
                translations["button_reinstall"] = "Reinstall";
                translations["button_uninstall"] = "Uninstall";
                translations["button_config_file"] = "Edit config file";
                translations["tab_about"] = "About...";
                translations["help_file"] = "Readme-En.txt";
                translations["help_file_not_found_title"] = "Missing help file";
                translations["help_file_not_found_text"] = "The help file is missing. Try visiting the website to grab it.";
                translations["button_help"] = "View Help";
                translations["button_website"] = "Visit my website";
                translations["button_download_schemes"] = "Download more sound schemes";
                translations["box_system_info"] = "System information";
                translations["supported_system_version"] = "This Windows version is supported by the application.";
                translations["unsupported_system_version"] = "The application has not been tested on this Windows version and as such *MIGHT* not perform as expected.";
                translations["drag_drop_no_target_sound_title"] = "No selected sound";
                translations["drag_drop_no_target_sound_text"] = "Please select a target sound before dropping a file.";
                translations["drag_drop_sound_confirm_title"] = "Confirm sound replacement";
                translations["drag_drop_sound_confirm_text"] = "Replace this sound?";
                translations["download_schemes_selection_title"] = "Download Schemes";
                translations["download_schemes_selection_text"] = "Select schemes to download";
                translations["download_schemes_selection_fetching_list"] = "Fetching list...";
                translations["download_schemes_selection_everything"] = "All Sound Schemes";
                translations["download_schemes_no_tls_title"] = "TLS/HTTPS configuration is inadequate";
                translations["download_schemes_no_tls_text"] = "The current HTTPS configuration does not allow downloading. View scheme list in your web browser?";
                translations["download_schemes_failed_title"] = "Download failed";
                translations["download_schemes_failed_text"] = "Failed to download sound schemes. View scheme list in your web browser?";
                translations["auto_import_offer_title"] = "Import this sound scheme?";
                translations["auto_import_offer_text"] = "The system sound scheme has changed, would you like to import it? Any unsaved project will be lost.";
                //Add new translations here
            }
        }
    }
}

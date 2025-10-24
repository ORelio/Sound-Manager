========================================================
==== SoundManager v3.4.1 - By ORelio - Microzoom.fr ====
======= https://github.com/ORelio/Sound-Manager/ =======
========================================================

Thanks for downloading SoundManager!

SoundManager is free software allowing to easily create and share Windows sound schemes.
All Windows versions from Windows XP SP3 to Windows 11 are supported.

Main features are the following:
 - Play missing sounds on Windows 8 and greater
 - Export and import sound schemes using archive files
 - Import sound schemes created with the Sound applet
 - Import proprietary "soundpack" archive files
 - Load and test sound files for each event
 - Define metadata such as thumbnail, author, description
 - Auto-convert sounds to WAV format (Windows 7+)
 - Patch built-in startup sound (Admin required, Windows Vista+)

============
 How to use
============

Setup (if using the standalone version):
 - Extract the archive in a fixed place such as C:\Program Files\SoundManager or Documents\SoundManager
 - Run SoundManager.exe, go to Settings and enable the desired features under System integration
 - Optionally create a Desktop shortcut for SoundManager.exe ;)

Uninstall (if using the standalone version):
 - Run SoundManager.exe, go to Settings and click Uninstall under Maintenance section
 - Delete the SoundManager directory, such as C:\Program Files\SoundManager or Documents\SoundManager
 - Optionally delete the previously created Desktop shortcut

Create a sound scheme
 - Prepare sound files, preferably in WAV format, using an audio editor such as Audacity
 - Load a sound scheme or click Reset to start from the system sound scheme
 - Change sounds using Drag & Drop or right click > Replace
 - Set metadata such as image, name, author and description
 - Click Export when you are satisfied with the result :)

Download sound schemes
 - Get some sound schemes from https://github.com/ORelio/Sound-Manager-Schemes
 - Or alternatively, use the built-in download tool in the About tab of the program
   In that case, the download folder is alongside SoundManager.exe (portable mode) or in Music folder (setup mode)
 - Do not hesitate to submit your own sound schemes to be added into the repository :)

=================================
 Tips for creating sound schemes
=================================

A sound scheme is an integral part of the operating system user experience.
Here are some tips to make your sound scheme both useful and enjoyable:

1. Frequency and Duration

   Choose a duration that is appropriate for the frequency with which a sound event occurs.
   This will help you avoid fatigue caused by playing too often a sound that is too long:

    Frequency  | Sound Examples                        | Recommended Maximum Duration
    -----------+---------------------------------------+------------------------------
    Very Rare  | Startup, Shutdown, USB Error, Battery | 10 seconds
    Rare       | Log On, Log Off, Recycle Bin          | 5 seconds
    Regular    | Information, Error, USB, Admin Access | 1 second
    Frequent   | Default, Launch/Close App             | 300 milliseconds
    Repetitive | Navigate, Menu, Menu Click            | 100 milliseconds

   If in doubt, you can refer to the sound durations inside the default Windows XP or Vista/7 sound scheme.
   Remember to remove silences before and after each sound effect if present in the sound files.

2. Volume level

   Keep a consistent volume between your different files, and with the default Windows sound scheme:
    - The Default event serves as reference: It is played when adjusting the PC volume from notification area.
    - The sound scheme must have a reasonably low volume to be heard without disturbing use of the PC.
    - Some very frequent events such as Navigation may have a lower volume than the others.

3. Sound groups

   Some events can be grouped by similarity.
   Your scheme will seem more consistent during use if sounds of the same group share similarities:
    - PC startup: Startup, Shutdown
    - Session: Log On, Log Off
    - Dialogs: Information, Question, Warning, Error
    - Peripherals: USB Connect, USB Remove, USB Error
    - Programs: Launch, Close, Minimize, Restore, Maximize, Reduce
    - Messaging: Email, Reminder
    - Battery: Low, Critical
    - Drop-down menu: Menu, Menu Click

4. Recycling sounds

   Ideally, you should define sounds from Startup to Admin Access to have a fairly complete scheme.
   But if you create a scheme from existing files, you may be missing some to complete your scheme.
   Rather than leaving empty events, you can try recycling the sounds at your disposal:
    - Cut a portion of a longer event and use it on a shorter sound event
    - Play a sound in reverse, e.g. USB Connect -> USB Remove (reversed), Log On -> Log Off (reversed)
    - As a last resort, copy and paste sounds. Some tips to make this less noticeable:
       - Keep different sounds within the same group, e.g. inside Session Events or Dialogs
       - Copy and paste events of similar type but in different groups:
          - Information -> Print
          - Warning -> Admin Access
          - Error -> Battery Critical
          - Balloon -> Email

===========
 Changelog
===========

 - 1.0   : Initial version for Windows XP SP2 FR, replacing files in C:\Windows\Media
 - 1.1   : Add support for Windows Vista, different file names in C:\Windows\Media
 - 1.1b  : Bugfix on scheme loading
 - 1.1c  : New icon, improve UI font
 - 1.2   : Add a built-in scheme editor
 - 2.0   : Add support for Windows 7, using a dedicated sound scheme in registry
 - 2.1   : Ability to load a .ths file directly by clicking on it
 - 3.0   : C# rewrite, open sourcing, English translation, add support for Windows 8 and 10
 - 3.0.1 : Reduce startup sound delay on Windows 8+, Fix some sounds not playing on Windows 10
 - 3.0.2 : Remove startup sound delay on Windows 10, Add support for Windows 11
 - 3.1.0 : Add more sounds, fix handling of readonly files, add categories on download utility
 - 3.1.1 : Fix startup sound in multi-user context, bug introduced in version 3.0.2
 - 3.2.0 : Add support for loading proprietary "soundpack" archive files
 - 3.2.1 : Program icon overhaul, fix crash when launching from a \\network\share
 - 3.3.0 : Rework startup sound patching, adding support for Windows 8, 10 and 11
 - 3.3.1 : Add quick access to advanced config file, improve compatibility with screen readers
 - 3.4.0 : Add ability to disable some sounds on your PC, fix Email on Windows 8+, add Reminder
 - 3.4.1 : Improve startup/logon sound behavior. Add Load Scheme. Offer to import system scheme

=====
 FAQ
=====

Q: When patching the built-in system startup sound, it does not properly update?
A: System files may be in use, try rebooting and reapplying the sound scheme.
A: A major system update may also revert or break the patch, try disabling and enabling the setting.

Q: There are some sound events I don't like. How to mute them?
R: Right click on undesired sound event > Disable on this PC

Q: When using the Windows XP sound scheme, the startup sound should also play on logon. How to do this?
R: Settings > Edit config file > set "PreferStartupSoundOnLogon=True" and save
R: If you do not see the setting, you can add it below the others

Q: Is there any source code for versions 1.x and 2.x?
A: No, these versions were created using Game Maker and a bunch of batch files.

=========
 Credits
=========

The SoundManager program has been created using the following resources:

 - Privilege20 library from MSDN magazine, March 2005
 - Tri-State Tree View library, from CodeProject no. 202435
 - Teko font by Manushi Parikh (Logo)
 - Dancing Script font by Pablo Impallari (Logo)
 - Download icon, by Microsoft Corporation
 - Clipping Sound Icon by RAD.E8

+--------------------+
| © 2009-2025 ORelio |
+--------------------+

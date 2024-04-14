========================================================
==== SoundManager v3.2.1 - By ORelio - Microzoom.fr ====
========================================================

Thanks for downloading SoundManager!

SoundManager is a free software allowing to easily create and share Windows sound schemes.
All Windows versions from Windows XP SP3 to Windows 11 are supported.

Main features are the following:
 - Load and test sound files for each event
 - Define metadata such as thumbnail, author, description
 - Export and import sound schemes using archive files
 - Import sound schemes created with the Sound applet
 - Auto-convert sounds to WAV format (Windows 7+)
 - Patch Windows Vista/7 startup sound (Admin required)
 - Play startup/shutdown sounds on Windows 8 and greater
 - Load proprietary "soundpack" archive files

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
 - It is recommended to have a consistent loudness between all your audio files
 - Load a sound scheme or click Reset to start from the system sound scheme
 - Change sounds using Drag & Drop or right click > Replace
 - Set metadata such as image, name, author and description
 - Click Export when you are satisfied with the result :)

Download sound schemes
 - Get some sound schemes from https://github.com/ORelio/Sound-Manager-Schemes
 - Or alternatively, use the built-in download tool in the About tab of the program
   In that case, the download folder is alongside SoundManager.exe (portable mode) or in Music folder (setup mode)
 - Do not hesitate to submit your own sound schemes to be added into the repository :)

===========
 Changelog
===========

 - 1.0   : Initial version for Windows XP SP2 FR, replacing files in C:\Windows\Media
 - 1.1   : Add support for Windows Vista, different file names in C:\Windows\Media
 - 1.1b  : Bugfix on theme loading
 - 1.1c  : New icon, improve UI font
 - 1.2   : Add a built-in scheme editor
 - 2.0   : Add support for Windows 7, using a dedicated sound scheme in registry
 - 2.1   : Ability to load a .ths file directly by clicking on it
 - 3.0   : C# rewrite, open sourcing, English translation, add support for Windows 8 and 10
 - 3.0.1 : Reduce startup sound delay on Windows 8+, Fix some sounds not playing on Windows 10
 - 3.0.2 : Remove startup sound delay on Windows 8+, Add support for Windows 11
 - 3.1.0 : Add more sounds, fix handling of readonly files, add categories on download utility
 - 3.1.1 : Fix startup sound in multi-user context, bug introduced in version 3.0.2
 - 3.2.0 : Add support for loading proprietary "soundpack" archive files
 - 3.2.1 : Program icon overhaul, fix crash when launching from a \\network\share

=====
 FAQ
=====

Q: On Windows 7, the startup sound is not always properly updated?
A: DLL files may be in use, you may need to reboot and reapply the sound scheme.

Q: Is there any source code for versions 1.x and 2.x?
A: No, these versions were created using Game Maker and a bunch of batch files.

=========
 Credits
=========

The SoundManager program has been created using the following resources:

 - Privilege20 library from MSDN magazine, March 2005
 - Tri-State Tree View library, from CodeProject no. 202435
 - Teko & Dancing Script Fonts from Google Fonts (Logo)
 - Download icon, by Microsoft Corporation
 - Clipping Sound Icon by RAD.E8
 - Resource Hacker by Angus Johnson

+--------------------+
| © 2009-2024 ORelio |
+--------------------+

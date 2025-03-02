![SoundManager](Images/logo-en.png)

SoundManager is free software that makes it easy to create and share Windows sound schemes. All Windows versions from Windows XP SP3 to Windows 11 are supported. Requires [.NET 4.0](http://www.microsoft.com/en-us/download/details.aspx?id=17718) or greater.

* 💾 **Download:** Have a look at the [releases section](https://github.com/ORelio/Sound-Manager/releases) to get a build
* 📁 **Sound schemes:** Check out the [sound schemes repository](https://github.com/ORelio/Sound-Manager-Schemes)

## Overview

SoundManager allows editing the current sound scheme, as well as defining metadata:

![SoundManager main UI](Images/gui-scheme-en.png)

Main features are the following:

* Play missing sounds on Windows 8 and greater
* Export and import sound schemes using archive files
* Import sound schemes created with the [Sound applet](https://www.thewindowsclub.com/change-sounds-in-windows)
* Import proprietary "soundpack" archive files
* Load and test sound files for each event
* Define metadata such as thumbnail, author, description
* Auto-convert sounds to WAV format (Windows 7+)
* Patch built-in startup sound (Admin required, Windows Vista+)

## User Manual

See [User Manual](UserManual/Readme-En.txt) for more details on how to use the program.

## How it works

### The `SoundManager` sound scheme

SoundManager integrates into the system using the built-in sound scheme feature in Registry:
````
HKEY_CURRENT_USER\AppEvents\Schemes
````
The `SoundManager` scheme is automatically created on first launch, pointing to:
````
C:\Users\%USERNAME%\AppData\Roaming\SoundManager\Media\
````
Sound files such as `Startup.wav`, `Shutdown.wav` and so on are placed here. Since they are automatically played by the system, the SoundManager app is not required to run once the sound scheme has been set, except if you want to restore the missing sounds on Windows 8+ (see below).

SoundManager handles registry differences between Windows versions, such as the balloon sound which [does not play by default](https://winaero.com/blog/fix-windows-plays-no-sound-for-tray-balloon-tips-notifications/) on Windows 7/8 and changes again on Windows 10.

### Sound Archives

Sound archive files are simply Zip files having the `.ths` file extension:

````
SoundScheme.ths
 |- Scheme.ini
 |- Scheme.png
 |- Startup.wav
 |- Shutdown.wav
 \- <OtherSounds>.wav
````

SoundManager can associate itself with this file type to conveniently load sound schemes, and you can manually edit them using any file archive utility such as [7-Zip](https://www.7-zip.org/) or by renaming them to `.zip` while displaying [file extensions](https://www.thewindowsclub.com/show-file-extensions-in-windows), then using the built-in Windows utility.

### Windows Vista+ startup sound

On Windows Vista and greater, the startup sound is no longer customizable by the user, the corresponding WAV file being embedded in `C:\Windows\System32\imageres.dll` for [performance reasons](https://blogs.msdn.microsoft.com/e7/2009/02/18/engineering-the-windows-7-boot-animation/).

SoundManager can optionally [patch imageres.dll](https://www.sevenforums.com/tutorials/63398-startup-sound-change-windows-7-a.html) to update the startup sound:

* Ownership of `imageres.dll` is [transferred](https://helpdeskgeek.com/windows-7/windows-7-how-to-delete-files-protected-by-trustedinstaller/) from `TrustedInstaller` to `Administrators`
* If not already done, `imageres.dll` is backed up to `imageres.dll.bak`
* Existing `imageres.dll` is moved to `imageres.dll.old` since it is in use by the system
* `imageres.dll.bak` is copied to `imageres.dll` and its `WAV` resourse is updated

This feature requires administrator privileges. If enabled, SoundManager will show an [UAC](https://en.wikipedia.org/wiki/User_Account_Control) prompt on launch. Due to `imageres.dll` files being used by the system, SoundManager might not be able to patch the startup sound more than once between each system reboot. Also, major system updates might revert the startup sound to its original state and/or break the patch mechanism.

Initially implemented using [Resource Hacker](https://www.angusj.com/resourcehacker/), SoundManager now patches the DLL directly using the Windows API ([BeginUpdateResource](http://msdn.microsoft.com/en-us/library/windows/desktop/ms648030%28v=vs.85%29.aspx), [UpdateResource](http://msdn.microsoft.com/en-us/library/windows/desktop/ms648049%28v=vs.85%29.aspx), [EndUpdateResource](http://msdn.microsoft.com/en-us/library/windows/desktop/ms648032%28v=vs.85%29.aspx)) to replace the startup sound. This allows seamless patching on newer system versions that implement a [distinct resource file for the DLL](https://answers.microsoft.com/en-us/windows/forum/all/workaround-for-changing-the-windows-1011-startup/b15dd438-42c7-471c-bc86-2e5fb0fa4037) `imageres.dll.mun`.

### Windows 8+ shutdown, login, logoff sounds

On Windows 8, the shutdown sound was removed for further [performance reasons](https://winaero.com/blog/how-to-play-the-logon-or-startup-sound-in-windows-8-1-or-windows-8/), as well as the logon and logoff sounds. SoundManager can emulate the playback of these sounds by launching a background process on logon:

* Process spawns an invisible window, mandatory for delaying system shutdown
* Process plays Startup or Logon sound and goes inactive
* On logoff, process wakes up and [sets up a ShutdownBlockReason](https://devblogs.microsoft.com/oldnewthing/20120614-00/?p=7373)
* Process determines if the Logoff or Shutdown sound should be played
* Sound is played, then ShutdownBlockReason is removed and the process exits

This is typically how `explorer.exe` was handling the thing on Windows 7, but you'll get yet another process sleeping in background, separate from `explorer.exe`. This feature can be disabled entierely in the SoundManager settings.

SoundManager also allows patching the startup sound on Windows 8 and greater. When used in combination with the background sound player process, the system itself will play the native startup sound, and the background process from SoundManager will play the other sounds. This helps reducing latency in startup sound playback since the system will play the startup sound with high priority.

## Build instructions

In order to maintain support for Windows XP SP3, SoundManager targets .NET Framework v4.0 and builds under Visual Studio 2010. If you want to build without support for Windows XP, you should be able to build by importing the project into the latest version of [Visual Studio Community](https://visualstudio.microsoft.com/vs/community/). The following instructions detail how to build with XP support.

For proper support of newer operating systems such as Windows 10, SoundManager needs APIs such as [Task Scheduler 2.0](https://learn.microsoft.com/en-us/windows/win32/taskschd/task-scheduler-2-0-interfaces) and [ShutdownBlockReason](https://devblogs.microsoft.com/oldnewthing/20120614-00/?p=7373), which aren't present on XP, so building under Windows XP will not work. Building was tested successfully under Windows Vista, 7, 8, 10 and 11 using Visual Studio 2010.

### Setting up Visual Studio

1. Download [Visual Studio 2010 Express](https://archive.org/details/vs-2010-express-1). This is the free version of Visual Studio 2010 that includes the bare minimum components needed to get started using Visual Studio.
2. Mount or extract the downloaded ISO file: `VS2010Express1.iso`
  * On Windows 8 or greater, opening the ISO file in file Explorer will automatically mount it as a virtual DVD drive.
  * On earlier Windows versions, software such as [WinCDEmu](https://wincdemu.sysprogs.org/) or [Daemon Tools](https://www.daemon-tools.cc/products/dtLite) can mount the ISO file.
  * Alternatively, you can use a file archive utility such as [7-Zip](https://7-zip.org/) to extract the ISO file instead of mounting it.
4. Open the `VCSExpress` folder inside the virtual DVD drive or extracted ISO file and launch `setup.exe`
5. Follow the on-screen instructions to install Visual C# 2010.

### Compiling

This section assumes you already have the `Sound-Manager` git repository cloned or [manually downloaded](https://github.com/seediffusion/Sound-Manager/archive/refs/heads/master.zip). In the following instructions, "project folder" refers to the main folder of the `Sound-Manager` repository, where `README.md` and `SoundManager.sln` are housed.

1. Navigate to the project folder and open `SoundManager.sln file`
  * If your system isn't configured to [show file extensions](https://www.thewindowsclub.com/show-file-extensions-in-windows), you won't see the `.sln` part of the filename.
  * If VS 2010 doesn't open automatically, select Visual C# 2010 in the "open with" dialog.
2. Once the project is open, set the build target to `Release` instead of `Debug` in the dropdown menu next to the Build button. You can leave `Debug` while making changes in the code and using the debugger in Visual Studio.
3. Hit `Shift+Control+B` to build the entire `SoundManager` solution. This should only take a few seconds to compile.
4. Assuming there were no errors during compilation, hit Alt + F4 to close VS 2010.
5. If everything worked properly, you should see:
  * `SoundManager.exe` in `<ProjectFolder>\SoundManager\bin\Release`
  * `DownloadSchemes.exe` in `<ProjectFolder>\DownloadSchemes\bin\Release`.
6. Copy the following items into `<ProjectFolder>\SoundManager\bin\Release`.
  * `<ProjectFolder>\SoundManager\Lang`
  * `<ProjectFolder>\UserManual\Readme-En.txt`
  * `<ProjectFolder>\UserManual\Readme-Fr.txt`
  * `<ProjectFolder>\DownloadSchemes\bin\Release\DownloadSchemes.exe`
7. Finally, check that everything's working by launching `<ProjectFolder> \SoundManager\bin\Release\SoundManager.exe`.

## License

SoundManager is provided under
[CDDL-1.0](http://opensource.org/licenses/CDDL-1.0)
([Why?](http://qstuff.blogspot.fr/2007/04/why-cddl.html)).

Basically, you can use it or its source for any project, free or commercial, but if you improve it or fix issues,
the license requires you to contribute back by submitting a pull request with your improved version of the code.
Also, credit must be given to the original project, and license notices may not be removed from the code.

# KAWAII Theme Switcher
A small tool for Windows Theme enthusiast.

Use this app to switch between Windows themes with just One click! Plus, without showing any dialog! Yup. 
KTS (in shorts), will scan your /Resources/Themes folder for .theme files and save each file's path for later.

## Main Features
* Small, kawaii file size.
* No services, background process etc.
* Play theme's "theme change" sound (if any) until the very ends, before exiting.
* Ability to change User Logon background (tested on Windows 7).
* Command Prompt support with arguments.

## System Requirements
 - [.NET 4.0 or Above](https://www.microsoft.com/en-au/download/details.aspx?id=17851)
 
## Installation
NO NEED for installation! Since this guy doesn't have any GUI, just put the EXE somewhere nice, Thats all!

## Extra Features
Well, in order to enable some features, you need to do some stuff, but don't worry, i will keep it simple and clean!

### Launch on every Startup
- Inside the folder where you put the EXE, create a text file named "startup.txt". 
- Open it, write "start: 10000", make sure you wrote it on the FIRST LINE.
- Save and close.
- Lastly, launch the EXE, it will automatically create a shortcut on your startup folder.
* What about the "Delay"?? That's a delay before changing your theme when it launch on startup, in Milliseconds.
So, 10000 means 10 seconds delay before changing theme. Yes, you may modify this. You can set start delay with negative values.
Negative values means it will wait until current CPU Usage is 20% or lower for X seconds. Remeber that negative values are count as SECONDS, putting -1000 means it will hold the proccess until current CPU Usage is 20% or lower for 1000 SECONDS!
* You can also set the "Exit Delay" by writing "exit: [delay]" on the second line. This will hold application before actually closing.
This is useful if you heard the "theme change" sound trimmed. Exit delay doesn't support negative values.

### Theme Selection Mode
Normally, KTS use RANDOM mode for theme selection. That means, on each launch, it will pick a theme RANDOMLY from your themes folder.
Actually, there are 2 more modes for this. 
#### SEQUENTIAL mode.
Just like the name, it will pick themes SEQUENTIALLY, from A to Z. Unlike RANDOM mode, SEQUENTIAL mode will make sure all themes are used.
#### RANDOM SEQUENTIAL mode
This mode is a combination of the last 2 modes. All of your themes will be shuffled and then picked sequentially on each run.

#### How To
- Inside the folder where you put the EXE, create an empty text file.
- To enable SEQUENTIAL mode: name it "Sequence.txt", for RANDOM SEQUENTIAL: name it "RSequence.txt".
- In the future, to enable RANDOM mode, delete the file.

### Exclusion
In case you have "THOSE" themes that you dont want to show to someone else, you can add them to Exclusion.

#### How To
- Inside the folder where you put the EXE, create a text file named "exclusion.txt".
- Open it, and put some theme's name there, without extension, just the name, one name per line.
- All themes inside this file will be excluded from selection (all modes).

### Logon Background Switching
3rd-party themes usually comes with it's own Logon background. KAWAII Theme Switcher support changing Logon background with `any .jpg file with maximum size of 256KB`.

#### How To
- Inside the folder where you put the EXE, create a text file named "logon.txt".
- Open it, write "mode: [mode; see below.]"
- Save and Close.

#### Logon Selection Mode (Without Quotes!)
- `respective` - Default mode. Pick Logon background based on current theme if it have it's own Logon background. If not, do nothing.
- `sequence` - Get all .jpg files inside %windir%/Resources/Logon and it's subdirectories, sort the sequence (ascending), pick one from the sequence on every switch.
- `random` - Pick a random .jpg file inside %windir%/Resources/Logon and it's subdirectories.
- `random sequence` - Get all .jpg files inside %windir%/Resources/Logon and it's subdirectories, shuffle the sequence, pick one from the sequence on every switch.

### Launch from Command Prompt
Yes, you can launch KAWAII Theme Switcher via Command Prompt!

#### How To
```xaml
KAWAII Theme Switcher.exe [first argument. See below.] [second argument, see below. optional.]
```

#### First Arguments
First arguments are for Theme Selection mode. Case-insensitive.
- Path to .theme file (with quotes if there are spaces):
```xaml
> KAWAII Theme Switcher.exe C:\Windows\Resources\Themes\aero.theme
> KAWAII Theme Switcher.exe "C:\Windows\Resources\Themes\Vocaloid Rin V2 by andrea_37.theme"
```
- Theme's name (with quotes if there are spaces):
```xaml
> KAWAII Theme Switcher.exe aero
> KAWAII Theme Switcher.exe "Vocaloid Rin V2 by andrea_37"
```
- Random:
```xaml
> KAWAII Theme Switcher.exe random
```

#### Second Arguments
Optional. Second arguments are for Logon Selection mode.
- Path to .jpg file (with quotes if there are spaces):
```xaml
> KAWAII Theme Switcher.exe [first argument] C:\Users\Kuchienkz\Pictures\kagamineRin.jpg
> KAWAII Theme Switcher.exe [first argument] "C:\Users\Kuchienkz\Pictures\Hatsune Miku.jpg"
```
- Random:
```xaml
> KAWAII Theme Switcher.exe [first argument] Random
```
- Respective (only works if first argument is not empty):
```xaml
> KAWAII Theme Switcher.exe [first argument] ""
```

## Additional Information
- Tested on Windows 7 Ultimate 64bit with 3rd-party theme support.

# KAWAII Theme Switcher
A small tool for Windows Theme enthusiast. Now support Windows 10 !!!

Use this app to switch between Windows themes with just One click! 
KTS (in shorts), will scan your /Resources folder for .theme files and save each file's path for later.

## Main Features
* Small, kawaii file size.
* No services, background process etc.
* UI-less, just launch and done. Startup configurable.
* Play theme's "theme change" sound (if any) until the very ends, no more trimmed sounds.
* Ability to change Lock Screen background! Automatically! Configurable.
* Command Prompt support with arguments. Unlimited possibilities.

## Requirements
 - Windows 7 or newer as Operating System
 - [.NET 4.6 or Above](https://www.microsoft.com/en-us/download/details.aspx?id=48130)
 - 3rd-party theme support (optional)
 - StartIsBack (for Windows 10 user, optional)
 
## Installation
NO NEED for installation! Since this guy doesn't have any GUI, just put the EXE somewhere nice, Thats all!

## Extra Features
Well, in order to enable some features, you need to do some stuff, but don't worry, i will keep it simple and clean!

### Launch on every Startup
- Inside the folder where you put the EXE, create a text file named "startup.txt". 
- Open it, write "start: 10000", make sure you wrote it on the FIRST LINE.
- Save and close.
- Lastly, launch the EXE, it will automatically create a registry for startup purpose.
* What about the "start"?? That's a delay before changing your theme when it launch on startup, in Milliseconds.
So, '10000' means '10 seconds' delay before changing theme. Yes, you may modify this. You can set start delay with negative values.
Negative values means it will wait until current CPU Usage is 20% or lower for X seconds. Remember that negative values are treated as SECONDS, putting -1000 means it will hold the proccess until current CPU Usage is 20% or lower for 1000 SECONDS!
* You can also set the "Exit Delay" by writing "exit: [delay]" on the second line. This will hold application before actually closing.
This is useful if you heard the "theme change" sound trimmed. Exit delays are in milliseconds and DOES NOT support negative values.

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

### Skip
You can skip next switching (once) by creating 'skip.txt' file inside the folder where you put the EXE. Once skipped, the 'skip.txt' file will be deleted. OR, you can create a 'repeating skip' by creating 'skip.txt' and add these lines:
```xaml
3/3
repeat
```
The number '3' above can be any number. The first '3' is the 'remaining value', the second one is the 'number of repeat'. On 'skip.txt' creation, you should set 'remaining value' with the same value with the 'number of repeat' value.
'Repeating skip' is useful if you want to switch theme every X startup, instead of every startup.

### Blacklisting
In case you have "THOSE" themes that you dont want to show to someone else, you can add them to Blacklist.

#### How To
- Inside the folder where you put the EXE, create a text file named "blacklist.txt".
- Open it, and put some theme's name there, without extension, just the name, one name per line.
- All themes inside this file will be excluded from selection (all modes).

### Whitelisting
The opposite of Blacklist, KTS will apply theme only if it exists in Whitelist. If both Blacklist and Whitelist exists, `Blacklist will be ignored`.

#### How To
- Inside the folder where you put the EXE, create a text file named "whitelist.txt".
- Open it, and put some theme's name there, without extension, just the name, one name per line.
- Only themes inside this file will be included to selection (all modes).

### Lockscreen Background Switching
3rd-party themes usually comes with it's own Lockscreen background. KAWAII Theme Switcher support changing Lockscreen background with `any .jpg file with maximum size of 256KB` for Windows 7. There is no file size limit for later version Windows.

#### How To
- Inside the folder where you put the EXE, create a text file named "lockscreen.txt".
- Open it, write "mode: [mode; see below.]"
- Save and Close.

#### Lockscreen Selection Mode (Without Quotes!)
- `respective` - Default mode. Pick Lockscreen background based on current theme, if it does have it's own Lockscreen background. If not, do nothing.
- `sequence` - Get all .jpg files inside Lockscreen directory and it's subdirectories, sort the sequence (ascending), pick one from the sequence on every switch.
- `random` - Pick a random .jpg file inside Lockscreen directory and it's subdirectories.
- `random sequence` - Get all .jpg files inside Lockscreen directory and it's subdirectories, shuffle the sequence, pick one from the sequence on every switch.

For `any mode beside respective mode`, KTS search for .jpg files on ALL of the following locations, including their subdirectories:
- ~folder_containing_the_exe\Lockscreen
- C:\Windows\Resources\Logon
- C:\Windows\Resources\Lockscreen

ALL .jpg files in these folder will be treated as Lockscreen background, regardless of their names. If neither of those folders exists, a folder named `Lockscreen` will be created on the same location as the KTS executable file, which then you can fill with some .jpg files later. You may create another of those folders manually.

For `respective mode`, KTS will search `a .jpg file inside every theme's folder named lockscreen.jpg or logon.jpg`. This doesn't include their subdirectories. However, if none .jpg found, KTS will search for .jpg file inside ALL the mentioned locations above, which has the same name as the related theme.

### Tips & Trick
#### Disabling Theme Switching Function
In case you don't want to use theme switching function, but still want to switch Lockscreen bacground, you can create an empty whitelist.txt file. Please note that since theme switching is skipped, `respective` mode can't be used for Lockscreen background switching.

### Launch from Command Prompt
- Yes, you can launch KAWAII Theme Switcher via Command Prompt!
- BUT! You can't use all 'extra features' mentioned above, since all .txt files will be ignored.

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
Optional. Second arguments are for Lockscreen Selection mode.
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
- Also Tested on Windows 10 version 1909 64bit with 3rd-party theme support and StartIsBack installed.

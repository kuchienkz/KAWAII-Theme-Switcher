# KAWAII Theme Switcher
A small tool for Windows Theme enthusiast.

Use this app to switch between Windows themes with just One click! Plus, without showing any dialog! Yup. 
KTS (in shorts), will scan your /Resources/Themes folder for .theme files and save each file's path for later.

# Main Features
- Small, kawaii file size.
- No services, background process etc.
- Play theme's "theme change" sound (if any) until the very ends, before exiting.

# Installation
NO NEED for installation! Since this guy doesn't have any GUI, just put the EXE somewhere nice, Thats all!

# Extra Features
Well, in order to enable some features, you need to do some stuff, but don't worry, i will keep it simple and clean!

- To launch this tool on every Startup:
Inside the folder where you put the EXE, create a text file named "startup.txt". 
Open it, write "delay: 10000", make sure you wrote it on the first line. 
save and close. Lastly, launch the EXE, it will automatically create a shortcut on your startup folder.
What about the "Delay"? Thats a delay before changing your theme when it launch on startup, in Milliseconds.
So, 10000 means 10 seconds delay before changing theme. Yes, you may modify this.

- Theme Selection Mode:
Normally, KTS use RANDOM mode for theme selection. That means, on each launch, it will pick a theme RANDOMLY from your themes folder.
Actually, there are 2 more modes for this. The first is SEQUENTIAL mode. Just like the name, it will pick themes SEQUENTIALLY, from A to Z. Unlike RANDOM mode, SEQUENTIAL mode will make sure all themes are used.
The second is RANDOM SEQUENTIAL. This mode is a combination of the last 2 modes. All of your themes will be shuffled 
and then picked sequentially on each run.
Inside the folder where you put the EXE,
To change selection mode: Create an empty text file.
To enable SEQUENTIAL mode: name it "Sequence.txt". For RANDOM SEQUENTIAL: name it "RSequence.txt". Thats all.

- Exclusion: In case you have "THOSE" themes that you dont want to show to someone else, you can add them to Exclusion.
Inside the folder where you put the EXE, create a text file named "exclusion.txt". Now open it, and put some theme's name there,
without extension, just the name, one name per line.
All themes inside this file will be excluded from selection (all modes).

# Additional Information
- Tested on Windows 7 Ultimate 64bit with 3rd-party theme support.

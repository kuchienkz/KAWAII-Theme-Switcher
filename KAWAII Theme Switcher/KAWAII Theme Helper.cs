﻿/*
 * This file is absolutely FREE, can freely be copied, modified, compiled, or decompiled.
 * Also, you may print it and eat it with pizza, burn it in bizarre rituals,
 * or put it in the middle of Transmutation circle and do some shit, 
 * 
 * or whatever.
 * 
 * I don't give a fuck, as long as you leave my name and email AS IT IS when distributing it.
 * 
 * ALSO, KEEP IT FREE!!!
 * 
 * Originally created by Kuchienkz.
 * Email: wahyu.darkflame@gmail.com
 * 
 * 
 * Other Contributors:
 * [your name here]
 * 
 */

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static KAWAII_Theme_Switcher.Program;

namespace KAWAII_Theme_Switcher
{
    public static class KAWAII_Theme_Helper
    {
        public static readonly string windir = Environment.GetEnvironmentVariable("windir");

        [DllImport("UxTheme.Dll", EntryPoint = "#65", CharSet = CharSet.Unicode)]
        private static extern int SetSystemVisualStyle(string pszFilename, string pszColor, string pszSize, int dwReserved);

        [DllImport("winmm.dll")]
        private static extern uint mciSendString(string command, StringBuilder returnValue, int returnLength, IntPtr winHandle);

        private static int GetSoundLength(string fileName)
        {
            StringBuilder lengthBuf = new StringBuilder(32);

            mciSendString(string.Format("open \"{0}\" type waveaudio alias wave", fileName), null, 0, IntPtr.Zero);
            mciSendString("status wave length", lengthBuf, lengthBuf.Capacity, IntPtr.Zero);
            mciSendString("close wave", null, 0, IntPtr.Zero);

            int length = 0;
            int.TryParse(lengthBuf.ToString(), out length);

            return length;
        }

        [ComImport, Guid("D23CC733-5522-406D-8DFB-B3CF5EF52A71"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ITheme
        {
            [DispId(0x60010000)]
            string DisplayName
            {
                [return: MarshalAs(UnmanagedType.BStr)]
                [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
                get;
            }

            [DispId(0x60010001)]
            string VisualStyle
            {
                [return: MarshalAs(UnmanagedType.BStr)]
                [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
                get;
            }
        }

        [ComImport, Guid("0646EBBE-C1B7-4045-8FD0-FFD65D3FC792"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IThemeManager
        {
            [DispId(0x60010000)]
            ITheme CurrentTheme
            {
                [return: MarshalAs(UnmanagedType.Interface)]
                [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
                get;
            }

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void ApplyTheme([In, MarshalAs(UnmanagedType.BStr)] string bstrThemePath);
        }

        [ComImport, Guid("A2C56C2A-E63A-433E-9953-92E94F0122EA"), CoClass(typeof(ThemeManagerClass))]
        private interface ThemeManager : IThemeManager { }

        [ComImport, Guid("C04B329E-5823-4415-9C93-BA44688947B0"), ClassInterface(ClassInterfaceType.None), TypeLibType(TypeLibTypeFlags.FCanCreate)]
        private class ThemeManagerClass : IThemeManager, ThemeManager
        {
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            public virtual extern void ApplyTheme([In, MarshalAs(UnmanagedType.BStr)] string bstrThemePath);

            [DispId(0x60010000)]
            public virtual extern ITheme CurrentTheme
            {
                [return: MarshalAs(UnmanagedType.Interface)]
                [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
                get;
            }
        }

        private static class NativeMethods
        {
            [DllImport("UxTheme.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool IsThemeActive();
        }

        private static IThemeManager themeManager = new ThemeManagerClass();

        [PermissionSet(SecurityAction.LinkDemand)]
        public static string GetCurrentThemeName()
        {
            return themeManager.CurrentTheme.DisplayName;
        }

        public static void ApplyTheme(string themePath, int exitDelay = 1500)
        {
            string msstylePath = "";
            string color = "NormalColor";
            string size = "NormalSize";
            int TCSound = 0;
            using (var file = new StreamReader(themePath))
            {
                bool enterVS = false, enterTCSound = false;
                string line = "";
                while ((line = file.ReadLine()) != null)
                {
                    if (enterVS == false && !line.Equals("[VisualStyles]"))
                    {
                        continue;
                    }
                    if (enterVS == false)
                    {
                        enterVS = true;
                        continue;
                    }

                    var l = line.RegexReplace(@"\w+ ?= ?", "", 1);
                    if (line.Contains("path", StringComparison.OrdinalIgnoreCase))
                    {
                        msstylePath = l.RegexReplace(@"%systemroot%", @"C:\Windows", 1);
                        continue;
                    }
                    if (line.Contains("colorstyle", StringComparison.OrdinalIgnoreCase))
                    {
                        color = l;
                        continue;
                    }
                    if (line.Contains("size", StringComparison.OrdinalIgnoreCase))
                    {
                        size = l;
                        continue;
                    }
                    if (line.Equals(@"[AppEvents\Schemes\Apps\.Default\ChangeTheme]") || enterTCSound == true)
                    {
                        if (enterTCSound == false)
                        {
                            enterTCSound = true;
                            continue;
                        }
                        TCSound = GetSoundLength(l.RegexReplace(@"%systemroot%", @"C:\Windows", 1));
                        log.Add("_Theme Change Sound found! Duration: " + TCSound + " milliseconds");
                        break;
                    }
                }
            }
            SetSystemVisualStyle(msstylePath, color, size, 0);
            ChangeTheme(themePath);
            TCSound = TCSound < 1500 ? 1500 : TCSound;
            log.Add("_Waiting for sound to finish: " + (TCSound + exitDelay) + " milliseconds");
            Task.Run(() => Thread.Sleep(TCSound + exitDelay)).Wait();
        }

        [PermissionSet(SecurityAction.LinkDemand)]
        private static void ChangeTheme(string themeFilePath)
        {
            themeManager.ApplyTheme(themeFilePath);
        }

        [PermissionSet(SecurityAction.LinkDemand)]
        public static string GetCurrentVisualStyleName()
        {
            return Path.GetFileName(themeManager.CurrentTheme.VisualStyle);
        }

        public static string GetThemeStatus()
        {
            return NativeMethods.IsThemeActive() ? "running" : "stopped";
        }

        // Logon Stuff
        public static void ChangeLockscreenBackground(string jpegFilename, WindowsVersion version, List<string> log)
        {
            if (version == WindowsVersion.UNSUPPORTED)
            {
                var i = "Windows version currently not supported for changing lock screen background";
                Console.WriteLine(i);               
                log.Add(i);

                return;
            }

            if (!File.Exists(jpegFilename))
            {
                var i = "Can't change lock screen background! Image file doesnt exists: " + jpegFilename;
                Console.WriteLine(i);
                log.Add(i);

                return;
            }

            if (version == WindowsVersion.WIN11 || version == WindowsVersion.WIN10 || version == WindowsVersion.WIN8)
            {
                var regChk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\Personalization");
                if (regChk.GetValue("NoChangingLockScreen") == null || (int)regChk.GetValue("NoChangingLockScreen") == 0)
                {
                    Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\Personalization").SetValue("NoChangingLockScreen", 1);
                    log.Add("Locksreen slideshow disabled");
                }

                var command = @"Start-Process -filePath ""$env:systemroot\system32\takeown.exe"" -ArgumentList "" /F `""$env:programdata\Microsoft\Windows\SystemData`"" /R /A /D Y"" -NoNewWindow -Wait
Start-Process -filePath ""$env:systemroot\system32\icacls.exe"" -ArgumentList ""`""$env:programdata\Microsoft\Windows\SystemData`"" /grant Administrators:(OI)(CI)F /T"" -NoNewWindow -Wait
Start-Process -filePath ""$env:systemroot\system32\icacls.exe"" -ArgumentList ""`""$env:programdata\Microsoft\Windows\SystemData\S-1-5-18\ReadOnly`"" /reset /T"" -NoNewWindow -Wait
Remove-Item -Path ""$env:programdata\Microsoft\Windows\SystemData\S-1-5-18\ReadOnly\LockScreen_Z\*"" -Force
Start-Process -filePath ""$env:systemroot\system32\takeown.exe"" -ArgumentList ""/F `""$env:systemroot\Web\Screen`"" /R /A /D Y"" -NoNewWindow -Wait
Start-Process -filePath ""$env:systemroot\system32\icacls.exe"" -ArgumentList ""`""$env:systemroot\Web\Screen`"" /grant Administrators:(OI)(CI)F /T"" -NoNewWindow -Wait
Start-Process -filePath ""$env:systemroot\system32\icacls.exe"" -ArgumentList ""`""$env:systemroot\Web\Screen`"" /reset /T"" -NoNewWindow -Wait
if(!(Test-Path ""$env:systemroot\Web\Screen\img100.jpg.bak""))
{
    Copy-Item -Path ""$env:systemroot\Web\Screen\img100.jpg"" -Destination ""$env:systemroot\Web\Screen\img100.jpg.bak"" -Force
}
if(Test-Path ""$env:systemroot\Web\Screen\img100.jpg"")
{
    Remove-Item -Path ""$env:systemroot\Web\Screen\img100.jpg"" -Force
}
Copy-Item -Path """ + jpegFilename + @""" -Destination ""$env:systemroot\Web\Screen\img100.jpg"" -Force";

                var psCommandBytes = System.Text.Encoding.Unicode.GetBytes(command);
                var psCommandBase64 = Convert.ToBase64String(psCommandBytes);

                var startInfo = new ProcessStartInfo()
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy unrestricted -EncodedCommand {psCommandBase64}",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                };
                var p = new Process();
                p.StartInfo = startInfo;
                p.Start();
                while (!p.StandardOutput.EndOfStream)
                {
                    string line = p.StandardOutput.ReadLine();
                    Console.WriteLine(line);
                    log.Add(line);
                }
            }
            else if (version == WindowsVersion.WIN7)
            {
                if (new FileInfo(jpegFilename).Length > 256000)
                {
                    var i = "Cant change logon background! Image file must be JPG with size no more than 256 KB.";
                    Console.WriteLine(i);
                    log.Add(i);
                    return;
                }

                var regChk = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Authentication\\LogonUI\\Background");
                if (regChk.GetValue("OEMBackground") == null || (int)regChk.GetValue("OEMBackground") == 0)
                {
                    Registry.LocalMachine.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Authentication\\LogonUI\\Background").SetValue("OEMBackground", 1);
                }

                if (!Directory.Exists(windir + "\\System32\\oobe\\Info\\Backgrounds"))
                {
                    Directory.CreateDirectory(windir + "\\System32\\oobe\\Info\\Backgrounds");
                }
                if (File.Exists(windir + "\\System32\\oobe\\info\\backgrounds\\backgroundDefault.jpg"))
                {
                    File.Delete(windir + "\\System32\\oobe\\info\\backgrounds\\backgroundDefault.jpg");
                }
                File.Copy(jpegFilename, windir + "\\System32\\oobe\\Info\\Backgrounds\\backgroundDefault.jpg");
            }
        }
    }
}

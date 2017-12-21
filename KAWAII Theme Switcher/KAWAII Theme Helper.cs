/*
 * This file is absolutely FREE, can freely be copied, modified, compiled, or decompiled.
 * Also, you may print it and eat it with pizza, burn it in bizarre rituals,
 * or put it in the middle of Transmutation circle and do some shit, 
 * 
 * or whatever.
 * 
 * I don't give a fuck, as long as you leave this notice when distributing it.
 * 
 * 
 * 
 * Originally created by Kuchienkz.
 * 
 * Email: wahyu.darkflame@gmail.com
 */

using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KAWAII_Theme_Switcher
{
    static class KAWAII_Theme_Helper
    {
        private static string windir = Environment.GetEnvironmentVariable("windir");

        [DllImport("UxTheme.Dll", EntryPoint = "#65", CharSet = CharSet.Unicode)]
        public static extern int SetSystemVisualStyle(string pszFilename, string pszColor, string pszSize, int dwReserved);

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
        public interface ITheme
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
        public interface IThemeManager
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
        public interface ThemeManager : IThemeManager { }

        [ComImport, Guid("C04B329E-5823-4415-9C93-BA44688947B0"), ClassInterface(ClassInterfaceType.None), TypeLibType(TypeLibTypeFlags.FCanCreate)]
        public class ThemeManagerClass : IThemeManager, ThemeManager
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

        public static void ApplyTheme(string themePath)
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
                    }
                }
            }
            SetSystemVisualStyle(msstylePath, color, size, 0);
            ChangeTheme(themePath);
            TCSound = TCSound < 1500 ? 1500 : TCSound;
            Task.Factory.StartNew(() => Thread.Sleep(TCSound + 500)).Wait();
        }

        [PermissionSet(SecurityAction.LinkDemand)]
        public static void ChangeTheme(string themeFilePath)
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
        public static void ChangeLogonBackground(string jpegFilename)
        {
            if (!File.Exists(jpegFilename))
            {
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

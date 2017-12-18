using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KAWAII_Theme_Switcher
{
    static class Program
    {
        private static string[] _themes;
        private static string[] _exclude;

        [STAThread(), PermissionSet(SecurityAction.LinkDemand)]
        static void Main(string[] args)
        {
            string startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            if (File.Exists(Environment.CurrentDirectory + "\\startup.txt"))
            {
                int delay = Int32.Parse(File.ReadAllLines(Environment.CurrentDirectory + "\\startup.txt")[0].RegexReplace(@"[a-z_ :]", "", -1));
                IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
                string shortcutAddress = startupFolder + @"\KAWAII Theme Switcher.lnk";
                IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutAddress);
                shortcut.Description = "Shortcut for KAWAII Theme Switcher"; // set the description of the shortcut
                shortcut.WorkingDirectory = Application.StartupPath; /* working directory */
                shortcut.TargetPath = Application.ExecutablePath; /* path of the executable */
                shortcut.Arguments = "/a /c";
                shortcut.Save(); // save the shortcut 

                Thread.Sleep(delay);
            }
            else
            {
                if (File.Exists(startupFolder + @"\KAWAII Theme Switcher.lnk"))
                {
                    File.Delete(startupFolder + @"\KAWAII Theme Switcher.lnk");
                }
            }
            
            var path = "";
            if (File.Exists(Environment.CurrentDirectory + "\\exclusion.txt"))
            {
                _exclude = File.ReadAllLines(Environment.CurrentDirectory + "\\exclusion.txt");
            }

            if (File.Exists(Environment.CurrentDirectory + @"\RSequence.txt"))
            {
                if (File.ReadAllLines(Environment.CurrentDirectory + "\\RSequence.txt").Where(a => !a.Replace(" ", "").Equals("")).Count() == 0)
                {
                    var rs = new DirectoryInfo(@"C:\Windows\Resources\Themes").GetFiles("*.theme", SearchOption.TopDirectoryOnly).Select(item => item.FullName).ToList();
                    rs.Shuffle();
                    File.WriteAllLines(Environment.CurrentDirectory + @"\RSequence.txt", rs.ToArray());
                }
                var lrse = File.ReadAllLines(Environment.CurrentDirectory + "\\RSequence.txt").Where(a => !a.Replace(" ", "").Equals(""));
                if (_exclude != null && _exclude.Count() > 0)
                {
                    lrse = lrse.Where(a => !_exclude.Contains(Path.GetFileNameWithoutExtension(a)));
                }
                var rse = new Queue<string>(lrse);
                path = rse.Dequeue();
                File.Delete(Environment.CurrentDirectory + @"\RSequence.txt");
                File.WriteAllLines(Environment.CurrentDirectory + @"\RSequence.txt", rse.ToArray());
            }
            else if (File.Exists(Environment.CurrentDirectory + @"\Sequence.txt"))
            {
                if (File.ReadAllLines(Environment.CurrentDirectory + "\\Sequence.txt").Where(a => !a.Replace(" ", "").Equals("")).Count() == 0)
                {
                    var rs = new DirectoryInfo(@"C:\Windows\Resources\Themes").GetFiles("*.theme", SearchOption.TopDirectoryOnly).Select(item => item.FullName).ToList();
                    File.WriteAllLines(Environment.CurrentDirectory + @"\Sequence.txt", rs.ToArray());
                }
                var lrse = File.ReadAllLines(Environment.CurrentDirectory + "\\Sequence.txt").Where(a => !a.Replace(" ", "").Equals(""));
                if (_exclude != null && _exclude.Count() > 0)
                {
                    lrse = lrse.Where(a => !_exclude.Contains(Path.GetFileNameWithoutExtension(a)));
                }
                var rse = new Queue<string>(lrse);
                path = rse.Dequeue();
                File.Delete(Environment.CurrentDirectory + @"\Sequence.txt");
                File.WriteAllLines(Environment.CurrentDirectory + @"\Sequence.txt", rse.ToArray());
            }
            else
            {
                _themes = new DirectoryInfo(@"C:\Windows\Resources\Themes").GetFiles("*.theme", SearchOption.TopDirectoryOnly).Select(item => item.FullName).ToArray();
                if (_exclude != null && _exclude.Count() > 0)
                {
                    _themes = _themes.Where(a => !_exclude.Contains(Path.GetFileNameWithoutExtension(a))).ToArray();
                }
                path = _themes[ThreadSafeRandom.ThisThreadsRandom.Next(0, _themes.Count())];
                while (Path.GetFileNameWithoutExtension(path) == KAWAII_ThemeEngine.GetCurrentThemeName() || Path.GetFileNameWithoutExtension(path) == KAWAII_ThemeEngine.GetCurrentVisualStyleName())
                {
                    path = _themes[ThreadSafeRandom.ThisThreadsRandom.Next(0, _themes.Count() - 1)];
                }
            }

            KAWAII_ThemeEngine.ApplyTheme(path);
        }

        public static class KAWAII_ThemeEngine
        {
            [DllImport("UxTheme.Dll", EntryPoint = "#65", CharSet = CharSet.Unicode)]
            public static extern int SetSystemVisualStyle(string pszFilename, string pszColor, string pszSize, int dwReserved);

            [DllImport("winmm.dll")]
            private static extern uint mciSendString(
            string command,
            StringBuilder returnValue,
            int returnLength,
            IntPtr winHandle);

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
        }
    }

    public static class ThreadSafeRandom
    {
        [ThreadStatic] private static Random Local;

        public static Random ThisThreadsRandom
        {
            get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }
    }

    static class MyExtensions
    {
        public static string RegexReplace(this string str, string pattern, string replacement, int count = 1)
        {
            return new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase).Replace(str, replacement, count);
        }
        public static bool Contains(this string s, string value, StringComparison comparison)
        {
            return s.IndexOf(value, comparison) >= 0;
        }
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}

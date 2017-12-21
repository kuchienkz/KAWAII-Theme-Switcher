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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using static KAWAII_Theme_Switcher.MyExtensions;

namespace KAWAII_Theme_Switcher
{
    static class Program
    {
        private static string[] _themes;
        private static string[] _exclude;
        private static string windir = Environment.GetEnvironmentVariable("windir");

        [STAThread(), PermissionSet(SecurityAction.LinkDemand)]
        static void Main(string[] args)
        {
            string startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            if (File.Exists(Environment.CurrentDirectory + "\\startup.txt"))
            {
                int delay = int.Parse(File.ReadAllLines(Environment.CurrentDirectory + "\\startup.txt")[0].RegexReplace(@"[a-z_ :=]", "", -1));
                IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
                string shortcutAddress = startupFolder + @"\KAWAII Theme Switcher.lnk";
                IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutAddress);
                shortcut.Description = "Shortcut for KAWAII Theme Switcher"; // set the description of the shortcut
                shortcut.WorkingDirectory = Application.StartupPath; /* working directory */
                shortcut.TargetPath = Application.ExecutablePath; /* path of the executable */
                shortcut.Arguments = "/a /c";
                shortcut.Save(); // save the shortcut 

                if (delay <= -1)
                {
                    using (System.Diagnostics.PerformanceCounter cpu = new System.Diagnostics.PerformanceCounter("Processor", "% Processor Time", "_Total"))
                    {
                        int hits = 0;
                        while (hits < delay*-1)
                        {
                            cpu.NextValue();
                            Thread.Sleep(1000);
                            if (cpu.NextValue() < 20)
                            {
                                hits++;
                            }
                            else
                            {
                                hits = 0;
                            }
                        }
                    }
                }
                else
                {
                    Thread.Sleep(delay);
                }
            }
            else
            {
                if (File.Exists(startupFolder + @"\KAWAII Theme Switcher.lnk"))
                {
                    File.Delete(startupFolder + @"\KAWAII Theme Switcher.lnk");
                }
            }
            
            //Load Exclusion
            if (File.Exists(Environment.CurrentDirectory + "\\exclusion.txt"))
            {
                _exclude = File.ReadAllLines(Environment.CurrentDirectory + "\\exclusion.txt");
            }

            //Get latest list for checking new Themes
            var latestThemeList = new DirectoryInfo(windir + @"\Resources\Themes").GetFiles("*.theme", SearchOption.TopDirectoryOnly).Select(item => item.FullName).ToArray();
            if (_exclude != null && _exclude.Count() > 0)
            {
                latestThemeList = latestThemeList.Where(a => !_exclude.Contains(Path.GetFileNameWithoutExtension(a))).ToArray();
            }
            
            // Theme selection
            var path = "";
            if (File.Exists(Environment.CurrentDirectory + @"\RSequence.txt"))
            {
                if (!File.Exists(Environment.CurrentDirectory + @"\loaded.txt"))
                {
                    File.WriteAllText(Environment.CurrentDirectory + @"\loaded.txt", "");
                }
                var loaded = ReadAllLines(Environment.CurrentDirectory + "\\loaded.txt").ToList();
                if (ReadAllLines(Environment.CurrentDirectory + "\\RSequence.txt").Count() <= 0)
                {
                    var rs = latestThemeList.ToList();
                    rs.Shuffle();
                    File.WriteAllLines(Environment.CurrentDirectory + @"\RSequence.txt", rs);
                }
                else
                {
                    var queue = ReadAllLines(Environment.CurrentDirectory + "\\RSequence.txt").ToList();
                    var union = loaded.Concat(queue);
                    var newThemes = latestThemeList.Except(union);
                    if (newThemes.Count() > 0)
                    {
                        queue.AddRange(newThemes);
                        queue.Shuffle();
                        File.WriteAllLines(Environment.CurrentDirectory + @"\RSequence.txt", queue);
                    }
                }
                var lrse = ReadAllLines(Environment.CurrentDirectory + "\\RSequence.txt");
                if (_exclude != null && _exclude.Count() > 0)
                {
                    lrse = lrse.Where(a => !_exclude.Contains(Path.GetFileNameWithoutExtension(a)));
                }
                var rse = new Queue<string>(lrse);
                path = rse.Dequeue();
                loaded.Add(path);
                File.WriteAllLines(Environment.CurrentDirectory + @"\loaded.txt", loaded);
                File.WriteAllLines(Environment.CurrentDirectory + @"\RSequence.txt", rse);
            }
            else if (File.Exists(Environment.CurrentDirectory + @"\Sequence.txt"))
            {
                if (!File.Exists(Environment.CurrentDirectory + @"\loaded.txt"))
                {
                    File.WriteAllText(Environment.CurrentDirectory + @"\loaded.txt", "");
                }
                var loaded = ReadAllLines(Environment.CurrentDirectory + "\\loaded.txt").ToList();
                if (ReadAllLines(Environment.CurrentDirectory + "\\Sequence.txt").Count() <= 0)
                {
                    var rs = new DirectoryInfo( windir + @"\Resources\Themes").GetFiles("*.theme", SearchOption.TopDirectoryOnly).Select(item => item.FullName).ToList();
                    rs = rs.OrderBy(a => a).ToList();
                    File.WriteAllLines(Environment.CurrentDirectory + @"\Sequence.txt", rs);
                }
                else
                {
                    var queue = ReadAllLines(Environment.CurrentDirectory + "\\Sequence.txt").ToList();
                    var union = loaded.Concat(queue);
                    var newThemes = latestThemeList.Except(union);
                    if (newThemes.Count() > 0)
                    {
                        queue.AddRange(newThemes);
                        queue = queue.OrderBy(a => a).ToList();
                        File.WriteAllLines(Environment.CurrentDirectory + @"\Sequence.txt", queue);
                    }
                }
                var lrse = ReadAllLines(Environment.CurrentDirectory + "\\Sequence.txt");
                if (_exclude != null && _exclude.Count() > 0)
                {
                    lrse = lrse.Where(a => !_exclude.Contains(Path.GetFileNameWithoutExtension(a)));
                }
                var rse = new Queue<string>(lrse);
                path = rse.Dequeue();
                loaded.Add(path);
                File.WriteAllLines(Environment.CurrentDirectory + @"\loaded.txt", loaded);
                File.WriteAllLines(Environment.CurrentDirectory + @"\Sequence.txt", rse);
            }
            else
            {
                if (File.Exists(Environment.CurrentDirectory + @"\loaded.txt"))
                {
                    File.Delete(Environment.CurrentDirectory + @"\loaded.txt");
                }
                _themes = new DirectoryInfo(windir + @"\Resources\Themes").GetFiles("*.theme", SearchOption.TopDirectoryOnly).Select(item => item.FullName).ToArray();
                if (_exclude != null && _exclude.Count() > 0)
                {
                    _themes = _themes.Where(a => !_exclude.Contains(Path.GetFileNameWithoutExtension(a))).ToArray();
                }
                path = _themes[ThreadSafeRandom.ThisThreadsRandom.Next(0, _themes.Count() - 1)];
                while (Path.GetFileNameWithoutExtension(path) == KAWAII_Theme_Helper.GetCurrentThemeName() || Path.GetFileNameWithoutExtension(path) == KAWAII_Theme_Helper.GetCurrentVisualStyleName())
                {
                    path = _themes[ThreadSafeRandom.ThisThreadsRandom.Next(0, _themes.Count() - 1)];
                }
            }

            KAWAII_Theme_Helper.ApplyTheme(path);

            // Logon Modifier
            if (File.Exists(Environment.CurrentDirectory + @"\logon.txt"))
            {
                if (Directory.Exists(windir + @"\Resources\Logon"))
                {
                    var logons = Directory.GetFiles(windir + @"\Resources\Logon", "*.jpg", SearchOption.AllDirectories).ToList();
                    logons = logons.Where(a => !_exclude.Any(b => a.Contains("\\" + b + "\\"))).ToList();
                    if (logons.Count() > 0)
                    {
                        var mode = File.ReadAllLines(Environment.CurrentDirectory + "\\logon.txt")[0].RegexReplace(@"[a-z_ ]+[:=]{1} ?", "", -1).ToLower();
                        if (mode.Equals("random") || mode.Equals("r"))
                        {
                            if (File.Exists(Environment.CurrentDirectory + "\\logon.used"))
                            {
                                File.Delete(Environment.CurrentDirectory + "\\logon.used");
                            }
                            KAWAII_Theme_Helper.ChangeLogonBackground(logons[ThreadSafeRandom.ThisThreadsRandom.Next(0, logons.Count() - 1)]);
                        }
                        else if (mode.Equals("sequence") || mode.Equals("s"))
                        {
                            if (!File.Exists(Environment.CurrentDirectory + "\\logon.used"))
                            {
                                File.WriteAllText(Environment.CurrentDirectory + "\\logon.used", "");
                            }
                            var loaded  = ReadAllLines(Environment.CurrentDirectory + "\\logon.used").ToList();
                            if (!File.Exists(Environment.CurrentDirectory + @"\logon.seq") || ReadAllLines(Environment.CurrentDirectory + @"\logon.seq").Count() <= 0)
                            {
                                logons = logons.OrderBy(a => a).ToList();
                                File.WriteAllLines(Environment.CurrentDirectory + @"\logon.seq", logons);
                            }
                            else
                            {
                                var queue = ReadAllLines(Environment.CurrentDirectory + "\\logon.seq").ToList();
                                var union = loaded.Concat(queue);
                                var newLogons = logons.Except(union);
                                if (newLogons.Count() > 0)
                                {
                                    queue.AddRange(newLogons);
                                    queue.OrderBy(a => a).ToList();
                                    File.WriteAllLines(Environment.CurrentDirectory + @"\logon.seq", queue);
                                }
                            }
                            var seq = ReadAllLines(Environment.CurrentDirectory + @"\logon.seq");
                            if (_exclude != null && _exclude.Count() > 0)
                            {
                                seq = seq.Where(a => !_exclude.Any(b => a.Contains("\\" + b + "\\")));
                            }
                            var rse = new Queue<string>(seq);
                            var lg = rse.Dequeue();
                            loaded.Add(lg);
                            File.WriteAllLines(Environment.CurrentDirectory + @"\logon.used", loaded);
                            File.WriteAllLines(Environment.CurrentDirectory + @"\logon.seq", rse);
                            KAWAII_Theme_Helper.ChangeLogonBackground(lg);
                        }
                        else if (mode.Equals("random sequence") || mode.Equals("rs"))
                        {
                            if (!File.Exists(Environment.CurrentDirectory + "\\logon.used"))
                            {
                                File.WriteAllText(Environment.CurrentDirectory + "\\logon.used", "");
                            }
                            var loaded = ReadAllLines(Environment.CurrentDirectory + "\\logon.used").ToList(); ;
                            if (!File.Exists(Environment.CurrentDirectory + @"\logon.rseq") || ReadAllLines(Environment.CurrentDirectory + @"\logon.rseq").Count() <= 0)
                            {
                                logons.Shuffle();
                                File.WriteAllLines(Environment.CurrentDirectory + @"\logon.rseq", logons);
                            }
                            else
                            {
                                var queue = ReadAllLines(Environment.CurrentDirectory + "\\logon.rseq").ToList();
                                var union = loaded.Concat(queue);
                                var newLogons = logons.Except(union);
                                if (newLogons.Count() > 0)
                                {
                                    queue.AddRange(newLogons);
                                    queue.Shuffle();
                                    File.WriteAllLines(Environment.CurrentDirectory + @"\logon.rseq", queue);
                                }
                            }
                            var seq = ReadAllLines(Environment.CurrentDirectory + @"\logon.rseq");
                            if (_exclude != null && _exclude.Count() > 0)
                            {
                                seq = seq.Where(a => !_exclude.Any(b => a.Contains("\\" + b + "\\")));
                            }
                            var rse = new Queue<string>(seq);
                            var lg = rse.Dequeue();
                            loaded.Add(lg);
                            File.WriteAllLines(Environment.CurrentDirectory + @"\logon.used", loaded);
                            File.WriteAllLines(Environment.CurrentDirectory + @"\logon.rseq", rse);
                            KAWAII_Theme_Helper.ChangeLogonBackground(lg);
                        }
                        else // Respective mode
                        {
                            if (File.Exists(Environment.CurrentDirectory + "\\logon.used"))
                            {
                                File.Delete(Environment.CurrentDirectory + "\\logon.used");
                            }
                            if (Directory.Exists(windir + @"\Resources\Logon\" + Path.GetFileNameWithoutExtension(path)))
                            {
                                var files = Directory.GetFiles(windir + @"\Resources\Logon\" + Path.GetFileNameWithoutExtension(path), "*.jpg", SearchOption.AllDirectories);
                                if (files.Count() > 0)
                                {
                                    KAWAII_Theme_Helper.ChangeLogonBackground(files[0]);
                                }
                            }
                        }
                    }
                }
            }
        }


    }

    static class ThreadSafeRandom
    {
        [ThreadStatic] private static Random Local;

        public static Random ThisThreadsRandom
        {
            get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }
    }

    static class MyExtensions
    {
        public static IEnumerable<string> ReadAllLines(string filename)
        {
            return File.ReadAllLines(filename).Where(a => !a.Replace(" ", "").Equals(""));
        }
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

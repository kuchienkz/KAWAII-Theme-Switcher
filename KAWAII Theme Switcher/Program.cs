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
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;

using static KAWAII_Theme_Switcher.HelperFunction;

namespace KAWAII_Theme_Switcher
{
    public enum WindowsVersion
    {
        WIN10,
        WIN8,
        WIN7,
        UNSUPPORTED
    }

    public static class Program
    {
        private static string[] _exclude;
        private static string appFolder = AppDomain.CurrentDomain.BaseDirectory;
        public static List<string> log = new List<string>();
        private static WindowsVersion winVer = GetWindowsVersion();

        [STAThread(), PermissionSet(SecurityAction.LinkDemand)]
        static void Main(string[] args)
        {
            if (appFolder.Last() == '\\')
            {
                appFolder = appFolder.Remove(appFolder.Length - 1, 1);
            }

            var path = "";
            log.Add("[" + DateTime.Now.ToLongDateString() + "]");
            log.Add("Environtment directory: " + appFolder);
            if (args.Length > 0)
            {
                log.Add("_Using Command Prompt mode...");
                log.Add("__Command: " + String.Join(" ", args));
                if (!args[0].Equals(""))
                {
                    if (File.Exists(args[0]) && Path.GetExtension(args[0]).Equals(".theme"))
                    {
                        path = args[0];
                    }
                    else if (File.Exists(KAWAII_Theme_Helper.windir + @"\Resources\Themes\" + args[0] + ".theme") || File.Exists(KAWAII_Theme_Helper.windir + @"\Resources\Themes\" + args[0]))
                    {
                        path = KAWAII_Theme_Helper.windir + @"\Resources\Themes\" + args[0] + ".theme";
                        path = path.Replace(".theme.theme", ".theme");
                    }
                    else if (args[0].EqualsIgnoreCase("random"))
                    {
                        var themeList = new DirectoryInfo(KAWAII_Theme_Helper.windir + @"\Resources\Themes").GetFiles("*.theme", SearchOption.TopDirectoryOnly).Select(item => item.FullName).ToArray();
                        path = themeList[ThreadSafeRandom.ThisThreadsRandom.Next(0, themeList.Count() - 1)];
                        while (Path.GetFileNameWithoutExtension(path) == KAWAII_Theme_Helper.GetCurrentThemeName() || Path.GetFileNameWithoutExtension(path) == KAWAII_Theme_Helper.GetCurrentVisualStyleName())
                        {
                            path = themeList[ThreadSafeRandom.ThisThreadsRandom.Next(0, themeList.Count() - 1)];
                        }
                    }
                }
                if (args.Length > 1)
                {
                    if (Path.GetExtension(args[1]).EqualsIgnoreCase(".jpg"))
                    {
                        if (File.Exists(args[1]))
                        {
                            KAWAII_Theme_Helper.ChangeLockscreenBackground(args[1], winVer);
                            log.Add("___Lockscreen applied: " + args[1]);
                        }
                        else if (File.Exists(appFolder + "\\" + args[1]))
                        {
                            KAWAII_Theme_Helper.ChangeLockscreenBackground(appFolder + "\\" + args[1], winVer);
                            log.Add("___Lockscreen applied: " + args[1]);
                        }
                    }
                    else if (args[1].EqualsIgnoreCase("random"))
                    {
                        ChangeLockscreen("random", _exclude, "", true);
                    }
                    else if (!path.Equals("") && !args[1].Equals(""))
                    {
                        ChangeLockscreen("respective", _exclude, path, true);
                    }
                }

                if (!path.Equals(""))
                {
                    KAWAII_Theme_Helper.ApplyTheme(path);
                    log.Add("___Theme applied: " + path);
                }
                log.Add("_END");
            }
            else
            {
                log.Add("_Using Standard mode...");
                int exitDelay = 1500;

                RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                var valName = AppDomain.CurrentDomain.FriendlyName.Replace(".exe", "");
                if (File.Exists(appFolder + "\\startup.txt"))
                {
                    log.Add("__startup.txt found!");
                    int startupDelay = -3;
                    var startupParams = File.ReadAllLines(appFolder + "\\startup.txt").Where(a => !a.Replace(" ", "").Equals("")).ToArray();
                    log.Add("__Startup params: " + String.Join(" | ", startupParams));
                    if (startupParams.Count() > 0 && !int.TryParse(startupParams[0].RegexReplace(@"[a-z_ :=]", "", -1), out startupDelay))
                    {
                        startupDelay = -3;
                    }
                    if (startupParams.Count() > 1 && !int.TryParse(startupParams[1].RegexReplace(@"[a-z_ :=]", "", -1), out exitDelay))
                    {
                        exitDelay = 1500;
                    }
                    if (exitDelay < 0)
                    {
                        exitDelay = 1500;
                    }

                    if (rk.GetValue(valName) == null)
                    {
                        log.Add("___Startup registry NOT found! Creating new registry...");
                        rk.SetValue(valName, Application.ExecutablePath);
                    }

                    if (startupDelay <= -1)
                    {
                        log.Add("___Using Smart Delay...");
                        using (System.Diagnostics.PerformanceCounter cpu = new System.Diagnostics.PerformanceCounter("Processor", "% Processor Time", "_Total"))
                        {
                            int hits = 0;
                            startupDelay = startupDelay <= -1000 ? (int)Math.Ceiling(startupDelay / -1000.0) : (int)Math.Ceiling(startupDelay / -1.0);
                            while (hits < startupDelay)
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
                        Thread.Sleep(startupDelay);
                    }
                }
                else
                {
                    log.Add("__startup.txt NOT found!");
                    if (rk.GetValue(valName) != null)
                    {
                        log.Add("___Startup registry found! Deleting registry...");
                        rk.DeleteValue(valName, false);
                    }
                }

                if (File.Exists(appFolder + "\\skip.txt"))
                {
                    log.Add("__skip.txt found!");
                    if (File.ReadAllText(appFolder + "\\skip.txt").Replace(" ", "").Equals(""))
                    {
                        log.Add("___Using 'skip once', deleting skip.txt...");
                        File.Delete(appFolder + "\\skip.txt");
                        log.Add("___END");
                        log.Add("");
                        File.AppendAllLines(appFolder + "\\logs.txt", log);
                        return;
                    }

                    var lns = File.ReadAllLines(appFolder + "\\skip.txt");
                    File.Delete(appFolder + "\\skip.txt");
                    log.Add("__Skip params: " + string.Join(" | ", lns));
                    int rem = int.Parse(lns[0].Replace(" ", "").Split('/')[0]);
                    if (lns.Length == 2)
                    {
                        if (rem <= 0)
                        {
                            log.Add("___Remaining skip has reached Zero! Changing theme...");
                            if (lns[1].Replace(" ", "").Equals("repeat", StringComparison.OrdinalIgnoreCase) || Equals("true", StringComparison.OrdinalIgnoreCase))
                            {
                                log.Add("____'Repeating Skip' enabled!");
                                rem = int.Parse(lns[0].Replace(" ", "").Split('/')[1]);
                                File.WriteAllLines(appFolder + "\\skip.txt", new string[] { rem + "/" + rem, "repeat" });
                            }
                        }
                        else
                        {
                            log.Add("___Remaining skip is " + rem + ", skipping current switch!");
                            rem--;
                            File.WriteAllLines(appFolder + "\\skip.txt", new string[] { rem + "/" + lns[0].Replace(" ", "").Split('/')[1] });
                            if (lns[1].Replace(" ", "").Equals("repeat", StringComparison.OrdinalIgnoreCase) || Equals("true", StringComparison.OrdinalIgnoreCase))
                            {
                                log.Add("____'Repeating Skip' enabled!");
                                File.AppendAllLines(appFolder + "\\skip.txt", new string[] { "repeat" });
                            }
                            log.Add("___END");
                            log.Add("");
                            File.AppendAllLines(appFolder + "\\logs.txt", log);
                            return;
                        }
                    }
                    else
                    {
                        log.Add("___Skip using only 1 parameter!");
                        if (rem > 0)
                        {
                            log.Add("____Remaining skip is " + rem + ", skipping current switch!");
                            rem--;
                            File.WriteAllLines(appFolder + "\\skip.txt", new string[] { rem + "/" + lns[0].Replace(" ", "").Split('/')[1] });
                            log.Add("____END");
                            log.Add("");
                            File.AppendAllLines(appFolder + "\\logs.txt", log);
                            return;
                        }
                    }
                }

                if (File.Exists(appFolder + "\\safetheme.txt"))
                {
                    log.Add("__safetheme.txt found! Asking for switching...");
                    // TODO
                }

                //Load Exclusion
                if (File.Exists(appFolder + "\\exclusion.txt"))
                {
                    log.Add("__exclusion.txt found! Loading exclusion...");
                    _exclude = ReadAllLines(appFolder + "\\exclusion.txt").ToArray();
                    log.Add("__Exclusion count: " + _exclude.Count());
                }

                //Get latest list for checking new Themes
                List<string> latestThemeList = new List<string>();
                foreach (var d in Directory.GetDirectories(KAWAII_Theme_Helper.windir + @"\Resources"))
                {
                    latestThemeList.AddRange(Directory.GetFiles(d, "*.theme", SearchOption.AllDirectories));
                }
                if (_exclude != null && _exclude.Count() > 0)
                {
                    latestThemeList = latestThemeList.Where(a => !_exclude.Contains(Path.GetFileNameWithoutExtension(a))).ToList();
                }

                // Theme selection
                if (File.Exists(appFolder + @"\RSequence.txt"))
                {
                    if (!File.Exists(appFolder + @"\loaded.txt"))
                    {
                        File.WriteAllText(appFolder + @"\loaded.txt", "");
                    }
                    var loaded = ReadAllLines(appFolder + "\\loaded.txt").ToList();
                    if (ReadAllLines(appFolder + "\\RSequence.txt").Count() <= 0)
                    {
                        var rs = latestThemeList.ToList();
                        rs.Shuffle();
                        File.WriteAllLines(appFolder + @"\RSequence.txt", rs);
                    }
                    else
                    {
                        var queue = ReadAllLines(appFolder + "\\RSequence.txt").ToList();
                        var union = loaded.Concat(queue);
                        var newThemes = latestThemeList.Except(union);
                        if (newThemes.Count() > 0)
                        {
                            queue.AddRange(newThemes);
                            queue.Shuffle();
                            File.WriteAllLines(appFolder + @"\RSequence.txt", queue);
                        }
                    }
                    var lrse = ReadAllLines(appFolder + "\\RSequence.txt");
                    if (_exclude != null && _exclude.Count() > 0)
                    {
                        lrse = lrse.Where(a => !_exclude.Contains(Path.GetFileNameWithoutExtension(a)));
                    }
                    var rse = new Queue<string>(lrse);
                    path = rse.Dequeue();
                    loaded.Add(path);
                    File.WriteAllLines(appFolder + @"\loaded.txt", loaded);
                    File.WriteAllLines(appFolder + @"\RSequence.txt", rse);
                }
                else if (File.Exists(appFolder + @"\Sequence.txt"))
                {
                    if (!File.Exists(appFolder + @"\loaded.txt"))
                    {
                        File.WriteAllText(appFolder + @"\loaded.txt", "");
                    }
                    var loaded = ReadAllLines(appFolder + "\\loaded.txt").ToList();
                    if (ReadAllLines(appFolder + "\\Sequence.txt").Count() <= 0)
                    {
                        var rs = new DirectoryInfo(KAWAII_Theme_Helper.windir + @"\Resources\Themes").GetFiles("*.theme", SearchOption.TopDirectoryOnly).Select(item => item.FullName).ToList();
                        rs = rs.OrderBy(a => a).ToList();
                        File.WriteAllLines(appFolder + @"\Sequence.txt", rs);
                    }
                    else
                    {
                        var queue = ReadAllLines(appFolder + "\\Sequence.txt").ToList();
                        var union = loaded.Concat(queue);
                        var newThemes = latestThemeList.Except(union);
                        if (newThemes.Count() > 0)
                        {
                            queue.AddRange(newThemes);
                            queue = queue.OrderBy(a => a).ToList();
                            File.WriteAllLines(appFolder + @"\Sequence.txt", queue);
                        }
                    }
                    var lrse = ReadAllLines(appFolder + "\\Sequence.txt");
                    if (_exclude != null && _exclude.Count() > 0)
                    {
                        lrse = lrse.Where(a => !_exclude.Contains(Path.GetFileNameWithoutExtension(a)));
                    }
                    var rse = new Queue<string>(lrse);
                    path = rse.Dequeue();
                    loaded.Add(path);
                    File.WriteAllLines(appFolder + @"\loaded.txt", loaded);
                    File.WriteAllLines(appFolder + @"\Sequence.txt", rse);
                }
                else
                {
                    if (File.Exists(appFolder + @"\loaded.txt"))
                    {
                        File.Delete(appFolder + @"\loaded.txt");
                    }
                    path = latestThemeList[ThreadSafeRandom.ThisThreadsRandom.Next(0, latestThemeList.Count() - 1)];
                    while (Path.GetFileNameWithoutExtension(path) == KAWAII_Theme_Helper.GetCurrentThemeName() || Path.GetFileNameWithoutExtension(path) == KAWAII_Theme_Helper.GetCurrentVisualStyleName())
                    {
                        path = latestThemeList[ThreadSafeRandom.ThisThreadsRandom.Next(0, latestThemeList.Count() - 1)];
                    }
                }

                // Lockscreen Modifier
                if (File.Exists(appFolder + @"\lockscreen.txt"))
                {
                    var prms = File.ReadAllLines(appFolder + "\\lockscreen.txt").ToList();
                    if (prms.Count == 0)
                    {
                        prms.Add("respective");
                    }
                    ChangeLockscreen(prms[0].RegexReplace(@"[a-z_ ]+[:=]{1} ?", "", -1).ToLower(), _exclude, path);
                }

                // Apply Theme/Visual Style
                KAWAII_Theme_Helper.ApplyTheme(path, exitDelay);
                log.Add("___Theme applied: " + path);

                log.Add("_END");
                log.Add("");
            }
            File.AppendAllLines(appFolder + "\\logs.txt", log);
        }

        static void SearchForLockscreenBackgrounds(ref List<string> list)
        {
            if (Directory.Exists(appFolder + @"\Lockscreen"))
            {
                var jpgs = Directory.GetFiles(appFolder + @"\Lockscreen", "*.jpg", SearchOption.AllDirectories);
                if (jpgs.Length > 0)
                {
                    list.AddRange(jpgs);
                }
            }
            if (Directory.Exists(KAWAII_Theme_Helper.windir + @"\Resources\Lockscreen"))
            {
                var jpgs = Directory.GetFiles(KAWAII_Theme_Helper.windir + @"\Resources\Lockscreen", "*.jpg", SearchOption.AllDirectories);
                if (jpgs.Length > 0)
                {
                    list.AddRange(jpgs);
                }
            }
            if (Directory.Exists(KAWAII_Theme_Helper.windir + @"\Resources\Logon"))
            {
                var jpgs = Directory.GetFiles(KAWAII_Theme_Helper.windir + @"\Resources\Logon", "*.jpg", SearchOption.AllDirectories);
                if (jpgs.Length > 0)
                {
                    list.AddRange(jpgs);
                }
            }

            if (list.Count == 0 && !Directory.Exists(appFolder + @"\Lockscreen"))
            {
                Directory.CreateDirectory(appFolder + @"\Lockscreen");
                File.WriteAllText(appFolder + "\\Lockscreen\\Put Lock Screen images here.txt",
                    "When using Respective mode, either put it inside the related theme folder and name it lockscreen.jpg, OR you can put it in this folder but make sure the jpg file has the same name as the theme. Example:" +
                    "\n\nKagamine Rin.theme" +
                    "\nKagamine Rin.jpg");
            }
            else
            {
                if (list.Count > 0 && _exclude != null)
                    list = list.Where(a => !_exclude.Any(b => a.Contains("\\" + b + "\\"))).ToList();
            }
        }

        static void ChangeLockscreen(string mode, string[] _exclude, string path, bool commandPrompt = false)
        {
            if (_exclude == null)
            {
                _exclude = new string[0];
            }

            var lockscreens = new List<string>();
            if (mode.EqualsIgnoreCase("respective"))
            {
                if (commandPrompt == false && File.Exists(appFolder + "\\lockscreen.used"))
                {
                    File.Delete(appFolder + "\\lockscreen.used");
                }

                var rawpath = File.ReadAllLines(path).DefaultIfEmpty("").FirstOrDefault(l => l.Contains(".msstyles"));
                if (rawpath.Length > 0)
                {
                    var themeDir = Path.GetDirectoryName(Environment.ExpandEnvironmentVariables(rawpath));
                    if (File.Exists(themeDir + "\\lockscreen.jpg"))
                    {
                        KAWAII_Theme_Helper.ChangeLockscreenBackground(themeDir + "\\lockscreen.jpg", winVer);
                        log.Add("___Lockscreen applied: " + themeDir + "\\lockscreen.jpg");
                    }
                    else if (File.Exists(themeDir + "\\logon.jpg"))
                    {
                        KAWAII_Theme_Helper.ChangeLockscreenBackground(themeDir + "\\logon.jpg", winVer);
                        log.Add("___Lockscreen applied: " + themeDir + "\\logon.jpg");
                    }
                    else
                    {
                        SearchForLockscreenBackgrounds(ref lockscreens);

                        foreach (var bg in lockscreens)
                        {
                            if (Path.GetFileNameWithoutExtension(bg).EqualsIgnoreCase(Path.GetFileNameWithoutExtension(path)))
                            {
                                KAWAII_Theme_Helper.ChangeLockscreenBackground(bg, winVer);
                                log.Add("___Lockscreen applied: " + bg);
                                break;
                            }
                        }

                        return;
                    }
                }
            }
            else
            {
                SearchForLockscreenBackgrounds(ref lockscreens);

                if (lockscreens.Count > 0)
                {
                    if (mode.EqualsIgnoreCase("random"))
                    {
                        if (commandPrompt == false && File.Exists(appFolder + "\\lockscreen.used"))
                        {
                            File.Delete(appFolder + "\\lockscreen.used");
                        }
                        var s = lockscreens[ThreadSafeRandom.ThisThreadsRandom.Next(0, lockscreens.Count() - 1)];
                        KAWAII_Theme_Helper.ChangeLockscreenBackground(s, winVer);
                        log.Add("___Lockscreen applied: " + s);
                    }
                    else if (mode.EqualsIgnoreCase("sequence"))
                    {
                        if (!File.Exists(appFolder + "\\lockscreen.used"))
                        {
                            File.WriteAllText(appFolder + "\\lockscreen.used", "");
                        }
                        var loaded = ReadAllLines(appFolder + "\\lockscreen.used").ToList();
                        if (!File.Exists(appFolder + @"\lockscreen.seq") || ReadAllLines(appFolder + @"\lockscreen.seq").Count() <= 0)
                        {
                            lockscreens = lockscreens.OrderBy(a => a).ToList();
                            File.WriteAllLines(appFolder + @"\lockscreen.seq", lockscreens);
                        }
                        else
                        {
                            var queue = ReadAllLines(appFolder + "\\lockscreen.seq").ToList();
                            var union = loaded.Concat(queue);
                            var newLockscreens = lockscreens.Except(union);
                            if (newLockscreens.Count() > 0)
                            {
                                queue.AddRange(newLockscreens);
                                queue.OrderBy(a => a).ToList();
                                File.WriteAllLines(appFolder + @"\lockscreen.seq", queue);
                            }
                        }
                        var seq = ReadAllLines(appFolder + @"\lockscreen.seq");
                        if (_exclude != null && _exclude.Count() > 0)
                        {
                            seq = seq.Where(a => !_exclude.Any(b => a.Contains("\\" + b + "\\")));
                        }
                        var rse = new Queue<string>(seq);
                        var lg = rse.Dequeue();
                        loaded.Add(lg);
                        File.WriteAllLines(appFolder + @"\lockscreen.used", loaded);
                        File.WriteAllLines(appFolder + @"\lockscreen.seq", rse);
                        KAWAII_Theme_Helper.ChangeLockscreenBackground(lg, winVer);
                        log.Add("___Lockscreen applied: " + lg);
                    }
                    else if (mode.EqualsIgnoreCase("random sequence"))
                    {
                        if (!File.Exists(appFolder + "\\lockscreen.used"))
                        {
                            File.WriteAllText(appFolder + "\\lockscreen.used", "");
                        }
                        var loaded = ReadAllLines(appFolder + "\\lockscreen.used").ToList(); ;
                        if (!File.Exists(appFolder + @"\lockscreen.rseq") || ReadAllLines(appFolder + @"\lockscreen.rseq").Count() <= 0)
                        {
                            lockscreens.Shuffle();
                            File.WriteAllLines(appFolder + @"\lockscreen.rseq", lockscreens);
                        }
                        else
                        {
                            var queue = ReadAllLines(appFolder + "\\lockscreen.rseq").ToList();
                            var union = loaded.Concat(queue);
                            var newLockscreens = lockscreens.Except(union);
                            if (newLockscreens.Count() > 0)
                            {
                                queue.AddRange(newLockscreens);
                                queue.Shuffle();
                                File.WriteAllLines(appFolder + @"\lockscreen.rseq", queue);
                            }
                        }
                        var seq = ReadAllLines(appFolder + @"\lockscreen.rseq");
                        if (_exclude != null && _exclude.Count() > 0)
                        {
                            seq = seq.Where(a => !_exclude.Any(b => a.Contains("\\" + b + "\\")));
                        }
                        var rse = new Queue<string>(seq);
                        var lg = rse.Dequeue();
                        loaded.Add(lg);
                        File.WriteAllLines(appFolder + @"\lockscreen.used", loaded);
                        File.WriteAllLines(appFolder + @"\lockscreen.rseq", rse);
                        KAWAII_Theme_Helper.ChangeLockscreenBackground(lg, winVer);
                        log.Add("___Lockscreen applied: " + lg);
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

    static class HelperFunction
    {
        public static IEnumerable<string> ReadAllLines(string filename)
        {
            return File.ReadAllLines(filename).Where(a => !a.Replace(" ", "").Equals(""));
        }
        public static bool EqualsIgnoreCase(this string str, string compareWith)
        {
            return str.ToLower().Equals(compareWith.ToLower());
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
        public static WindowsVersion GetWindowsVersion()
        {
            WindowsVersion ver = WindowsVersion.UNSUPPORTED;
            string r = "";
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
            {
                ManagementObjectCollection information = searcher.Get();
                if (information != null)
                {
                    foreach (ManagementObject obj in information)
                    {
                        r = obj["Caption"].ToString() + " - " + obj["OSArchitecture"].ToString();
                    }
                }

                if (r.Contains("Windows 10"))
                {
                    ver =  WindowsVersion.WIN10;
                }
                else if (r.Contains("Windows 8"))
                {
                    ver = WindowsVersion.WIN8;
                }
                else if (r.Contains("Windows 7"))
                {
                    ver = WindowsVersion.WIN7;
                }
            }

            return ver;
        }
    }

}

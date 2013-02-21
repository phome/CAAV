using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace caav
{
    class Program
    {
        static string sPath = "";
        static int[] nInc = { 0, 0, 0, 0 };
        static int nDec = 1;


        static void Main(string[] args)
        {
            try
            {
                if (!ParseArg(args)) return;
                List<string> Files = GetFiles(sPath, "AssemblyInfo.cs");
                Files.AddRange(GetFiles(sPath, "AssemblyInfo.tmpl"));
                Regex RegAssembly = new Regex(
                    @"(?<pre>\[assembly\: Assembly(File)?Version\("")(?<d1>\d{1,})(?<p2>\.(?<d2>\d{1,}))((?<p3>\.((?<d3>\d{1,})|(\*{1})|([^\d\*]+[^\.]*)))(?<p4>\.((?<d4>\d{1,})|(\*{1})|([^\d\*]+[^\.]*)))?)?(?<app>""\)\])");

                foreach (string _File in Files)
                {
                    try
                    {
                        string BackupFile = _File;
                        int nIndex = BackupFile.LastIndexOf(".");
                        BackupFile = BackupFile.Substring(0, nIndex) + "_BAK" + BackupFile.Substring(nIndex);

                        StringBuilder result = new StringBuilder();
                        string[] str = File.ReadAllLines(_File);

                        foreach (string _str in str)
                            result.Append(GetResult(RegAssembly, _str) + "\r\n");

                        File.Delete(BackupFile);
                        File.Copy(_File, BackupFile);
                        File.WriteAllText(_File, result.ToString());
                    }
                    catch (Exception e2)
                    {
                    }
                }
            }
            catch (Exception e2)
            {
            }
        }


        static bool ParseArg(string[] args)
        {
            if (args == null || args.Count() == 0 || args.Count() > 3) { ShowHelp(); return false; }

            Regex regInc = new Regex(@"^(\*?)\.(\*?)\.(\*?)\.(\*?)$");
            Regex regMin = new Regex(@"^[-\+]{1}$");

            foreach (string s in args)
            {
                string sArg = s.Trim();
                if (regInc.IsMatch(sArg))
                {
                    Match m = regInc.Match(sArg);
                    nInc[0] = (m.Groups[1].Value == "*") ? 1 : 0;
                    nInc[1] = (m.Groups[2].Value == "*") ? 1 : 0;
                    nInc[2] = (m.Groups[3].Value == "*") ? 1 : 0;
                    nInc[3] = (m.Groups[4].Value == "*") ? 1 : 0;
                }
                else if (regMin.IsMatch(sArg))
                {
                    if (sArg == "-")
                    {
                        nDec = -1;
                    }
                    else
                    {
                        nDec = 1;
                    }
                }
                else
                {
                    if (sArg.Length > 0) sPath = sArg;
                }
            }

            if (sPath.Length == 0)
            {
                sPath = Process.GetCurrentProcess().MainModule.FileName;
                sPath = sPath.Substring(0, sPath.LastIndexOf("\\"));
            }

            if (!Directory.Exists(sPath)) { ShowHelp(); return false; }
            return true;
        }

        static void ShowHelp()
        {
            Console.WriteLine("caav v{0}",
                    Process.GetCurrentProcess().MainModule.FileVersionInfo.FileVersion);
            Console.WriteLine("Console Automatic Assembly Version");
            Console.WriteLine("usage:");
            Console.WriteLine("caav [start path] [-] *.*.*.*");
            Console.WriteLine("caav [start path] [-] ...*");
        }

        static List<string> GetFiles(string vDir, string vSearch)
        {
            List<string> _Files = new List<string>();
            if (Directory.Exists(vDir))
            {
                _Files.AddRange(Directory.GetFiles(vDir, vSearch, SearchOption.AllDirectories));
            }
            return _Files;
        }

        private static string GetResult(Regex RegPattern, string vstr)
        {
            if (!RegPattern.IsMatch(vstr)) return vstr;

            string str = vstr;
            Match m = RegPattern.Match(str);

            if (nInc[0] > 0)
            {
                try
                {
                    string s = GetNextVer(m.Groups["d1"].Value, nInc[0]).ToString();
                    if (s.Length > 0) { str = RegPattern.Replace(str, "${pre}" + s + "${p2}${p3}${p4}${app}"); }
                }
                catch (FormatException e2) { }
            }

            if (nInc[1] > 0)
            {
                try
                {
                    string s = GetNextVer(m.Groups["d2"].Value, nInc[1]).ToString();
                    if (s.Length > 0) { str = RegPattern.Replace(str, "${pre}${d1}." + s + "${p3}${p4}${app}"); }
                }
                catch (FormatException e2) { }
            }

            if (nInc[2] > 0)
            {
                try
                {
                    string s = GetNextVer(m.Groups["d3"].Value, nInc[2]).ToString();
                    if (s.Length > 0) { str = RegPattern.Replace(str, "${pre}${d1}${p2}." + s + "${p4}${app}"); }
                }
                catch (FormatException e2) { }
            }

            if (nInc[3] > 0)
            {
                try
                {
                    string s = GetNextVer(m.Groups["d4"].Value, nInc[3]).ToString();
                    if (s.Length > 0) { str = RegPattern.Replace(str, "${pre}${d1}${p2}${p3}." + s + "${app}"); }
                }
                catch (FormatException e2) { }
            }

            return str;

        }

        private static int GetNextVer(string aVer, int adoInc)
        {
            int n = Convert.ToInt32(aVer) + (nDec * adoInc);
            if (n < 0) n = 0;
            return n;
        }
    }
}

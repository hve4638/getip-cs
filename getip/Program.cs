using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace getip
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool help = args.Contains("-h") || args.Contains("?");
            bool simple = args.Contains("-s");
                
            if (help)
            {
                Console.WriteLine("Option:");
                Console.WriteLine("  -s\tShow simply");
            }
            else Run(simple);
        }

        static void Run(bool simple)
        {
            var lines = RunCommand("ipconfig");
            var results = ParseIP(lines);

            foreach (var result in results)
            {
                if (simple) Console.WriteLine(result.IP);
                else Console.WriteLine($"{result.IP,-16}{result.Name}");
            }
        }

        static List<IPName> ParseIP(string[] lines)
        {
            Regex reName = new(
                @"^(?! )(.+):",
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline
            );
            Regex reIPv4 = new(
                @"^.*IPv4.+(\.| )+: *(?'IP'\d+\.\d+\.\d+\.\d+).*$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline
            );

            List<IPName> results = new();
            IPName? ipName = null;
            foreach (var line in lines)
            {
                if (reName.IsMatch(line))
                {
                    var match = reName.Match(line);
                    ipName = new()
                    {
                        Name = match.Groups[1].ToString(),
                    };
                }
                else if (ipName != null && reIPv4.IsMatch(line))
                {
                    var match = reIPv4.Match(line);
                    ipName.IP = match.Groups[2].ToString();

                    results.Add(ipName);
                }
            }

            return results;
        }

        static string[] RunCommand(string name, string args="")
        {
            Process p = new();

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = name;
            p.StartInfo.Arguments = args;
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return output.Split('\n');
        }
    }
}
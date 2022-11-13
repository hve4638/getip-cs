using System.Diagnostics;
using System.Text.RegularExpressions;

namespace getip
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var output = RunCommand("ipconfig");
            var ip = parseIP(output);

            Console.WriteLine(ip);
        }

        static string RunCommand(string name, string args="")
        {
            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = name;
            p.StartInfo.Arguments = args;
            p.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            return output;
        }

        static string parseIP(string text)
        {
            string output = "";
            Regex rx = new Regex(
                @"^.*IPv4.+(\.| )+: *(?'IP'\d+\.\d+\.\d+\.\d+).*$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline
            );

            MatchCollection matches = rx.Matches(text);
            foreach (Match match in matches)
            {
                output += $"{match.Groups[2]}\n";
            }
            
            return output.Trim();
        }
    }
}
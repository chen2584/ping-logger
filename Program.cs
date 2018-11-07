using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Serilog;
using Serilog.Core;

namespace PingLogger
{
    class Program
    {
        private static Logger logger;
        static void Main(string[] args)
        {
            QuickEdit.Disable();
            Console.WriteLine("===== The Amazing Ping Logger =====");
            Console.Write("Input IP: ");
            string ip = Console.ReadLine();

            var pathToExe = Assembly.GetEntryAssembly().Location;
            var pathToContentRoot = Path.GetDirectoryName(pathToExe);
            logger = new LoggerConfiguration()
                     .WriteTo.Console()
                     .WriteTo.File(Path.Combine(pathToContentRoot, "logs/.log"), rollingInterval: RollingInterval.Day)
                     .CreateLogger();

            Ping(ip);
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }

        static void Ping(string ip)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"cmd.exe",
                    Arguments = $"/c ping -t {ip}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                },
            };
            process.OutputDataReceived += new DataReceivedEventHandler((s, e) => { logger.Information(e.Data); });
            process.ErrorDataReceived += new DataReceivedEventHandler((s, e) => { logger.Error(e.Data); });
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            process.Close();
        }
    }
}

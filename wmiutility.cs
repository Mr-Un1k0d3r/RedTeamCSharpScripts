using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Threading;

namespace WMIUtility
{
    class Program
    {
        static string RunRemoteQuery(SelectQuery query, string target = "", string remoteHost = "", string username = "", string password = "")
        {
            StringBuilder sb = new StringBuilder();
            ConnectionOptions con = new ConnectionOptions();
            con.Username = username;
            con.Password = password;
            ManagementScope ms = new ManagementScope($"\\\\{remoteHost}\\root\\cimv2", con);
            try
            {
                ms.Connect();
                bool isConnected = ms.IsConnected;
                if (isConnected)
                {
                    Console.WriteLine($"[+] Connected to: {remoteHost}");
                    Console.WriteLine($"[+] Querying:     {query.QueryString}");
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher(ms, query);
                    ManagementObjectCollection entries = searcher.Get();
                    if (query.QueryString.Contains("Win32_NTLogEvent"))
                    {
                        Console.WriteLine($"[+] Searching 4624 events for {target}, this may take a while...");
                        foreach (ManagementBaseObject entry in entries)
                        {
                            if (entry["Message"].ToString().Contains("Source Network Address"))
                            {
                                foreach (string item in entry["Message"].ToString().Split('\n'))
                                {
                                    if (item.Contains("Source Network Address:"))
                                    {
                                        if (!sb.ToString().Contains($"[!] Previously logged on: {item.Split(':')[1].Trim()}") && item.Split(':')[1].Trim() != "-")
                                        {
                                            sb.Append($"[!] Previously logged on: {item.Split(':')[1].Trim()}\r\n");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (ManagementBaseObject item in new ManagementObjectSearcher(ms, query).Get())
                        {
                            foreach (string column in target.Split(','))
                            {
                                sb.Append(column + new string(' ', 20 - column.Length) + ": ");
                                sb.Append(item[column] + "\r\n");
                            }
                            sb.Append("\r\n");
                        }
                    }
                    return sb.ToString();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return sb.ToString();
        }
        static string RunLocalQuery(SelectQuery query, string columns)
        {
            Console.WriteLine($"Querying:      {query.QueryString}");
            StringBuilder sb = new StringBuilder();
            foreach (ManagementBaseObject item in new ManagementObjectSearcher(query).Get())
            {
                foreach (string column in columns.Split(','))
                {
                    sb.Append(column + new string(' ', 20 - column.Length) + ": ");
                    sb.Append(item[column] + "\r\n");
                }
                sb.Append("\r\n");
            }
            return sb.ToString();
        }
        static void Main(string[] args)
        {
            if (args.Length >= 1)
            {
                string arg1 = args[0].ToLower();
                string arg2 = args.Length >= 2 ? args[1] : "";
                string arg3 = args.Length >= 3 ? args[2] : "";
                string arg4 = args.Length >= 4 ? args[3] : "";
                string arg5 = args.Length >= 5 ? args[4] : "";
                string arg6 = args.Length >= 6 ? args[5] : "";

                if (arg1 == "listprocess")
                {
                    Console.WriteLine(RunLocalQuery(new SelectQuery("Select * From Win32_Process"), "CreationDate,ProcessId,ExecutablePath,CommandLine"));
                }
                if (arg1 == "listremoteprocess")
                {
                    Console.WriteLine(RunRemoteQuery(new SelectQuery("Select * From Win32_Process"), "CreationDate,ProcessId,ExecutablePath,CommandLine", arg2, arg3, arg4));
                }
                else if (arg1 == "listservice")
                {
                    Console.WriteLine(RunLocalQuery(new SelectQuery("Select * From Win32_Service"), "Name,ProcessId,Description,DisplayName,State,Status,PathName"));
                }
                else if (arg1 == "listremoteservice")
                {
                    Console.WriteLine(RunRemoteQuery(new SelectQuery("Select * From Win32_Service"), "Name,ProcessId,Description,DisplayName,State,Status,PathName", arg2, arg3, arg4));
                }
                else if (arg1 == "get-eventforuser")
                {
                    Console.WriteLine(RunRemoteQuery(new SelectQuery($"Select * from Win32_NTLogEvent Where Logfile='Security' and EventCode='4624' and Message Like '%{arg2}%'"), arg2, arg3, arg4, arg5));
                }
                else if (arg1 == "remotequery")
                {
                    Console.WriteLine(RunRemoteQuery(new SelectQuery(arg2), arg3, arg4, arg5, arg6));
                }
                else if (arg1 == "query")
                {
                    Console.WriteLine(RunLocalQuery(new SelectQuery(arg2), arg3));
                }
                else
                {
                    Console.WriteLine($"Invalid argument: {arg1} not found");
                }
            }
            else
            {
                Console.WriteLine("ERROR: missing arguments");
                Console.WriteLine($"Usage: {System.Reflection.Assembly.GetExecutingAssembly().Location} options [arguments]");
            }
        }
    }
}

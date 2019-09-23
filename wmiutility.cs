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
        static string RunQuery(string query, string columns)
        {
            Console.WriteLine("Querying:      {0}", query);
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
                string option = args[0].ToLower();
                string query = args.Length >= 2 ? args[1] : "";
                string columns = args.Length >= 3 ? args[2] : "";
                if (option == "listprocess")
                {
                    Console.WriteLine(RunQuery("Select * From Win32_Process", "CreationDate,ProcessId,ExecutablePath,CommandLine"));
                }
                else if (option == "listservice")
                {
                    Console.WriteLine(RunQuery("Select * From Win32_Service", "Name,ProcessId,Description,DisplayName,State,Status,PathName"));
                }
                else if (option == "query")
                {
                    Console.WriteLine(RunQuery(query, columns));
                }
                else
                {
                    Console.WriteLine("Invalid argument: {0} not found", option);
                }
            }
            else
            {
                Console.WriteLine("ERROR: missing arguments");
                Console.WriteLine("Usage: {0} options [arguments]", System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
        }
    }
}

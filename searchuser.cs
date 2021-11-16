using System;
using System.Xml;
using System.Net;
using System.Reflection;
using System.Globalization;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Security.Principal;
using System.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;

namespace EventLogParser
{
    public enum Options
    {
        username = 0,
        domain = 1,
        ip = 2
    }
    class Program
    {
        static void Main(string[] args)
        {
            string[] queryString = new string[]
            {
                "*[System[(EventID=4624)] and EventData[Data[@Name=\"TargetUserName\"]=\"{0}\"]]",
                "*[System[(EventID=4624)] and EventData[Data[@Name=\"TargetDomainName\"]=\"{0}\"]]",
                "*[EventData[Data[@Name=\"IpAddress\"] and(Data=\"{0}\")]]"
            };
            string search = args[1];
            if(!Enum.IsDefined(typeof(Options), args[0])) {
                Console.WriteLine("Invalid Option: username, domain, ip");
                return;
            }
            Console.WriteLine("Searching for '{0}'", search);

            int index = (int)Enum.Parse(typeof(Options), args[0]);
            
            string query = String.Format(queryString[index], search);

            Console.WriteLine("Querying: {0}", query);
            foreach (DomainController target in Domain.GetCurrentDomain().DomainControllers)
            {
                try
                {
                    Console.WriteLine("Parsing {0} ({1}) logs", target.IPAddress, target.Name);
                    EventLogSession els = new EventLogSession(target.Name);

                    EventLogQuery logQuery = new EventLogQuery("Security", PathType.LogName, query);
                    logQuery.Session = els;

                    EventLogReader elr = new EventLogReader(logQuery);
                    while (true)
                    {
                        EventRecord er = elr.ReadEvent();

                        if (er == null)
                        {
                            break;
                        }
                        Console.WriteLine(er.FormatDescription() + "\r\n-----------------------------------\r\n");

                        if(er != null)
                        {
                            er.Dispose();
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }
        }
    }
}

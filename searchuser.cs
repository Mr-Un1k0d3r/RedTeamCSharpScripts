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
    class Program
    {
        static void Main(string[] args)
        {
            string username = args[0];
            Console.WriteLine("Searching for user '{0}'", username);
            string xmlQuery = String.Format("<QueryList><Query Id=\"0\" Path=\"Security\"><Select Path=\"Security\">*[System[(EventID=4624)] and EventData[Data[@Name=\"TargetDomainName\"]=\"{0}\"]]</Select></Query></QueryList>", username);
            foreach (DomainController target in Domain.GetCurrentDomain().DomainControllers)
            {
                try
                {
                    Console.WriteLine("Parsing {0} ({1})", target.IPAddress, target.Name);
                    EventLogSession els = new EventLogSession(target.Name);

                    EventLogQuery logQuery = new EventLogQuery(null, PathType.LogName, xmlQuery);
                    logQuery.Session = els;

                    EventLogReader elr = new EventLogReader(logQuery);
                    while (true)
                    {
                        EventRecord er = elr.ReadEvent();

                        if (er == null)
                        {
                            break;
                        }
                        Console.WriteLine(er.FormatDescription());
                    }

                } catch(Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }
        }
    }
}

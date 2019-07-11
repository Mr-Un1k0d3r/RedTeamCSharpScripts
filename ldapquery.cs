using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;

namespace LdapQuery
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length >= 3)
            {
                string domain = args[0];
                string filter = args[1];
                string properties = args[2];

                Console.WriteLine("Querying LDAP://{0}", domain);
                Console.WriteLine("Querying: {0}", filter);
                Console.WriteLine("Extracting: {0}", properties);
                DirectoryEntry de = new DirectoryEntry("LDAP://" + domain);
                DirectorySearcher ds = new DirectorySearcher(de);

                ds.Filter = filter;
                
                foreach (SearchResult r in ds.FindAll())
                {
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (string prop in properties.Split(','))
                        {
                            sb.Append(r.Properties[prop].Count > 0 ? r.Properties[prop][0] : String.Empty);
                            sb.Append(",");
                        }
                        Console.WriteLine(sb.ToString());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR: {0}", e.Message);
                    }
                }
            }
            else
            {
                Console.WriteLine("Missing arguments\nUsage: domain query properties,...");
            }
        }
    }
}

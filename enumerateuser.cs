using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;

namespace ldap
{
    class Program
    {
        static void Main(string[] args)
        {
            string domain = args[0];
            string q = "(&(objectCategory=User))";

            Console.WriteLine("Querying LDAP://{0}", domain);
            Console.WriteLine("Querying: {0}", q);
            DirectoryEntry de = new DirectoryEntry("LDAP://" + domain);
            DirectorySearcher ds = new DirectorySearcher(de);
            
            ds.Filter = q;
            foreach(SearchResult r in ds.FindAll()) {
                try {
                    Console.WriteLine("{0}:{1}",
                        r.Properties["samaccountname"].Count > 0 ? r.Properties["samaccountname"][0] : String.Empty, 
                        r.Properties["mail"].Count > 0 ? r.Properties["mail"][0] : String.Empty);
                } catch (Exception e) {
                    Console.WriteLine("ERROR: {0}", e.Message);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.DirectoryServices;

namespace LdapUtility
{

    class Program
    {
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr a, UInt32 b);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        static string FormatProperties(ResultPropertyValueCollection r)
        {
            StringBuilder sb = new StringBuilder();
            Int32 size = r.Count;
            for (Int32 i = 0; i < size; i++)
            {
                sb.Append(r[i] + ",");
            }
            return sb.ToString().TrimEnd(',');
        }

        static string FormatTime(object p)
        {
            // I will assume that all Int64 are timestamp
            if (p.GetType().ToString() == "System.Int64")
            {
                return DateTime.FromFileTime((long)p).ToString();
            }
            return p.ToString(); ;
        }

        static void LdapQuery(string domain, string query, string properties)
        {
            Console.WriteLine("Connecting to: LDAP://{0}", domain);
            Console.WriteLine("Querying:      {0}", query);

            DirectoryEntry de = new DirectoryEntry("LDAP://" + domain);
            DirectorySearcher ds = new DirectorySearcher(de);

            ds.Filter = query;
            foreach (SearchResult r in ds.FindAll())
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (string prop in properties.Split(','))
                    {
                        sb.Append(prop + new string(' ', 20 - prop.Length) + ": ");
                        Int32 item = r.Properties[prop].Count;
                        if (item > 0)
                        {
                            sb.Append(item > 1 ? "[" + FormatProperties(r.Properties[prop]) + "]" : FormatTime(r.Properties[prop][0]));
                        }
                        sb.Append("\r\n");
                    }
                    Console.WriteLine(sb.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: {0}", e.Message);
                }
            }
        }

        static void Main(string[] args)
        {
            // ShowWindow(GetConsoleWindow(), 0);
            if (args.Length >= 2)
            {
                string option = args[0];
                string domain = args[1];

                if (option == "DumpAllUsers")
                {
                    string query = "";
                    string properties = "name,givenname,displayname,samaccountname,adspath,distinguishedname,memberof,ou,mail,proxyaddresses,lastlogon,pwdlastset,mobile,streetaddress,co,title,department,description,comment,badpwdcount,objectcategory,userpassword";
                    try
                    {
                        query = "(&(objectClass=user))";
                        LdapQuery(domain, query, properties);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR: DumpAllUsers catched an unexpected exception");
                    } 
                }
                else if (option == "DumpUser")
                {
                    string query = "";
                    string properties = "name,givenname,displayname,samaccountname,adspath,distinguishedname,memberof,ou,mail,proxyaddresses,lastlogon,pwdlastset,mobile,streetaddress,co,title,department,description,comment,badpwdcount,objectcategory,userpassword"; 
                    try
                    {
                        query = "(&(objectClass=user)(samaccountname=*" + args[2] + "*))";
                        LdapQuery(domain, query, properties);
                    } catch(Exception e) {
                        Console.WriteLine("ERROR: DumpUser required a user argument");
                    } 
                }
                else if (option == "DumpUsersEmail")
                {
                    string query = "";
                    string properties = "name,samaccountname,mail";
                    try
                    {
                        query = "(&(objectClass=user))";
                        LdapQuery(domain, query, properties);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR: DumpUsersEmail catched an unexpected exception");
                    } 
                }
                else if (option == "DumpAllComputers")
                {
                    string query = "";
                    string properties = "name,displayname,operatingsystem,description,adspath,objectcategory,serviceprincipalname,distinguishedname,cn,lastlogon";
                    try
                    {
                        query = "(&(objectClass=computer))";
                        LdapQuery(domain, query, properties);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR: DumpAllComputers catched an unexpected exception");
                    } 
                }
                else if (option == "DumpComputer")
                {
                    string query = "";
                    string properties = "name,displayname,operatingsystem,description,adspath,objectcategory,serviceprincipalname,distinguishedname,cn,lastlogon";
                    try
                    {
                        query = "(&(objectClass=computer)(name=*" + args[2] + "))";
                        LdapQuery(domain, query, properties);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR: DumpComputer required a computer name argument");
                    } 
                }
                else if (option == "DumpAllGroups")
                {
                    string query = "";
                    string properties = "name,adspath,distinguishedname,member,memberof";
                    try
                    {
                        query = "(&(objectClass=group))";
                        LdapQuery(domain, query, properties);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR: DumpAllGroups required a computer name argument");
                    } 
                }
                else if (option == "DumpGroup")
                {
                    string query = "";
                    string properties = "name,adspath,distinguishedname,member,memberof";
                    try
                    {
                        query = "(&(objectClass=group)(name=*" + args[2] + "))";
                        LdapQuery(domain, query, properties);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR: DumpGroup required a group name argument");
                    } 
                }
                else if (option == "DumpPasswordPolicy")
                {
                    string query = "";
                    string properties = "name,msDS-MinimumPasswordLength,msDS-PasswordHistoryLength,msDS-PasswordComplexityEnabled,msDS-PasswordReversibleEncryptionEnabled,msDS-LockoutThreshold,msDS-PasswordSettingsPrecedence";
                    try
                    {
                        query = "(&(objectClass=msDS-PasswordSettings))";
                        LdapQuery(domain, query, properties);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR: DumpPasswordPolicy catched an unexpected exception");
                    } 
                }
                else
                {
                    Console.WriteLine("Invalid argument: {0} not found", option);
                }
            }
            else
            {
                Console.WriteLine("ERROR: missing arguments");
                Console.WriteLine("Usage: {0} options domain [arguments]", System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
        }
    }
}

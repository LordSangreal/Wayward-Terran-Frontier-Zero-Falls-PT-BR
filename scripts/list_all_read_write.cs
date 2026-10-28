using System;
using System.IO;
using System.Linq;
using Mono.Cecil;

class Program
{
    static void Main()
    {
        string exePath = @"D:\SteamLibrary\steamapps\common\Zero Falls\WTF_backup.exe";
        var asm = AssemblyDefinition.ReadAssembly(exePath);

        foreach (var t in asm.MainModule.Types)
        {
            foreach (var m in t.Methods)
            {
                string n = m.Name.ToLowerInvariant();
                if (n.StartsWith("read") || n.StartsWith("write"))
                {
                    Console.WriteLine(string.Format("{0}::{1}", t.Name, m.Name));
                }
            }
        }
    }
}

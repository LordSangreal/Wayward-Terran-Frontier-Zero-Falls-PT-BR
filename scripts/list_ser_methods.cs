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

        string[] types = new string[] { "TriggerEvent", "AGENTBAG", "AgentTracker", "GameFile" };
        foreach (var tname in types)
        {
            var t = asm.MainModule.Types.FirstOrDefault(x => x.Name == tname);
            if (t == null) continue;
            Console.WriteLine("=== " + tname + " ===");
            foreach (var m in t.Methods)
            {
                Console.WriteLine("  " + m.Name);
            }
        }
    }
}

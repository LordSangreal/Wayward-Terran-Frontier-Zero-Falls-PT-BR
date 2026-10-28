using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

class Program
{
    static void Main()
    {
        string exePath = @"D:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe";
        var asm = AssemblyDefinition.ReadAssembly(exePath);

        var agentTracker = asm.MainModule.Types.FirstOrDefault(t => t.Name == "AgentTracker");
        if (agentTracker != null)
        {
            var m = agentTracker.Methods.FirstOrDefault(x => x.Name == "readData2");
            if (m != null)
            {
                Console.WriteLine("=== AgentTracker.readData2 ===");
                foreach (var i in m.Body.Instructions)
                {
                    Console.WriteLine(string.Format("{0} {1}", i.OpCode, i.Operand));
                }
            }
        }
    }
}

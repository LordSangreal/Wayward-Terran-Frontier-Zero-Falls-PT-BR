using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

class Program
{
    static void Main()
    {
        string exePath = @"D:\SteamLibrary\steamapps\common\Zero Falls\WTF_backup.exe";
        var asm = AssemblyDefinition.ReadAssembly(exePath);

        var q = asm.MainModule.Types.FirstOrDefault(t => t.Name == "FirstBattleQuest");
        if (q != null)
        {
            var m = q.Methods.FirstOrDefault(x => x.Name == "doBreadcrumbConvo");
            if (m != null)
            {
                Console.WriteLine("=== FirstBattleQuest.doBreadcrumbConvo ===");
                foreach (var i in m.Body.Instructions)
                {
                    Console.WriteLine(string.Format("{0:X4}: {1} {2}", i.Offset, i.OpCode, i.Operand));
                }
            }
        }
    }
}

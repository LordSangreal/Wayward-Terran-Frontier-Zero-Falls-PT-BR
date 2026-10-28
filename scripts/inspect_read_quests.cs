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

        var gameFile = asm.MainModule.Types.FirstOrDefault(t => t.Name == "GameFile");
        if (gameFile != null)
        {
            var m = gameFile.Methods.FirstOrDefault(x => x.Name == "readQuests3");
            if (m != null)
            {
                Console.WriteLine("=== GameFile.readQuests3 ===");
                for (int i = 0; i < m.Body.Instructions.Count; i++)
                {
                    var inst = m.Body.Instructions[i];
                    Console.WriteLine(string.Format("{0:X4}: {1} {2}", inst.Offset, inst.OpCode, inst.Operand));
                }
            }
        }
    }
}

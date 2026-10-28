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

        var t = asm.MainModule.Types.FirstOrDefault(x => x.Name == "BuyAShipTutorial");
        var m = t.Methods.FirstOrDefault(x => x.Name == "buildStages");

        Console.WriteLine("Instructions in buildStages:");
        for (int i = 0; i < m.Body.Instructions.Count; i++)
        {
            var inst = m.Body.Instructions[i];
            if (inst.OpCode == OpCodes.Ldsfld || inst.OpCode == OpCodes.Call || inst.OpCode == OpCodes.Callvirt || inst.OpCode == OpCodes.Newobj)
            {
                Console.WriteLine(string.Format("{0:X4}: {1} {2}", inst.Offset, inst.OpCode, inst.Operand));
            }
        }
    }
}

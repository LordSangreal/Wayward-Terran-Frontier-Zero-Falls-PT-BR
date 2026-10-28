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
        if (t != null)
        {
            var m = t.Methods.FirstOrDefault(x => x.Name == ".ctor" && x.Parameters.Count == 1 && x.Parameters[0].ParameterType.Name == "BinaryReader");
            if (m != null)
            {
                Console.WriteLine("=== BuyAShipTutorial..ctor(BinaryReader) ===");
                for (int i = 0; i < m.Body.Instructions.Count; i++)
                {
                    var inst = m.Body.Instructions[i];
                    Console.WriteLine(string.Format("{0:X4}: {1} {2}", inst.Offset, inst.OpCode, inst.Operand));
                }
            }
        }
    }
}

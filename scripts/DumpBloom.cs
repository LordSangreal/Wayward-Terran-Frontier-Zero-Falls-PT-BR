using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

class DumpBloom
{
    static void Main()
    {
        var asmOrig = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");
        var asmPatch = AssemblyDefinition.ReadAssembly(@"D:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe");

        foreach (var t in asmOrig.MainModule.Types)
        {
            if (t.Name.Contains("Bloom"))
            {
                foreach (var m in t.Methods)
                {
                    Console.WriteLine("ORIG " + t.Name + "." + m.Name);
                    if (m.HasBody)
                    {
                        foreach (var i in m.Body.Instructions)
                        {
                            if (i.OpCode == OpCodes.Ldstr)
                                Console.WriteLine("  ORIG: " + (string)i.Operand);
                        }
                    }
                }
            }
        }

        Console.WriteLine("\n--- PATCHED ---");
        foreach (var t in asmPatch.MainModule.Types)
        {
            if (t.Name.Contains("Bloom"))
            {
                foreach (var m in t.Methods)
                {
                    Console.WriteLine("PATCH " + t.Name + "." + m.Name);
                    if (m.HasBody)
                    {
                        foreach (var i in m.Body.Instructions)
                        {
                            if (i.OpCode == OpCodes.Ldstr)
                                Console.WriteLine("  PATCH: " + (string)i.Operand);
                        }
                    }
                }
            }
        }
    }
}

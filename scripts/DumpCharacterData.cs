using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

class DumpCharacterData
{
    static void Main()
    {
        var asmOrig = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");
        var asmPatch = AssemblyDefinition.ReadAssembly(@"D:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe");

        foreach (var t in asmOrig.MainModule.Types)
        {
            if (t.Name == "CHARACTER_DATA")
            {
                foreach (var m in t.Methods)
                {
                    if (m.Name == "loadMenuProfile" || m.Name == "init")
                    {
                        Console.WriteLine("================ METHOD: " + m.Name + " ================");
                        foreach (var i in m.Body.Instructions)
                        {
                            if (i.OpCode == OpCodes.Ldstr)
                            {
                                Console.WriteLine("  ORIG: " + (string)i.Operand);
                            }
                        }
                    }
                }
            }
        }

        Console.WriteLine("\n--- PATCHED VERSION ---");
        foreach (var t in asmPatch.MainModule.Types)
        {
            if (t.Name == "CHARACTER_DATA")
            {
                foreach (var m in t.Methods)
                {
                    if (m.Name == "loadMenuProfile" || m.Name == "init")
                    {
                        Console.WriteLine("================ METHOD: " + m.Name + " ================");
                        foreach (var i in m.Body.Instructions)
                        {
                            if (i.OpCode == OpCodes.Ldstr)
                            {
                                Console.WriteLine("  PATCH: " + (string)i.Operand);
                            }
                        }
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

class DiffGame1
{
    static void Main()
    {
        var asmOrig = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");
        var asmPatch = AssemblyDefinition.ReadAssembly(@"D:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe");

        var origMethod = FindMethod(asmOrig, "Game1", "loadStuff");
        var patchMethod = FindMethod(asmPatch, "Game1", "loadStuff");

        int diffs = 0;
        for (int i = 0; i < origMethod.Body.Instructions.Count; i++)
        {
            var iOrig = origMethod.Body.Instructions[i];
            var iPatch = patchMethod.Body.Instructions[i];

            if (iOrig.OpCode == OpCodes.Ldstr)
            {
                string sOrig = (string)iOrig.Operand;
                string sPatch = (string)iPatch.Operand;
                if (sOrig != sPatch)
                {
                    Console.WriteLine("DIFF in Game1.loadStuff: [" + sOrig + "] -> [" + sPatch + "]");
                    diffs++;
                }
            }
        }
        Console.WriteLine("Total diffs in Game1.loadStuff: " + diffs);
    }

    static MethodDefinition FindMethod(AssemblyDefinition asm, string typeName, string methodName)
    {
        foreach (var t in asm.MainModule.Types)
        {
            if (t.Name == typeName)
            {
                foreach (var m in t.Methods)
                {
                    if (m.Name == methodName) return m;
                }
            }
        }
        return null;
    }
}

using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

class CheckSteamUsage
{
    static void Main()
    {
        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");
        var steamCalls = new HashSet<string>();

        foreach (var t in asm.MainModule.Types)
        {
            ScanType(t, steamCalls);
        }

        Console.WriteLine("Steam Calls found: " + steamCalls.Count);
        foreach (var c in steamCalls)
        {
            Console.WriteLine("  CALL: " + c);
        }
    }

    static void ScanType(TypeDefinition t, HashSet<string> set)
    {
        foreach (var m in t.Methods)
        {
            if (!m.HasBody) continue;
            foreach (var i in m.Body.Instructions)
            {
                if (i.OpCode == OpCodes.Call || i.OpCode == OpCodes.Callvirt)
                {
                    var mr = i.Operand as MethodReference;
                    if (mr != null && mr.DeclaringType.FullName.Contains("Steam"))
                    {
                        set.Add(mr.DeclaringType.FullName + "::" + mr.Name + " (in " + t.Name + "." + m.Name + ")");
                    }
                }
            }
        }
        foreach (var nested in t.NestedTypes)
        {
            ScanType(nested, set);
        }
    }
}

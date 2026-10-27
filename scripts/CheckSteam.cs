using System;
using Mono.Cecil;

class CheckSteam
{
    static void Main()
    {
        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");

        Console.WriteLine("=== Scanning Steam References in WTF.exe ===");
        foreach (var an in asm.MainModule.AssemblyReferences)
        {
            if (an.Name.IndexOf("Steam", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Console.WriteLine("Assembly Ref: " + an.FullName);
            }
        }

        foreach (var t in asm.MainModule.Types)
        {
            if (t.Name.IndexOf("Steam", StringComparison.OrdinalIgnoreCase) >= 0 ||
                t.Name.IndexOf("Workshop", StringComparison.OrdinalIgnoreCase) >= 0 ||
                t.Name.IndexOf("UGC", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Console.WriteLine("Type: " + t.FullName);
                foreach (var m in t.Methods)
                {
                    Console.WriteLine("  Method: " + m.Name);
                }
            }
        }
    }
}

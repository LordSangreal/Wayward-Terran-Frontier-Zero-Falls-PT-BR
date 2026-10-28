using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

class Program
{
    static void Main(string[] args)
    {
        string exePath = @"D:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe";
        if (args.Length > 0) exePath = args[0];

        var asm = AssemblyDefinition.ReadAssembly(exePath);
        var types = asm.MainModule.Types.ToList();

        Console.WriteLine("Total types: " + types.Count);

        // Find interesting types
        var interesting = types.Where(t => 
            t.Name.IndexOf("item", StringComparison.OrdinalIgnoreCase) >= 0 ||
            t.Name.IndexOf("loot", StringComparison.OrdinalIgnoreCase) >= 0 ||
            t.Name.IndexOf("interact", StringComparison.OrdinalIgnoreCase) >= 0 ||
            t.Name.IndexOf("door", StringComparison.OrdinalIgnoreCase) >= 0 ||
            t.Name.IndexOf("airlock", StringComparison.OrdinalIgnoreCase) >= 0 ||
            t.Name.IndexOf("console", StringComparison.OrdinalIgnoreCase) >= 0 ||
            t.Name.IndexOf("bed", StringComparison.OrdinalIgnoreCase) >= 0 ||
            t.Name.IndexOf("terminal", StringComparison.OrdinalIgnoreCase) >= 0 ||
            t.Name.IndexOf("log", StringComparison.OrdinalIgnoreCase) >= 0 ||
            t.Name.IndexOf("hud", StringComparison.OrdinalIgnoreCase) >= 0 ||
            t.Name.IndexOf("manifest", StringComparison.OrdinalIgnoreCase) >= 0 ||
            t.Name.IndexOf("prompt", StringComparison.OrdinalIgnoreCase) >= 0 ||
            t.Name.IndexOf("message", StringComparison.OrdinalIgnoreCase) >= 0
        ).Select(t => t.FullName).ToList();

        Console.WriteLine("=== INTERESTING TYPES ===");
        foreach (var t in interesting)
        {
            Console.WriteLine(t);
        }

        // Search for specific phrases in all string literals
        string[] searchTerms = new string[] {
            "airlock", "sleep", "locked", "connected", "undock", "hangar", "cargo",
            "inventory", "console", "terminal", "access", "interact", "press [", "press <",
            "cannot", "deposit", "store", "research"
        };

        Console.WriteLine("\n=== METHODS CONTAINING INTERACTION STRINGS ===");
        var foundStrings = new HashSet<string>();

        foreach (var t in types)
        {
            foreach (var m in t.Methods)
            {
                if (!m.HasBody) continue;
                foreach (var inst in m.Body.Instructions)
                {
                    if (inst.OpCode == OpCodes.Ldstr)
                    {
                        string s = inst.Operand as string;
                        if (string.IsNullOrEmpty(s)) continue;

                        foreach (var term in searchTerms)
                        {
                            if (s.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                if (foundStrings.Add(s))
                                {
                                    Console.WriteLine(string.Format("[{0}::{1}] {2}", t.Name, m.Name, s.Replace("\n", "\\n")));
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }
        Console.WriteLine("Total matched strings: " + foundStrings.Count);
    }
}

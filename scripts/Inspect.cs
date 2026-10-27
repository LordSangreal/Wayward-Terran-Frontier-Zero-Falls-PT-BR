using System;
using System.IO;
using Mono.Cecil;
using Mono.Cecil.Cil;

class InspectStrings
{
    static void Main(string[] args)
    {
        string exePath = @"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe";
        var assembly = AssemblyDefinition.ReadAssembly(exePath);

        foreach (var type in assembly.MainModule.Types)
        {
            if (type.FullName.Contains("Tutorial") || type.FullName.Contains("FirstFlight") || type.FullName.Contains("OneAgent") || type.FullName.Contains("Intro"))
            {
                InspectType(type);
            }
        }
    }

    static void InspectType(TypeDefinition type)
    {
        foreach (var method in type.Methods)
        {
            if (!method.HasBody) continue;
            bool printedHeader = false;

            foreach (var instr in method.Body.Instructions)
            {
                if (instr.OpCode == OpCodes.Ldstr)
                {
                    string s = (string)instr.Operand;
                    if (!string.IsNullOrEmpty(s) && s.Length > 2)
                    {
                        if (!printedHeader)
                        {
                            Console.WriteLine("========================================");
                            Console.WriteLine("Method: " + type.FullName + " :: " + method.Name);
                            Console.WriteLine("========================================");
                            printedHeader = true;
                        }
                        Console.WriteLine("  LDSTR: " + s);
                    }
                }
            }
        }

        foreach (var nested in type.NestedTypes)
        {
            InspectType(nested);
        }
    }
}

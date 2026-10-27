using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

class FindShaderStrings
{
    static void Main()
    {
        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");
        var shaderStrings = new HashSet<string>();

        foreach (var type in asm.MainModule.Types)
        {
            FindInType(type, shaderStrings);
        }

        Console.WriteLine("Total shader technique/parameter strings identified: " + shaderStrings.Count);
        foreach (var s in shaderStrings)
        {
            Console.WriteLine("  SHADER STRING: " + s);
        }
    }

    static void FindInType(TypeDefinition type, HashSet<string> set)
    {
        foreach (var m in type.Methods)
        {
            if (!m.HasBody) continue;
            for (int i = 0; i < m.Body.Instructions.Count; i++)
            {
                var instr = m.Body.Instructions[i];
                if (instr.OpCode == OpCodes.Call || instr.OpCode == OpCodes.Callvirt)
                {
                    var mr = instr.Operand as MethodReference;
                    if (mr != null)
                    {
                        if (mr.DeclaringType.Name.Contains("Technique") || mr.DeclaringType.Name.Contains("Parameter"))
                        {
                            // Previous instruction might be ldstr
                            if (i > 0 && m.Body.Instructions[i - 1].OpCode == OpCodes.Ldstr)
                            {
                                string str = (string)m.Body.Instructions[i - 1].Operand;
                                set.Add(str);
                            }
                        }
                    }
                }
            }
        }

        foreach (var nested in type.NestedTypes)
        {
            FindInType(nested, set);
        }
    }
}

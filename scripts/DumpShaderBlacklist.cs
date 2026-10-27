using System;
using System.IO;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

class DumpShaderBlacklist
{
    static void Main()
    {
        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");
        var shaderStrings = new HashSet<string>();

        foreach (var type in asm.MainModule.Types)
        {
            FindInType(type, shaderStrings);
        }

        using (var sw = new StreamWriter(@"d:\Tradução WTF Zero falls\scripts\shader_blacklist.json", false, System.Text.Encoding.UTF8))
        {
            sw.WriteLine("[");
            int i = 0;
            foreach (var s in shaderStrings)
            {
                sw.Write("  \"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"");
                if (i < shaderStrings.Count - 1) sw.WriteLine(",");
                else sw.WriteLine();
                i++;
            }
            sw.WriteLine("]");
        }
        Console.WriteLine("Blacklist saved: " + shaderStrings.Count + " strings.");
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

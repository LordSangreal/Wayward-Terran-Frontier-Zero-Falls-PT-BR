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
        var asm = AssemblyDefinition.ReadAssembly(exePath);

        var getTips = new List<string>();

        foreach (var t in asm.MainModule.Types)
        {
            foreach (var m in t.Methods)
            {
                if (m.Name.Equals("getTip", StringComparison.OrdinalIgnoreCase) ||
                    m.Name.IndexOf("tip", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    m.Name.IndexOf("tooltip", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    m.Name.IndexOf("prompt", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (!m.HasBody) continue;
                    var ldstrs = m.Body.Instructions
                        .Where(i => i.OpCode == OpCodes.Ldstr)
                        .Select(i => (string)i.Operand)
                        .Where(s => !string.IsNullOrEmpty(s) && s.Trim().Length > 1)
                        .Distinct()
                        .ToList();

                    if (ldstrs.Count > 0)
                    {
                        foreach (var s in ldstrs)
                        {
                            getTips.Add(string.Format("{0}::{1} => {2}", t.Name, m.Name, s.Replace("\n", "\\n")));
                        }
                    }
                }
            }
        }

        Console.WriteLine("Total tip strings: " + getTips.Count);
        File.WriteAllLines("all_tips.txt", getTips);
        foreach (var line in getTips)
        {
            Console.WriteLine(line);
        }
    }
}

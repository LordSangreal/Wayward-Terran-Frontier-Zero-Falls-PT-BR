using System;
using System.IO;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class InspectDbStrings {
    public static void Main(string[] args) {
        string origPath = @"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe";
        var asm = AssemblyDefinition.ReadAssembly(origPath);

        foreach (var t in asm.MainModule.Types) {
            if (t.FullName == "CoOpSpRpG.CHARACTER_DATA" || t.FullName == "CoOpSpRpG.WorldDatabase") {
                Console.WriteLine("================ TYPE: " + t.FullName + " ================");
                foreach (var m in t.Methods) {
                    if (!m.HasBody) continue;
                    var strs = new List<string>();
                    foreach (var i in m.Body.Instructions) {
                        if (i.OpCode == OpCodes.Ldstr) {
                            strs.Add((string)i.Operand);
                        }
                    }
                    if (strs.Count > 0) {
                        Console.WriteLine("Method: " + m.Name);
                        foreach (var s in strs) {
                            Console.WriteLine("   [" + s + "]");
                        }
                    }
                }
            }
        }
    }
}

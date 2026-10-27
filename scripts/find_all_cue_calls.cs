using System;
using System.IO;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class FindAllCueCalls {
    public static void Main() {
        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");
        foreach (var t in asm.MainModule.Types) {
            CheckType(t);
        }
    }

    static void CheckType(TypeDefinition t) {
        foreach (var m in t.Methods) {
            if (!m.HasBody) continue;
            bool callsAudio = false;
            foreach (var instr in m.Body.Instructions) {
                var mr = instr.Operand as MethodReference;
                if (mr != null && (mr.Name == "GetCue" || mr.Name == "PlayCue")) {
                    callsAudio = true;
                    break;
                }
            }
            if (callsAudio) {
                Console.WriteLine("Audio method: " + t.FullName + "::" + m.Name);
                foreach (var instr in m.Body.Instructions) {
                    if (instr.OpCode == OpCodes.Ldstr) {
                        Console.WriteLine("   ldstr: [" + instr.Operand + "]");
                    }
                }
            }
        }
        foreach (var nested in t.NestedTypes) {
            CheckType(nested);
        }
    }
}

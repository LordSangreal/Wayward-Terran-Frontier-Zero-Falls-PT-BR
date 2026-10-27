using System;
using System.IO;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class FindAudioCues {
    public static void Main() {
        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");
        var soundStrings = new HashSet<string>();

        foreach (var t in asm.MainModule.Types) {
            CheckType(t, soundStrings);
        }

        Console.WriteLine("Total sound cues found: " + soundStrings.Count);
        foreach (var s in soundStrings) {
            Console.WriteLine("CUE: [" + s + "]");
        }
        File.WriteAllLines(@"d:\Tradução WTF Zero falls\scripts\audio_cues.txt", soundStrings);
    }

    static void CheckType(TypeDefinition t, HashSet<string> soundStrings) {
        foreach (var m in t.Methods) {
            if (!m.HasBody) continue;
            for (int i = 0; i < m.Body.Instructions.Count; i++) {
                var instr = m.Body.Instructions[i];
                var mr = instr.Operand as MethodReference;
                if (mr != null) {
                    if (mr.Name == "GetCue" || mr.Name == "PlayCue" || mr.DeclaringType.FullName.Contains("SoundBank") || mr.DeclaringType.FullName.Contains("Audio")) {
                        // Look backwards for ldstr
                        for (int j = i - 1; j >= Math.Max(0, i - 5); j--) {
                            if (m.Body.Instructions[j].OpCode == OpCodes.Ldstr) {
                                string val = m.Body.Instructions[j].Operand as string;
                                if (!string.IsNullOrEmpty(val)) {
                                    soundStrings.Add(val);
                                }
                            }
                        }
                    }
                }
            }
        }
        foreach (var nested in t.NestedTypes) {
            CheckType(nested, soundStrings);
        }
    }
}

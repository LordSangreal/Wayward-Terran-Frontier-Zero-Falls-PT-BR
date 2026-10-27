using System;
using System.IO;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class InspectUnlockHull {
    public static void Main() {
        Check(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe", "ORIGINAL");
        Check(@"D:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe", "PATCHED");
    }

    static void Check(string path, string tag) {
        Console.WriteLine("\n=== " + tag + ": " + path + " ===");
        var asm = AssemblyDefinition.ReadAssembly(path);
        foreach (var t in asm.MainModule.Types) {
            if (t.FullName == "CoOpSpRpG.CHARACTER_DATA") {
                foreach (var m in t.Methods) {
                    if (m.Name == "unlockHull") {
                        Console.WriteLine("Method: " + m.FullName);
                        if (m.HasBody) {
                            foreach (var i in m.Body.Instructions) {
                                if (i.OpCode == OpCodes.Ldstr) {
                                    Console.WriteLine("   ldstr: [" + i.Operand + "]");
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

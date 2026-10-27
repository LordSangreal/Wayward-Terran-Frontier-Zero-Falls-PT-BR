using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class DecompileActionDefense {
    public static void Main() {
        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");
        foreach (var t in asm.MainModule.Types) {
            if (t.Name == "ShipDesignRev4") {
                foreach (var m in t.Methods) {
                    if (m.Name == "actionDefense" || m.Name == "actionOffense") {
                        Console.WriteLine("\n=== Method: " + m.Name + " ===");
                        for (int i = 0; i < m.Body.Instructions.Count; i++) {
                            Console.WriteLine(m.Body.Instructions[i].ToString());
                        }
                    }
                }
            }
        }
    }
}

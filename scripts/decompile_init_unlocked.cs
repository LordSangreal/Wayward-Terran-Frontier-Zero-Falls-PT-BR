using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class DecompileInitUnlocked {
    public static void Main() {
        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");
        foreach (var t in asm.MainModule.Types) {
            if (t.Name == "ShipDesignRev4") {
                foreach (var m in t.Methods) {
                    if (m.Name == "InitUnlockedModules") {
                        for (int i = 0; i < m.Body.Instructions.Count; i++) {
                            var instr = m.Body.Instructions[i];
                            if (instr.OpCode == OpCodes.Ldstr) {
                                Console.WriteLine(i + ": ldstr [" + instr.Operand + "]");
                            }
                        }
                    }
                }
            }
        }
    }
}

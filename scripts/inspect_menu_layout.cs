using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class InspectMenuLayout {
    public static void Main() {
        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");
        foreach (var t in asm.MainModule.Types) {
            if (t.Name == "RootMenuRev2") {
                foreach (var m in t.Methods) {
                    if (m.Name == "createElements" || m.Name == "resize") {
                        Console.WriteLine("\n=== Method: " + m.Name + " ===");
                        for (int i = 0; i < m.Body.Instructions.Count; i++) {
                            var instr = m.Body.Instructions[i];
                            if (instr.OpCode == OpCodes.Ldstr) {
                                string s = (string)instr.Operand;
                                if (s == "Campaign" || s == "Arena" || s == "Settings" || s == "Extra" || s == "Keybinds" || s == "Video & General") {
                                    Console.WriteLine("Around string [" + s + "] at instruction " + i + ":");
                                    for (int j = Math.Max(0, i - 10); j <= Math.Min(m.Body.Instructions.Count - 1, i + 15); j++) {
                                        Console.WriteLine("  " + m.Body.Instructions[j].ToString());
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

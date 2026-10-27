using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class InspectStage {
    public static void Main() {
        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");
        foreach (var t in asm.MainModule.Types) {
            if (t.Name.Contains("AttackPlayerWithNonFactionStage") || t.Name == "Tutorial2") {
                Console.WriteLine("\n=== Type: " + t.FullName + " ===");
                foreach (var m in t.Methods) {
                    if (m.Name == "test" || m.Name == ".ctor" || m.Name == "initialSetupFunc" || m.Name.Contains("Stage")) {
                        Console.WriteLine("Method: " + m.FullName);
                        if (m.HasBody) {
                            foreach (var i in m.Body.Instructions) {
                                if (i.OpCode == OpCodes.Ldstr) {
                                    Console.WriteLine("  ldstr: [" + i.Operand + "]");
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

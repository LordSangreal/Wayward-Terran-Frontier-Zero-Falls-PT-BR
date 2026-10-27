using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class InspectAttackStage {
    public static void Main() {
        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");
        foreach (var t in asm.MainModule.Types) {
            if (t.Name == "AttackPlayerWithNonFactionStage") {
                foreach (var m in t.Methods) {
                    Console.WriteLine("\n--- Method: " + m.FullName + " ---");
                    if (m.HasBody) {
                        foreach (var i in m.Body.Instructions) {
                            Console.WriteLine(i.ToString());
                        }
                    }
                }
            }
        }
    }
}

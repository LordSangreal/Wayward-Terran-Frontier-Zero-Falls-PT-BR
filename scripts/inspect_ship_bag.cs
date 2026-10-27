using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class InspectShipBag {
    public static void Main() {
        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");
        foreach (var t in asm.MainModule.Types) {
            if (t.Name == "SHIPBAG" || t.Name == "Tutorial2") {
                foreach (var m in t.Methods) {
                    if (m.Name.Contains("makeEnemy") || m.Name == "initialSetupFunc") {
                        Console.WriteLine("\n=== Method: " + t.Name + "::" + m.Name + " ===");
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

using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class InspectSuspects {
    public static void Main() {
        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");

        foreach (var t in asm.MainModule.Types) {
            if (t.Name == "LoaderProc") {
                foreach (var m in t.Methods) {
                    if (m.Name == "loadSound") {
                        Console.WriteLine("\n=== LoaderProc::loadSound ===");
                        foreach (var i in m.Body.Instructions) {
                            if (i.OpCode == OpCodes.Ldstr) {
                                Console.WriteLine("  ldstr: [" + i.Operand + "]");
                            }
                        }
                    }
                }
            }
            if (t.Name == "ShipDesignRev4") {
                foreach (var m in t.Methods) {
                    if (m.Name.Contains("Categorie") || m.Name == "Init") {
                        Console.WriteLine("\n=== ShipDesignRev4::" + m.Name + " ===");
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

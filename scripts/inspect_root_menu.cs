using System;
using System.IO;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class InspectRootMenu {
    public static void Main() {
        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");
        foreach (var t in asm.MainModule.Types) {
            if (t.Name == "RootMenuRev2") {
                foreach (var m in t.Methods) {
                    if (m.Name == "getComponent") {
                        Console.WriteLine("Instructions in RootMenuRev2::getComponent:");
                        foreach (var i in m.Body.Instructions) {
                            if (i.OpCode == OpCodes.Ldstr) {
                                Console.WriteLine("  ldstr: " + i.Operand);
                            }
                        }
                    }
                }
            }
        }
    }
}

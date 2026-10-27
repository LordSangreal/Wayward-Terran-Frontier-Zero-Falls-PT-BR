using System;
using System.IO;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class InspectWidgetProfile {
    public static void Main() {
        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");
        foreach (var t in asm.MainModule.Types) {
            if (t.Name.Contains("WidgetProfile")) {
                Console.WriteLine("Type: " + t.FullName);
                foreach (var m in t.Methods) {
                    if (m.HasBody) {
                        Console.WriteLine("  Method: " + m.Name);
                        foreach (var i in m.Body.Instructions) {
                            if (i.OpCode == OpCodes.Ldstr) {
                                Console.WriteLine("    ldstr: [" + i.Operand + "]");
                            }
                        }
                    }
                }
            }
        }
    }
}

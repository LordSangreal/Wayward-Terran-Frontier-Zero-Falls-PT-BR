using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class InspectDeep {
    public static void Main() {
        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");

        foreach (var t in asm.MainModule.Types) {
            if (t.Name.Contains("FeedPirate") || t.Name.Contains("BuddAgent") || t.Name.Contains("MickAgent") || t.Name == "ProcRandCrew" || t.Name == "GenericPassenger") {
                Console.WriteLine("\n=== Type: " + t.FullName + " ===");
                foreach (var m in t.Methods) {
                    if (m.HasBody) {
                        foreach (var i in m.Body.Instructions) {
                            if (i.OpCode == OpCodes.Ldstr) {
                                string s = (string)i.Operand;
                                Console.WriteLine("  " + m.Name + " => [" + s + "]");
                            }
                        }
                    }
                }
            }
        }
    }
}

using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class InspectQuestStageStrings {
    public static void Main() {
        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");
        foreach (var t in asm.MainModule.Types) {
            if (t.BaseType != null && t.BaseType.Name == "QuestStage") {
                foreach (var m in t.Methods) {
                    if (m.HasBody) {
                        foreach (var i in m.Body.Instructions) {
                            if (i.OpCode == OpCodes.Ldstr) {
                                Console.WriteLine(t.Name + "::" + m.Name + " => [" + i.Operand + "]");
                            }
                        }
                    }
                }
            }
        }
    }
}

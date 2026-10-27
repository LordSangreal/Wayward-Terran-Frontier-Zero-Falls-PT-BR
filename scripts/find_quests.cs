using System;
using Mono.Cecil;

public class FindQuests {
    public static void Main() {
        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");
        foreach (var t in asm.MainModule.Types) {
            var b = t.BaseType;
            while (b != null) {
                if (b.Name == "TriggerEvent" || b.Name == "QuestStage") {
                    Console.WriteLine(t.FullName + " inherits from " + b.Name);
                    break;
                }
                try {
                    var def = b.Resolve();
                    b = def != null ? def.BaseType : null;
                } catch {
                    break;
                }
            }
        }
    }
}

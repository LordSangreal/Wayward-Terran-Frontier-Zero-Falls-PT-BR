using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class InspectTutorial2Stages {
    public static void Main() {
        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");
        foreach (var t in asm.MainModule.Types) {
            if (t.Name == "Tutorial2") {
                foreach (var m in t.Methods) {
                    if (m.Name == "buildStages") {
                        Console.WriteLine("Method: " + m.FullName);
                        foreach (var i in m.Body.Instructions) {
                            Console.WriteLine(i.ToString());
                        }
                    }
                }
            }
        }
    }
}

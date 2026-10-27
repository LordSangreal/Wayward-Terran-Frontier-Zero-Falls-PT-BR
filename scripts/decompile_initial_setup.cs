using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class DecompileInitialSetup {
    public static void Main() {
        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");
        foreach (var t in asm.MainModule.Types) {
            if (t.Name == "Tutorial2") {
                foreach (var m in t.Methods) {
                    if (m.Name == "initialSetupFunc") {
                        for (int i = 0; i < m.Body.Instructions.Count; i++) {
                            Console.WriteLine(m.Body.Instructions[i].ToString());
                        }
                    }
                }
            }
        }
    }
}

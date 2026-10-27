using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class FindCategoriesAccess {
    public static void Main() {
        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");
        foreach (var t in asm.MainModule.Types) {
            if (t.Name == "ShipDesignRev4") {
                foreach (var m in t.Methods) {
                    if (!m.HasBody) continue;
                    foreach (var i in m.Body.Instructions) {
                        var fr = i.Operand as FieldReference;
                        if (fr != null && fr.Name == "categories") {
                            Console.WriteLine("ShipDesignRev4::" + m.Name + " accesses categories");
                            break;
                        }
                    }
                }
            }
        }
    }
}

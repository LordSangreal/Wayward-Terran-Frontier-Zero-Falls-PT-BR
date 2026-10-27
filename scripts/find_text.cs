using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class FindText {
    public static void Main(string[] args) {
        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");
        string search = args.Length > 0 ? args[0] : "Continued development";
        foreach (var t in asm.MainModule.Types) {
            CheckType(t, search);
        }
    }

    static void CheckType(TypeDefinition t, string search) {
        foreach (var m in t.Methods) {
            if (!m.HasBody) continue;
            foreach (var i in m.Body.Instructions) {
                if (i.OpCode == OpCodes.Ldstr) {
                    string s = i.Operand as string;
                    if (s != null && s.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0) {
                        Console.WriteLine(t.FullName + "::" + m.Name + " => [" + s + "]");
                    }
                }
            }
        }
        foreach (var nested in t.NestedTypes) {
            CheckType(nested, search);
        }
    }
}

using System;
using Mono.Cecil;

class CheckXmlTypes {
    static void Main() {
        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");
        foreach(var t in asm.MainModule.Types) {
            foreach(var m in t.Methods) {
                if (!m.HasBody) continue;
                foreach(var i in m.Body.Instructions) {
                    if (i.Operand != null && i.Operand.ToString().Contains("System.Xml")) {
                        Console.WriteLine("Type using XML: " + t.FullName + "." + m.Name);
                        break;
                    }
                }
            }
        }
    }
}

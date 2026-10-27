using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class InspectConvoChecks {
    public static void Main() {
        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");

        foreach (var t in asm.MainModule.Types) {
            if (t.Name == "ProcRandCrew" || t.Name == "GenericPassenger") {
                foreach (var m in t.Methods) {
                    if (m.HasBody) {
                        for (int i = 0; i < m.Body.Instructions.Count; i++) {
                            var instr = m.Body.Instructions[i];
                            if (instr.OpCode == OpCodes.Ldstr) {
                                string s = (string)instr.Operand;
                                if (s.StartsWith("<") && s.EndsWith(">")) {
                                    Console.WriteLine(t.Name + "::" + m.Name + " has tag: [" + s + "]");
                                    // check next few instructions for op_Equality
                                    for (int j = i; j <= Math.Min(m.Body.Instructions.Count - 1, i + 5); j++) {
                                        var next = m.Body.Instructions[j];
                                        var mr = next.Operand as MethodReference;
                                        if (mr != null && mr.Name == "op_Equality") {
                                            Console.WriteLine("   -> CHECKED WITH STRING EQUALITY at " + next.ToString());
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

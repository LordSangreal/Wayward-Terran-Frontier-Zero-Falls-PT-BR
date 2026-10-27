using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class VerifyAllTutorial {
    public static void Main() {
        var asm = AssemblyDefinition.ReadAssembly(@"D:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe");
        foreach (var t in asm.MainModule.Types) {
            if (t.Name == "Tutorial2") {
                foreach (var m in t.Methods) {
                    if (m.HasBody) {
                        foreach (var i in m.Body.Instructions) {
                            if (i.OpCode == OpCodes.Ldstr) {
                                string s = (string)i.Operand;
                                if (s == "badguyspawn" || s == "bandido" || s == "waypoint_1" || s == "ponto de passagem_1" || s == "pest" || s == "praga") {
                                    Console.WriteLine("Tutorial2::" + m.Name + " => [" + s + "]");
                                }
                            }
                        }
                    }
                }
            }
            if (t.Name == "CONFIG") {
                foreach (var m in t.Methods) {
                    if (m.Name == ".cctor") {
                        foreach (var i in m.Body.Instructions) {
                            if (i.OpCode == OpCodes.Ldstr && ((string)i.Operand).Contains("odio")) {
                                Console.WriteLine("CONFIG joke => [" + i.Operand + "]");
                            }
                        }
                    }
                }
            }
        }
    }
}

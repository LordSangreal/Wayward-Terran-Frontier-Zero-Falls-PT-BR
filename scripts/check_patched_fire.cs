using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class CheckPatchedFire {
    public static void Main() {
        var asm = AssemblyDefinition.ReadAssembly(@"D:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe");
        foreach (var t in asm.MainModule.Types) {
            if (t.Name == "ShipNavigationRev3") {
                foreach (var m in t.Methods) {
                    if (m.Name == "startSounds" || m.Name == "spawnFire") {
                        Console.WriteLine("Method: " + m.Name);
                        foreach (var i in m.Body.Instructions) {
                            if (i.OpCode == OpCodes.Ldstr) {
                                Console.WriteLine("  ldstr: [" + i.Operand + "]");
                            }
                        }
                    }
                }
            }
        }
    }
}

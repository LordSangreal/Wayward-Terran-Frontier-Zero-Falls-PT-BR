using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

class Program {
    static void Main() {
        string exePath = @"D:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe";
        var asm = AssemblyDefinition.ReadAssembly(exePath);

        bool applied = false;
        foreach (var type in asm.MainModule.Types) {
            if (type.FullName == "CoOpSpRpG.CHARACTER_DATA") {
                foreach (var m in type.Methods) {
                    if (m.Name == "loadWorldCharacter") {
                        var il = m.Body.GetILProcessor();
                        var createAll = type.Methods.FirstOrDefault(x => x.Name == "createAllTables");
                        for (int i = 0; i < m.Body.Instructions.Count; i++) {
                            var instr = m.Body.Instructions[i];
                            if (instr.OpCode == OpCodes.Callvirt && instr.Operand != null && instr.Operand.ToString().Contains("DbConnection::Open")) {
                                if (i > 55) {
                                    var callCreate = il.Create(OpCodes.Call, createAll);
                                    il.InsertAfter(instr, callCreate);
                                    applied = true;
                                    Console.WriteLine("Successfully injected createAllTables after second DbConnection.Open at index " + i);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        if (applied) {
            string temp = exePath + ".tmp";
            asm.Write(temp);
            asm.Dispose();
            File.Copy(temp, exePath, true);
            File.Delete(temp);
            Console.WriteLine("Saved patched WTF.exe with SQLite save auto-migration!");
        } else {
            Console.WriteLine("Could not find injection point.");
        }
    }
}

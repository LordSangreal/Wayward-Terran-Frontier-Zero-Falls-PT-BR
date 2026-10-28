using System;
using System.IO;
using Mono.Cecil;
using Mono.Cecil.Cil;

class Program {
    static void Main(string[] args) {
        string targetExe = @"D:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe";
        bool enableSinglePlayer = true;
        if (args.Length > 0 && args[0] == "mp") enableSinglePlayer = false;

        var readerParams = new ReaderParameters { ReadWrite = false };
        var asm = AssemblyDefinition.ReadAssembly(targetExe, readerParams);

        bool modified = false;
        foreach (var t in asm.MainModule.Types) {
            if (t.FullName == "CoOpSpRpG.CONFIG") {
                foreach (var m in t.Methods) {
                    if (m.Name == ".cctor") {
                        for (int i = 0; i < m.Body.Instructions.Count; i++) {
                            var instr = m.Body.Instructions[i];
                            if (instr.OpCode == OpCodes.Stsfld && instr.Operand != null && instr.Operand.ToString().Contains("openMP")) {
                                var prev = m.Body.Instructions[i - 1];
                                prev.OpCode = enableSinglePlayer ? OpCodes.Ldc_I4_0 : OpCodes.Ldc_I4_1;
                                Console.WriteLine("openMP set to: " + (enableSinglePlayer ? "false (Singleplayer/Campaign Enabled)" : "true (Dedicated MP Test)"));
                                modified = true;
                            }
                        }
                    }
                }
            }
        }

        if (modified) {
            string temp = targetExe + ".tmp";
            asm.Write(temp);
            asm.Dispose();
            File.Copy(temp, targetExe, true);
            File.Delete(temp);
            Console.WriteLine("Saved successfully!");
        }
    }
}

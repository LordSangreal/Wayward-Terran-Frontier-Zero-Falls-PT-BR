using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

class Program
{
    static void Main()
    {
        string exePath = @"D:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe";
        string backupPath = @"D:\SteamLibrary\steamapps\common\Zero Falls\WTF_backup.exe";

        var asmPatched = AssemblyDefinition.ReadAssembly(exePath);
        var asmOrig = AssemblyDefinition.ReadAssembly(backupPath);

        Console.WriteLine("Checking differences in readData / deserialization methods...");

        foreach (var t in asmOrig.MainModule.Types)
        {
            var tPatched = asmPatched.MainModule.Types.FirstOrDefault(x => x.FullName == t.FullName);
            if (tPatched == null) continue;

            foreach (var m in t.Methods)
            {
                if (m.Name.StartsWith("read", StringComparison.OrdinalIgnoreCase) ||
                    m.Name.StartsWith("load", StringComparison.OrdinalIgnoreCase) ||
                    m.Name.IndexOf("deserial", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var mPatched = tPatched.Methods.FirstOrDefault(x => x.Name == m.Name && x.Parameters.Count == m.Parameters.Count);
                    if (mPatched == null || !m.HasBody) continue;

                    var origStrs = m.Body.Instructions.Where(i => i.OpCode == OpCodes.Ldstr).Select(i => (string)i.Operand).ToList();
                    var patchedStrs = mPatched.Body.Instructions.Where(i => i.OpCode == OpCodes.Ldstr).Select(i => (string)i.Operand).ToList();

                    for (int i = 0; i < Math.Min(origStrs.Count, patchedStrs.Count); i++)
                    {
                        if (origStrs[i] != patchedStrs[i])
                        {
                            Console.WriteLine(string.Format("[{0}::{1}] '{2}' -> '{3}'", t.Name, m.Name, origStrs[i], patchedStrs[i]));
                        }
                    }
                }
            }
        }
    }
}

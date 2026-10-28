using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

class Program
{
    static void Main()
    {
        string exePath = @"D:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe";
        var asm = AssemblyDefinition.ReadAssembly(exePath);

        string[] targets = new string[] {
            "CoOpSpRpG.Bedroom",
            "CoOpSpRpG.Airlock",
            "CoOpSpRpG.Door",
            "CoOpSpRpG.ConsoleAccess",
            "CoOpSpRpG.InteractiveRoom",
            "CoOpSpRpG.SPManifest",
            "CoOpSpRpG.TradeOverlay",
            "CoOpSpRpG.VNavigationRev3",
            "CoOpSpRpG.ITEMBAG"
        };

        foreach (var tname in targets)
        {
            var t = asm.MainModule.Types.FirstOrDefault(x => x.FullName == tname);
            if (t == null) continue;
            Console.WriteLine("=== " + tname + " ===");
            foreach (var m in t.Methods)
            {
                if (!m.HasBody) continue;
                foreach (var i in m.Body.Instructions)
                {
                    if (i.OpCode == OpCodes.Ldstr)
                    {
                        string s = (string)i.Operand;
                        if (string.IsNullOrEmpty(s)) continue;

                        // Check interesting strings
                        if (s.Contains("dormir") || s.Contains("eclusa") || s.Contains("travada") ||
                            s.Contains("console") || s.Contains("gosma") || s.Contains("Desacoplar") ||
                            s.Contains("reembolsado") || s.Contains("leito") || s.Contains("loja") ||
                            s.Contains("Terra") || s.Contains("Chumbo") || s.Contains("boring") ||
                            s.Contains("sleep") || s.Contains("Activate airlock") || s.Contains("Door is locked") ||
                            s.Contains("connected to nothing") || s.Contains("Craft goo") || s.Contains("to Undock"))
                        {
                            Console.WriteLine(string.Format("  [{0}] -> {1}", m.Name, s.Replace("\n", " ")));
                        }
                    }
                }
            }
        }
    }
}

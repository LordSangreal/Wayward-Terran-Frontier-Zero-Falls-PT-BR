using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

class Program
{
    static void Main(string[] args)
    {
        string exePath = @"D:\SteamLibrary\steamapps\common\Zero Falls\WTF_backup.exe";
        if (args.Length > 0) exePath = args[0];

        Console.WriteLine("Reading: " + exePath);
        var asm = AssemblyDefinition.ReadAssembly(exePath);

        string[] targetTypes = new string[] {
            "CoOpSpRpG.ITEMBAG",
            "CoOpSpRpG.LOOTBAG",
            "CoOpSpRpG.TILEBAG",
            "CoOpSpRpG.InteractiveRoom",
            "CoOpSpRpG.Bedroom",
            "CoOpSpRpG.Airlock",
            "CoOpSpRpG.Door",
            "CoOpSpRpG.ConsoleAccess",
            "CoOpSpRpG.ConsoleConnect",
            "CoOpSpRpG.SPManifest",
            "CoOpSpRpG.TradeOverlay",
            "CoOpSpRpG.VNavigationRev3",
            "CoOpSpRpG.WidgetPrompt",
            "CoOpSpRpG.LogisticsScreenRev3"
        };

        var results = new Dictionary<string, List<string>>();

        foreach (var typeName in targetTypes)
        {
            var t = asm.MainModule.Types.FirstOrDefault(x => x.FullName == typeName);
            if (t == null) continue;

            var list = new List<string>();
            results[typeName] = list;

            foreach (var m in t.Methods)
            {
                if (!m.HasBody) continue;
                foreach (var inst in m.Body.Instructions)
                {
                    if (inst.OpCode == OpCodes.Ldstr)
                    {
                        string s = inst.Operand as string;
                        if (!string.IsNullOrEmpty(s) && s.Length > 1)
                        {
                            list.Add(s);
                        }
                    }
                }
            }
        }

        foreach (var kvp in results)
        {
            var unique = kvp.Value.Distinct().ToList();
            Console.WriteLine(string.Format("{0}: {1} unique strings", kvp.Key, unique.Count));
            string safeName = kvp.Key.Replace(".", "_");
            File.WriteAllLines(safeName + "_clean.txt", unique);
        }
    }
}

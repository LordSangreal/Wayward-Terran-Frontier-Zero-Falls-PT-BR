using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mono.Cecil;

public class Program {
    public static void Main() {
        var dict = new Dictionary<string, string>();
        string[] lines = File.ReadAllLines(@"d:\Tradução WTF Zero falls\patcher\translations_exe.json");
        var regex = new Regex(@"^\s*""((?:\\.|[^""\\])*)""\s*:\s*""((?:\\.|[^""\\])*)""");
        foreach (var line in lines) {
            var match = regex.Match(line);
            if (match.Success) {
                string k = Regex.Unescape(match.Groups[1].Value);
                string v = Regex.Unescape(match.Groups[2].Value);
                dict[k] = v;
            }
        }

        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");
        string[] targetTypes = new string[] {
            "CoOpSpRpG.UNavigationRev2",
            "CoOpSpRpG.BACKDROP",
            "CoOpSpRpG.TipBiomeResource",
            "CoOpSpRpG.WidgetMapQuests",
            "CoOpSpRpG.WidgetStationMap",
            "CoOpSpRpG.TargetOverlay"
        };

        foreach (var typeName in targetTypes) {
            var t = FindType(asm.MainModule, typeName);
            if (t == null) continue;
            Console.WriteLine("=== " + typeName + " ===");
            foreach (var m in t.Methods) {
                if (!m.HasBody) continue;
                foreach (var i in m.Body.Instructions) {
                    if (i.OpCode.Name == "ldstr") {
                        string s = (string)i.Operand;
                        if (string.IsNullOrWhiteSpace(s) || s.Length < 3) continue;
                        if (s.StartsWith(".") || s.Contains("\\") || s.Contains("/")) continue;

                        if (!dict.ContainsKey(s)) {
                            Console.WriteLine("[NOT IN DICT] " + m.Name + " -> " + s);
                        } else if (dict[s] == s) {
                            Console.WriteLine("[SAME AS EN]  " + m.Name + " -> " + s);
                        } else {
                            Console.WriteLine("[TRANSLATED]  " + m.Name + " -> " + s + " => " + dict[s]);
                        }
                    }
                }
            }
        }
    }

    static TypeDefinition FindType(ModuleDefinition module, string fullName) {
        foreach (var t in module.Types) {
            if (t.FullName == fullName) return t;
        }
        return null;
    }
}

using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class CheckMenuStrings {
    public static void Main() {
        var dict = LoadDictionary(@"d:\Tradução WTF Zero falls\translations_exe.json");
        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");

        foreach (var t in asm.MainModule.Types) {
            if (t.Name == "RootMenuRev2") {
                foreach (var m in t.Methods) {
                    if (m.Name == "createElements") {
                        for (int i = 0; i < m.Body.Instructions.Count; i++) {
                            var instr = m.Body.Instructions[i];
                            if (instr.OpCode == OpCodes.Ldstr) {
                                string s = (string)instr.Operand;
                                string trans = dict.ContainsKey(s) ? dict[s] : "(NOT TRANSLATED)";
                                Console.WriteLine("[" + s + "] (" + s.Length + "c) => [" + trans + "] (" + trans.Length + "c)");
                            }
                        }
                    }
                }
            }
        }
    }

    static Dictionary<string, string> LoadDictionary(string jsonPath) {
        var dict = new Dictionary<string, string>();
        string[] lines = File.ReadAllLines(jsonPath);
        var regex = new Regex(@"^\s*""((?:\\.|[^""\\])*)""\s*:\s*""((?:\\.|[^""\\])*)""");
        foreach (var line in lines) {
            var match = regex.Match(line);
            if (match.Success) {
                dict[Regex.Unescape(match.Groups[1].Value)] = Regex.Unescape(match.Groups[2].Value);
            }
        }
        return dict;
    }
}

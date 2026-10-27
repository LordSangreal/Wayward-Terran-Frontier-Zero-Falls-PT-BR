using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class AnalyzeDictSql {
    public static void Main(string[] args) {
        string jsonPath = @"d:\Tradução WTF Zero falls\translations_exe.json";
        var dict = LoadDictionary(jsonPath);

        string origPath = @"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe";
        var asm = AssemblyDefinition.ReadAssembly(origPath);

        Console.WriteLine("Analyzing SQL types in assembly against translations_exe.json...");
        var badKeys = new HashSet<string>();

        foreach (var t in asm.MainModule.Types) {
            if (t.FullName == "CoOpSpRpG.CHARACTER_DATA" || t.FullName == "CoOpSpRpG.WorldDatabase") {
                foreach (var m in t.Methods) {
                    if (!m.HasBody) continue;
                    foreach (var i in m.Body.Instructions) {
                        if (i.OpCode == OpCodes.Ldstr) {
                            string s = (string)i.Operand;
                            if (dict.ContainsKey(s)) {
                                if (IsUserFacing(s)) {
                                    Console.WriteLine("SAFE USER STRING: [" + s + "] => [" + dict[s] + "] in " + m.Name);
                                } else {
                                    Console.WriteLine("DANGEROUS SQL/TECH STRING: [" + s + "] => [" + dict[s] + "] in " + t.Name + "::" + m.Name);
                                    badKeys.Add(s);
                                }
                            }
                        }
                    }
                }
            }
        }

        Console.WriteLine("\nTotal dangerous keys to purge: " + badKeys.Count);
        // Also check translations_exe.json for any other keys containing SQL patterns
        int extraPurged = 0;
        foreach (var kvp in dict) {
            string k = kvp.Key;
            if (badKeys.Contains(k)) continue;

            string lower = k.ToLowerInvariant().Trim();
            if (lower.StartsWith("select ") || lower.StartsWith("insert ") || lower.StartsWith("update ") || 
                lower.StartsWith("delete ") || lower.StartsWith("alter ") || lower.StartsWith("create table") || 
                lower.StartsWith("drop table") || lower.StartsWith("pragma ") ||
                lower.StartsWith("' and ") || lower.StartsWith("and ") || lower.StartsWith("', @") ||
                lower.Contains(" where ") || lower.Contains(" values (") || lower.Contains(" from ") ||
                lower.StartsWith("@") || lower.Contains("data source=") || lower.EndsWith(".sqlite")) {
                if (!IsUserFacing(k)) {
                    Console.WriteLine("EXTRA DANGEROUS SQL KEY: [" + k + "] => [" + kvp.Value + "]");
                    badKeys.Add(k);
                    extraPurged++;
                }
            }
        }

        Console.WriteLine("Extra purged: " + extraPurged);
        Console.WriteLine("Total keys being purged: " + badKeys.Count);

        // Save purged list
        File.WriteAllLines(@"d:\Tradução WTF Zero falls\scripts\purged_sql_keys.txt", badKeys);
    }

    static bool IsUserFacing(string s) {
        if (s == "Character name already exists") return true;
        if (s == "Character name error") return true;
        if (s == "Level up! You are now level ") return true;
        if (s == "New research available. Press ") return true;
        if (s == " to open research") return true;
        if (s == "New ship research available") return true;
        if (s == "Ship hull unlocked") return true;
        if (s == "Ship design added") return true;
        if (s == "variant ") return true;
        if (s == "Tab ") return true;
        if (s == "Failed to upgrade an old profile") return true;
        return false;
    }

    static Dictionary<string, string> LoadDictionary(string jsonPath) {
        var dict = new Dictionary<string, string>();
        string[] lines = File.ReadAllLines(jsonPath);
        var regex = new Regex(@"^\s*""((?:\\.|[^""\\])*)""\s*:\s*""((?:\\.|[^""\\])*)""");
        foreach (var line in lines) {
            var match = regex.Match(line);
            if (match.Success) {
                string k = Regex.Unescape(match.Groups[1].Value);
                string v = Regex.Unescape(match.Groups[2].Value);
                dict[k] = v;
            }
        }
        return dict;
    }
}

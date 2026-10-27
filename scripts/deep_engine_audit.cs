using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class DeepEngineAudit {
    public static void Main() {
        string jsonPath = @"d:\Tradução WTF Zero falls\translations_exe.json";
        var dict = LoadDictionary(jsonPath);
        Console.WriteLine("Loaded " + dict.Count + " translations.");

        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");

        var technicalStringsFound = new Dictionary<string, List<string>>();

        foreach (var t in asm.MainModule.Types) {
            AuditType(t, dict, technicalStringsFound);
        }

        Console.WriteLine("\n=== POTENTIALLY DANGEROUS TRANSLATIONS FOUND ===");
        var criticalPurgeList = new HashSet<string>();

        foreach (var kvp in technicalStringsFound) {
            string str = kvp.Key;
            var usages = kvp.Value;
            string trans = dict[str];

            // If the translation changed the string:
            if (str != trans) {
                Console.WriteLine("\n[CRITICAL SUSPECT] Original: \"" + str + "\" => Patched: \"" + trans + "\"");
                foreach (var u in usages) {
                    Console.WriteLine("   Used at: " + u);
                }
                criticalPurgeList.Add(str);
            }
        }

        Console.WriteLine("\nTotal dangerous strings flagged: " + criticalPurgeList.Count);
        File.WriteAllLines(@"d:\Tradução WTF Zero falls\scripts\flagged_technical_strings.txt", criticalPurgeList);
    }

    static void AuditType(TypeDefinition t, Dictionary<string, string> dict, Dictionary<string, List<string>> results) {
        foreach (var m in t.Methods) {
            if (!m.HasBody) continue;

            for (int i = 0; i < m.Body.Instructions.Count; i++) {
                var instr = m.Body.Instructions[i];
                if (instr.OpCode == OpCodes.Ldstr) {
                    string s = instr.Operand as string;
                    if (string.IsNullOrEmpty(s)) continue;

                    // Only care if it is in our translation dictionary
                    if (!dict.ContainsKey(s)) continue;

                    string reason = CheckIfTechnicalUsage(t, m, i, s);
                    if (reason != null) {
                        if (!results.ContainsKey(s)) results[s] = new List<string>();
                        string usageInfo = t.Name + "::" + m.Name + " (" + reason + ")";
                        if (!results[s].Contains(usageInfo)) results[s].Add(usageInfo);
                    }
                }
            }
        }

        foreach (var nested in t.NestedTypes) {
            AuditType(nested, dict, results);
        }
    }

    static string CheckIfTechnicalUsage(TypeDefinition t, MethodDefinition m, int instrIndex, string s) {
        var instructions = m.Body.Instructions;
        var instr = instructions[instrIndex];

        // 1. Look ahead for method calls where this string is an argument
        for (int k = instrIndex + 1; k <= Math.Min(instructions.Count - 1, instrIndex + 6); k++) {
            var next = instructions[k];
            var mr = next.Operand as MethodReference;
            if (mr != null) {
                string dt = mr.DeclaringType.FullName;
                string mn = mr.Name;

                // Audio
                if (dt.Contains("Sound") || dt.Contains("Audio") || mn == "GetCue" || mn == "PlayCue")
                    return "Audio Call: " + dt + "::" + mn;

                // IO & Filesystem
                if (dt.StartsWith("System.IO") || dt.Contains("Path") || dt.Contains("File") || dt.Contains("Directory"))
                    return "IO/File Call: " + dt + "::" + mn;

                // Content / Assets
                if (dt.Contains("Content") || dt.Contains("Asset") || mn == "Load")
                    return "Asset/Content Call: " + dt + "::" + mn;

                // Graphics / Shaders
                if (dt.Contains("Graphics") || dt.Contains("Effect") || dt.Contains("Shader") || mn == "get_Item" && dt.Contains("Parameter"))
                    return "Shader/Graphics Call: " + dt + "::" + mn;

                // XML / Serialization
                if (dt.Contains("Xml") || dt.Contains("Serialization"))
                    return "XML Call: " + dt + "::" + mn;

                // Database / SQLite
                if (dt.Contains("SQLite") || dt.Contains("Data") || mn.Contains("Execute"))
                    return "Database Call: " + dt + "::" + mn;

                // POI / Quest / Session matching
                if (dt.Contains("BattleSession") || dt.Contains("TriggerEvent") || dt.Contains("Quest") || mn.Contains("makeEnemy"))
                    return "Quest/POI Call: " + dt + "::" + mn;

                // Dictionary lookups where string is used as an internal key
                if (mn == "ContainsKey" || (mn == "get_Item" && dt.Contains("Dictionary"))) {
                    // Check if the type is an internal engine cache
                    if (!t.Name.Contains("Dialogue") && !t.Name.Contains("Text")) {
                        return "Dictionary Key Lookup: " + dt + "::" + mn;
                    }
                }
            }
        }

        // 2. Map markers and POI keywords
        string lower = s.ToLowerInvariant().Trim();
        if (lower.StartsWith("waypoint") || lower.EndsWith("spawn") || lower.StartsWith("poi_") || 
            lower.StartsWith("boss_") || lower.StartsWith("station_") || lower.StartsWith("quest_")) {
            return "POI/Map Keyword pattern";
        }

        // 3. File extensions or paths
        if (s.StartsWith(".") && s.Length <= 5 && !s.Contains(" ")) {
            return "File Extension";
        }
        if (s.Contains("\\") || s.Contains("/") && (s.EndsWith(".xnb") || s.EndsWith(".png") || s.EndsWith(".xml") || s.EndsWith(".ini") || s.EndsWith(".wdb"))) {
            return "File Path";
        }

        // 4. XML / HTML tag patterns
        if (s.StartsWith("<") && s.EndsWith(">")) {
            return "XML Tag";
        }

        return null;
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

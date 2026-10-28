using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mono.Cecil;
using Mono.Cecil.Cil;

class Program {
    static void Main() {
        string gameExe = @"D:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe";
        string dictFile = @"d:\Tradução WTF Zero falls\translations_exe.json";
        string blacklistFile = @"d:\Tradução WTF Zero falls\patcher\shader_blacklist.json";

        var dict = LoadDictionary(dictFile);
        var blacklist = LoadBlacklist(blacklistFile);

        var readerParams = new ReaderParameters { ReadWrite = false };
        var asm = AssemblyDefinition.ReadAssembly(gameExe, readerParams);

        int totalLdstr = 0;
        int matched = 0;
        var unmappedStrings = new Dictionary<string, int>();

        foreach (var type in asm.MainModule.Types) {
            CheckType(type, dict, blacklist, ref totalLdstr, ref matched, unmappedStrings);
        }

        Console.WriteLine("New WTF.exe analysis:");
        Console.WriteLine("Total Types: " + asm.MainModule.Types.Count);
        Console.WriteLine("Total Ldstr instructions examined: " + totalLdstr);
        Console.WriteLine("Total Matched and Patchable: " + matched);
        Console.WriteLine("Unmapped unique strings: " + unmappedStrings.Count);

        // Find candidate strings that look like english dialogue / UI in the unmapped list
        int candidateCount = 0;
        using (var sw = new StreamWriter(@"d:\Tradução WTF Zero falls\scripts\new_unmapped_candidates.txt")) {
            foreach (var kvp in unmappedStrings) {
                string s = kvp.Key;
                if (LooksLikeUserText(s)) {
                    candidateCount++;
                    sw.WriteLine(s);
                }
            }
        }
        Console.WriteLine("Candidates for new translation: " + candidateCount);
    }

    static void CheckType(TypeDefinition type, Dictionary<string, string> dict, HashSet<string> blacklist, ref int total, ref int matched, Dictionary<string, int> unmapped) {
        if (type.Name == "TutorialTemplate" || type.Name == "TUTORIAL_BAG" || type.Name == "WorldDatabase" || type.Name == "AttackPlayerWithNonFactionStage")
            return;

        foreach (var m in type.Methods) {
            if (!m.HasBody) continue;
            if (m.Name == "drawBumpyStuff" || m.Name == "combinePasses" || m.Name == "applyToScreen" || m.Name == "drawGraphs")
                continue;
            if (type.Name == "Game1" && (m.Name == ".ctor" || m.Name == "Initialize")) continue;
            if (type.Name == "LoaderProc" && m.Name == "loadSound") continue;
            if (type.Name == "ShipNavigationRev3" && (m.Name == "startSounds" || m.Name == "spawnFire")) continue;
            if (type.Name == "Ship" && m.Name == "playTravelCharge") continue;
            if (type.Name == "HackingScreen" && m.Name == "Update") continue;
            if ((type.Name == "ShipDesignRev4" || type.Name == "ShipDesignRev3") && (m.Name == "InitCategories" || m.Name == "Init")) continue;
            if (m.Name == "writeData") continue;
            if (type.BaseType != null && type.BaseType.Name == "TriggerEvent" && m.Name == ".ctor") continue;

            foreach (var i in m.Body.Instructions) {
                if (i.OpCode == OpCodes.Ldstr) {
                    string s = i.Operand as string;
                    if (string.IsNullOrEmpty(s)) continue;
                    total++;

                    if (blacklist.Contains(s) && !s.Contains(" ")) continue;
                    if (IsSqlOrTechnical(s)) continue;

                    if (dict.ContainsKey(s)) {
                        matched++;
                    } else {
                        if (!unmapped.ContainsKey(s)) unmapped[s] = 0;
                        unmapped[s]++;
                    }
                }
            }
        }

        foreach (var nested in type.NestedTypes) {
            CheckType(nested, dict, blacklist, ref total, ref matched, unmapped);
        }
    }

    static bool LooksLikeUserText(string s) {
        s = s.Trim();
        if (s.Length < 3) return false;
        if (s.Contains(" ") && (s.EndsWith(".") || s.EndsWith("!") || s.EndsWith("?") || s.EndsWith(":") || char.IsUpper(s[0]))) {
            return true;
        }
        return false;
    }

    static bool IsSqlOrTechnical(string s) {
        if (string.IsNullOrEmpty(s)) return true;
        string lower = s.ToLowerInvariant().Trim();
        if (lower == "name" || lower == "page" || lower == "img" || lower == "src" || lower == "body" || 
            lower == "type" || lower == "color" || lower == "font" || lower == "text" || lower == "bind")
            return true;
        if (lower == "content" || lower == @"root\cimv2") return true;
        if (lower == "fire" || lower == "warpspool" || lower == "klaxon_loop" || lower == "amb_reactor-loop") return true;
        if (lower == "introduction" || lower.StartsWith("waypoint_") || lower == "badguyspawn" || 
            lower == "pest" || lower == "hacking" || lower == "ship navigation" ||
            lower == "station_t1_repair" || lower == "ssc_drone_hive" || lower == "repair tutorial" ||
            lower == "farming" || lower == "feed the rats" || lower == "null quest" || lower == "null quest name" ||
            lower == "3 ssc bosses")
            return true;
        if (lower == "missile" || lower == "budd" || lower == "mick" || lower == "shady dealer" ||
            lower == "robo barkeep 880" || lower == "robo barkeep 890")
            return true;
        if (s.StartsWith(".") && s.Length <= 5 && !s.Contains(" ")) return true;
        if ((s.StartsWith("\\") || s.StartsWith("/")) && (s.EndsWith("\\") || s.EndsWith("/"))) return true;
        if (lower.Contains("data source=") || lower.Contains("version=3;") || lower.EndsWith(".sqlite") || lower.EndsWith(".db")) return true;
        if (lower.StartsWith("create table") || lower.StartsWith("delete from") || lower.StartsWith("insert into") || lower.StartsWith("insert or") || lower.StartsWith("drop table") || lower.StartsWith("pragma ") || lower.StartsWith("alter table")) return true;
        if (lower.StartsWith("select ") && (lower.Contains(" from ") || lower.Contains(" count(") || lower.Contains(" where ") || lower.EndsWith("from "))) return true;
        if (lower.StartsWith("' and") || lower.StartsWith("', @") || lower.StartsWith("@") || lower.Contains("column_name=") || lower.Contains("table_name=")) return true;
        if (lower.Contains("primary key") || lower.Contains("values (") || lower.Contains("values(")) return true;
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

    static HashSet<string> LoadBlacklist(string jsonPath) {
        var set = new HashSet<string>();
        if (!File.Exists(jsonPath)) return set;
        string[] lines = File.ReadAllLines(jsonPath);
        var regex = new Regex(@"^\s*""((?:\\.|[^""\\])*)""");
        foreach (var line in lines) {
            var match = regex.Match(line);
            if (match.Success) {
                string val = Regex.Unescape(match.Groups[1].Value);
                set.Add(val);
            }
        }
        return set;
    }
}
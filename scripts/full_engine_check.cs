using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class FullEngineSanityCheck {
    public static void Main() {
        string jsonPath = @"d:\Tradução WTF Zero falls\translations_exe.json";
        var dict = LoadDictionary(jsonPath);
        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");

        // 1. Collect all sound cues from LoaderProc::loadSound and all GetCue/PlayCue calls
        var soundCues = new HashSet<string>();
        foreach (var t in asm.MainModule.Types) {
            foreach (var m in t.Methods) {
                if (!m.HasBody) continue;
                if ((t.Name == "LoaderProc" && m.Name == "loadSound") || m.Name.Contains("Sound") || m.Name.Contains("Audio")) {
                    foreach (var i in m.Body.Instructions) {
                        if (i.OpCode == OpCodes.Ldstr) {
                            string s = (string)i.Operand;
                            if (!string.IsNullOrEmpty(s)) soundCues.Add(s);
                        }
                    }
                }
            }
        }

        // 2. Collect all agent serialized names
        var agentNames = new HashSet<string>();
        foreach (var t in asm.MainModule.Types) {
            if (t.Name.EndsWith("Agent") || t.Name.Contains("Dealer") || t.Name.Contains("Barkeep")) {
                foreach (var m in t.Methods) {
                    if (!m.HasBody) continue;
                    if (m.Name == "writeData" || m.Name == ".ctor") {
                        for (int i = 0; i < m.Body.Instructions.Count; i++) {
                            var instr = m.Body.Instructions[i];
                            if (instr.OpCode == OpCodes.Ldstr) {
                                for (int j = i + 1; j <= Math.Min(m.Body.Instructions.Count - 1, i + 3); j++) {
                                    var mr = m.Body.Instructions[j].Operand as MethodReference;
                                    if (mr != null && (mr.Name == "Write" || mr.Name == "ReadString")) {
                                        agentNames.Add((string)instr.Operand);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // 3. Collect ShipDesign category keys
        var categoryKeys = new HashSet<string>();
        foreach (var t in asm.MainModule.Types) {
            if (t.Name == "ShipDesignRev4" || t.Name == "ShipDesignRev3") {
                foreach (var m in t.Methods) {
                    if (m.Name == "InitCategories" || m.Name == "Init") {
                        if (m.HasBody) {
                            foreach (var i in m.Body.Instructions) {
                                if (i.OpCode == OpCodes.Ldstr) categoryKeys.Add((string)i.Operand);
                            }
                        }
                    }
                }
            }
        }

        // 4. Collect quest names and POI names
        var questIds = new HashSet<string>();
        foreach (var t in asm.MainModule.Types) {
            if (t.BaseType != null && (t.BaseType.Name == "TriggerEvent" || t.BaseType.Name == "QuestStage")) {
                foreach (var m in t.Methods) {
                    if (m.Name == ".ctor" && m.HasBody) {
                        foreach (var i in m.Body.Instructions) {
                            if (i.OpCode == OpCodes.Ldstr) {
                                string s = (string)i.Operand;
                                if (!s.Contains(" ") && s.Length > 2) questIds.Add(s);
                            }
                        }
                    }
                }
            }
        }

        // 5. Compare with translations_exe.json
        Console.WriteLine("Auditing translations dictionary...");
        var badSound = new List<string>();
        var badAgent = new List<string>();
        var badCat = new List<string>();
        var badQuest = new List<string>();
        var badPaths = new List<string>();

        foreach (var kvp in dict) {
            string k = kvp.Key;
            string v = kvp.Value;
            if (k == v) continue; // Unmodified is safe

            if (soundCues.Contains(k)) badSound.Add(k + " => " + v);
            if (agentNames.Contains(k)) badAgent.Add(k + " => " + v);
            if (categoryKeys.Contains(k)) badCat.Add(k + " => " + v);
            if (questIds.Contains(k)) badQuest.Add(k + " => " + v);

            if ((k.StartsWith("\\") || k.StartsWith("/")) && (k.EndsWith("\\") || k.EndsWith("/")))
                badPaths.Add(k + " => " + v);
            if (k.StartsWith(".") && k.Length <= 5 && !k.Contains(" "))
                badPaths.Add(k + " => " + v);
        }

        Console.WriteLine("\n[1] Dangerous Sound Cues translated: " + badSound.Count);
        foreach (var s in badSound) Console.WriteLine("  SOUND: " + s);

        Console.WriteLine("\n[2] Dangerous Agent Save Names translated: " + badAgent.Count);
        foreach (var s in badAgent) Console.WriteLine("  AGENT: " + s);

        Console.WriteLine("\n[3] Dangerous Category Keys translated: " + badCat.Count);
        foreach (var s in badCat) Console.WriteLine("  CATEGORY: " + s);

        Console.WriteLine("\n[4] Dangerous Quest IDs translated: " + badQuest.Count);
        foreach (var s in badQuest) Console.WriteLine("  QUEST: " + s);

        Console.WriteLine("\n[5] Dangerous File Paths/Extensions translated: " + badPaths.Count);
        foreach (var s in badPaths) Console.WriteLine("  PATH: " + s);
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

using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class PurgeAllEngineTechnical {
    public static void Main() {
        var dangerousKeys = new HashSet<string>() {
            "\\orange sales\\",
            "\\grey sales\\",
            ".txt",
            "Missile",
            "Budd",
            "Mick",
            "Robo barkeep 890",
            "Robo barkeep 880",
            "Shady Dealer",
            "Farming",
            "feed the rats",
            "NULL QUEST",
            "NULL QUEST NAME",
            "3 SSC bosses"
        };

        CleanFile(@"d:\Tradução WTF Zero falls\translations_exe.json", dangerousKeys);
        CleanFile(@"d:\Tradução WTF Zero falls\patcher\translations_exe.json", dangerousKeys);
    }

    static void CleanFile(string path, HashSet<string> keys) {
        if (!File.Exists(path)) return;
        string[] lines = File.ReadAllLines(path);
        var regex = new Regex(@"^\s*""((?:\\.|[^""\\])*)""\s*:\s*""((?:\\.|[^""\\])*)""");
        var outputLines = new List<string>();
        int removed = 0;

        foreach (var line in lines) {
            var m = regex.Match(line);
            if (m.Success) {
                string k = Regex.Unescape(m.Groups[1].Value);
                if (keys.Contains(k)) {
                    Console.WriteLine("Removed [" + k + "] from " + path);
                    removed++;
                    continue;
                }
            }
            outputLines.Add(line);
        }

        // Clean trailing comma
        for (int i = outputLines.Count - 1; i >= 0; i--) {
            string trimmed = outputLines[i].Trim();
            if (trimmed == "}") {
                for (int j = i - 1; j >= 0; j--) {
                    if (!string.IsNullOrEmpty(outputLines[j].Trim())) {
                        if (outputLines[j].EndsWith(",")) {
                            outputLines[j] = outputLines[j].TrimEnd(',');
                        }
                        break;
                    }
                }
                break;
            }
        }

        File.WriteAllLines(path, outputLines);
        Console.WriteLine("Cleaned " + removed + " technical keys from " + path);
    }
}

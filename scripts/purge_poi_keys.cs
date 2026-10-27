using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class PurgePoiKeys {
    public static void Main() {
        var keysToPurge = new HashSet<string>() {
            "introduction",
            "waypoint_1",
            "waypoint_2",
            "badguyspawn",
            "pest",
            "hacking",
            "Ship Navigation",
            "station_t1_repair",
            "ssc_drone_hive",
            "buyaship",
            "artifact_intro",
            "drone_boss",
            "janes_operation",
            "repair tutorial",
            "everetts_coherence_rev3",
            "Fire"
        };

        PurgeFile(@"d:\Tradução WTF Zero falls\translations_exe.json", keysToPurge);
        PurgeFile(@"d:\Tradução WTF Zero falls\patcher\translations_exe.json", keysToPurge);
    }

    static void PurgeFile(string path, HashSet<string> keys) {
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
        Console.WriteLine("Purged " + removed + " keys from " + path);
    }
}

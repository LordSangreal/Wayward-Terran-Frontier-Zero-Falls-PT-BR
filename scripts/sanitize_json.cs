using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class SanitizeJson {
    public static void Main() {
        var badKeys = new HashSet<string>() {
            "ALTER TABLE characters RENAME TO oldcharacters",
            "COLUMN_NAME='nodes' AND TABLE_NAME='characters'",
            "alter table characters add column nodes BIG INT",
            "' and type = @type",
            "', @path, @title, @description)",
            "' and path = @path",
            "' and tier = @tier ",
            "' and id = @id and tier = @tier",
            "', @id, @tier )",
            "', @id, @paid, @cost)",
            "', @R, @G, @B )",
            "' and red = @R and green = @G and blue = @B",
            "' and checksum = '",
            "', @id )",
            "' and id = @id",
            "The database does not contain game progress data.",
            "attempting to write faction binary data"
        };

        SanitizeFile(@"d:\Tradução WTF Zero falls\translations_exe.json", badKeys);
        SanitizeFile(@"d:\Tradução WTF Zero falls\patcher\translations_exe.json", badKeys);
    }

    static void SanitizeFile(string path, HashSet<string> badKeys) {
        if (!File.Exists(path)) return;
        string[] lines = File.ReadAllLines(path);
        var regex = new Regex(@"^\s*""((?:\\.|[^""\\])*)""\s*:\s*""((?:\\.|[^""\\])*)""");
        
        var outputLines = new List<string>();
        int removed = 0;

        for (int i = 0; i < lines.Length; i++) {
            var line = lines[i];
            var match = regex.Match(line);
            if (match.Success) {
                string k = Regex.Unescape(match.Groups[1].Value);
                if (badKeys.Contains(k)) {
                    removed++;
                    continue; // Skip this line
                }
            }
            outputLines.Add(line);
        }

        // Fix potential trailing comma before closing brace if needed
        for (int i = outputLines.Count - 1; i >= 0; i--) {
            string trimmed = outputLines[i].Trim();
            if (trimmed == "}") {
                for (int j = i - 1; j >= 0; j--) {
                    string prev = outputLines[j].Trim();
                    if (!string.IsNullOrEmpty(prev)) {
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
        Console.WriteLine("Sanitized " + path + ": removed " + removed + " dangerous SQL keys.");
    }
}

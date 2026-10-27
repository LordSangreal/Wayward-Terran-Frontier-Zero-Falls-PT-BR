using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class FindExactSqlKeys {
    public static void Main() {
        string jsonPath = @"d:\Tradução WTF Zero falls\translations_exe.json";
        string[] lines = File.ReadAllLines(jsonPath);
        var regex = new Regex(@"^\s*""((?:\\.|[^""\\])*)""\s*:\s*""((?:\\.|[^""\\])*)""");
        
        var badKeys = new List<string>();

        foreach (var line in lines) {
            var match = regex.Match(line);
            if (!match.Success) continue;
            string k = Regex.Unescape(match.Groups[1].Value);
            string v = Regex.Unescape(match.Groups[2].Value);
            string lower = k.ToLowerInvariant().Trim();

            bool isSql = false;

            if (lower.StartsWith("' and") || lower.StartsWith("and ") || lower.StartsWith("', @") || 
                lower.StartsWith("@") || lower.StartsWith("alter table") || lower.StartsWith("create table") || 
                lower.StartsWith("drop table") || lower.StartsWith("pragma ") || lower.StartsWith("insert into") ||
                lower.StartsWith("insert or") || lower.StartsWith("delete from") || lower.StartsWith("update ") ||
                lower.EndsWith(".sqlite") || lower.EndsWith(".db") || lower.Contains("data source=") ||
                lower.Contains("table_name=") || lower.Contains("column_name=")) {
                isSql = true;
            } else if (lower.StartsWith("select ") && (lower.Contains(" from ") || lower.Contains(" count("))) {
                isSql = true;
            }

            if (isSql) {
                badKeys.Add(k);
                Console.WriteLine("FOUND SQL KEY: [" + k + "] => [" + v + "]");
            }
        }

        Console.WriteLine("\nTotal SQL keys found: " + badKeys.Count);
        File.WriteAllLines(@"d:\Tradução WTF Zero falls\scripts\exact_sql_keys_to_remove.txt", badKeys);
    }
}

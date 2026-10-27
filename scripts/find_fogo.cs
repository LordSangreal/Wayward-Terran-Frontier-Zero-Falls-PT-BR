using System;
using System.IO;
using System.Text.RegularExpressions;

public class FindFogo {
    public static void Main() {
        string[] lines = File.ReadAllLines(@"d:\Tradução WTF Zero falls\translations_exe.json");
        var regex = new Regex(@"^\s*""((?:\\.|[^""\\])*)""\s*:\s*""((?:\\.|[^""\\])*)""");
        for (int i = 0; i < lines.Length; i++) {
            var m = regex.Match(lines[i]);
            if (m.Success) {
                string k = Regex.Unescape(m.Groups[1].Value);
                string v = Regex.Unescape(m.Groups[2].Value);
                if (k.Equals("Fire", StringComparison.OrdinalIgnoreCase) || v.Equals("Fogo", StringComparison.OrdinalIgnoreCase)) {
                    Console.WriteLine("Line " + (i+1) + ": [" + k + "] => [" + v + "]");
                }
            }
        }
    }
}

using System;
using System.IO;
using System.Text.RegularExpressions;

public class UpdateDiscordJoke {
    public static void Main() {
        UpdateFile(@"d:\Tradução WTF Zero falls\translations_exe.json");
        UpdateFile(@"d:\Tradução WTF Zero falls\patcher\translations_exe.json");
    }

    static void UpdateFile(string path) {
        if (!File.Exists(path)) return;
        string[] lines = File.ReadAllLines(path);
        var regex = new Regex(@"^\s*""((?:\\.|[^""\\])*)""\s*:\s*""((?:\\.|[^""\\])*)""");
        
        for (int i = 0; i < lines.Length; i++) {
            var m = regex.Match(lines[i]);
            if (m.Success) {
                string k = Regex.Unescape(m.Groups[1].Value);
                if (k.StartsWith("Continued development of this game")) {
                    lines[i] = "  \"Continued development of this game brought to you by hatred, mental illness, and sunk cost fallacy. Come visit our discord if you'd like to help test multiplayer.\": \"O desenvolvimento continuo deste jogo e movido a puro odio, loucura e a falacia dos custos irrecuperaveis. Visite nosso Discord se quiser ajudar a testar o multijogador.\",";
                    break;
                }
            }
        }
        File.WriteAllLines(path, lines);
        Console.WriteLine("Updated " + path);
    }
}

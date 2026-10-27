using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Program {
    public static void Main() {
        var dict = new Dictionary<string, string>();
        string[] lines = File.ReadAllLines(@"d:\Tradução WTF Zero falls\patcher\translations_exe.json");
        var regex = new Regex(@"^\s*""((?:\\.|[^""\\])*)""\s*:\s*""((?:\\.|[^""\\])*)""");
        foreach (var line in lines) {
            var match = regex.Match(line);
            if (match.Success) {
                string k = Regex.Unescape(match.Groups[1].Value);
                string v = Regex.Unescape(match.Groups[2].Value);
                dict[k] = v;
            }
        }
        string[] targets = new string[] {
            "Factory", "Processing", "Trade hub", "Mining", "Shipyard", "Farm",
            "Repair station", "Abandoned station", "Deactivated gate", "Active gate",
            "Home station", "Waypoint (right click)", "Point of interest",
            "Domum belt", "Grey belt", "Wolf nebula", "Infested nebula", "Outer belt",
            "Rich fertile land with an abundance of the low value bulk minerals needed for a massive terraforming operation. This area was under active prospecting when terraforming operations were shut down.",
            "Base yield:", "Base yield: ", "Guaranteed occurrence", "Possible occurrence",
            "Common", "Uncommon", "Rare", "Exotic"
        };
        foreach (var t in targets) {
            if (dict.ContainsKey(t)) {
                Console.WriteLine(t + " => " + dict[t]);
            } else {
                Console.WriteLine(t + " => [NOT IN DICTIONARY]");
            }
        }
    }
}

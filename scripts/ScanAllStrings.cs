using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mono.Cecil;
using Mono.Cecil.Cil;

class ScanAllStrings
{
    static void Main()
    {
        string exePath = @"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe";
        var asm = AssemblyDefinition.ReadAssembly(exePath);

        var candidateMap = new Dictionary<string, List<string>>(); // string -> list of "Type.Method"

        foreach (var type in asm.MainModule.Types)
        {
            ScanType(type, candidateMap);
        }

        Console.WriteLine("Total unique strings found in code: " + candidateMap.Count);

        // Filter user-facing strings
        var userFacing = new Dictionary<string, string>(); // string -> sample location
        int skipped = 0;

        foreach (var kvp in candidateMap)
        {
            string s = kvp.Key;
            string loc = kvp.Value[0];

            if (IsUserFacing(s, kvp.Value))
            {
                userFacing[s] = loc;
            }
            else
            {
                skipped++;
            }
        }

        Console.WriteLine("Filtered User-Facing strings: " + userFacing.Count);
        Console.WriteLine("Skipped internal/code strings: " + skipped);

        using (var sw = new StreamWriter(@"d:\Tradução WTF Zero falls\scripts\all_game_strings.json", false, System.Text.Encoding.UTF8))
        {
            sw.WriteLine("[");
            int i = 0;
            foreach (var kvp in userFacing)
            {
                string s = kvp.Key;
                string loc = kvp.Value;
                sw.Write("  { \"text\": \"" + EscapeJson(s) + "\", \"location\": \"" + EscapeJson(loc) + "\" }");
                if (i < userFacing.Count - 1) sw.WriteLine(",");
                else sw.WriteLine();
                i++;
            }
            sw.WriteLine("]");
        }
    }

    static void ScanType(TypeDefinition type, Dictionary<string, List<string>> map)
    {
        // Don't scan shader methods that crash
        foreach (var m in type.Methods)
        {
            if (!m.HasBody) continue;
            if (m.Name == "drawBumpyStuff" || m.Name == "combinePasses" || m.Name == "applyToScreen" || m.Name == "drawGraphs")
                continue;

            string location = type.Name + "." + m.Name;

            foreach (var instr in m.Body.Instructions)
            {
                if (instr.OpCode == OpCodes.Ldstr)
                {
                    string s = (string)instr.Operand;
                    if (string.IsNullOrEmpty(s)) continue;

                    if (!map.ContainsKey(s))
                    {
                        map[s] = new List<string>();
                    }
                    map[s].Add(location);
                }
            }
        }

        foreach (var nested in type.NestedTypes)
        {
            ScanType(nested, map);
        }
    }

    static bool IsUserFacing(string s, List<string> locations)
    {
        string trimmed = s.Trim();
        if (trimmed.Length < 2) return false;

        // Skip obvious assets & technical identifiers
        if (s.StartsWith("Content/") || s.StartsWith("Content\\")) return false;
        if (s.EndsWith(".xnb") || s.EndsWith(".png") || s.EndsWith(".fx") || s.EndsWith(".wav") || s.EndsWith(".ogg") || s.EndsWith(".dll")) return false;
        if (s.StartsWith("Shader") || s.StartsWith("Technique") || s.StartsWith("Pass")) return false;
        if (s.StartsWith("http://") || s.StartsWith("https://")) return false;
        if (s.StartsWith("Microsoft.") || s.StartsWith("System.")) return false;

        // Skip formatting or internal flags
        if (s == "incomplete" || s == "complete" || s == "failed") return false;
        if (s == "..." || s == "...." || s == "....." || s == "......" || s == ".......") return false;
        if (s == "\t|\r" || s == "\\t|\\r") return false;
        if (s.AllNumbersOrSymbols()) return false;

        // If it contains spaces or punctuation, it's very likely human text
        if (s.Contains(" ") || s.Contains(":") || s.Contains("!") || s.Contains("?") || s.Contains(".") || s.Contains(","))
        {
            // Skip regex patterns or file paths
            if (s.Contains("/") || s.Contains("\\"))
            {
                if (!s.Contains(" ")) return false;
            }
            if (s.StartsWith("^") || s.EndsWith("$")) return false;
            return true;
        }

        // Single word tokens: check location
        bool inUiOrDialogue = false;
        foreach (var loc in locations)
        {
            if (loc.Contains("Widget") || loc.Contains("Dialogue") || loc.Contains("Quest") || 
                loc.Contains("Agent") || loc.Contains("Tutorial") || loc.Contains("Screen") || 
                loc.Contains("Menu") || loc.Contains("Inventory") || loc.Contains("Item") || 
                loc.Contains("Research") || loc.Contains("Ship") || loc.Contains("Weapon"))
            {
                inUiOrDialogue = true;
                break;
            }
        }

        // Only allow known single-word user UI labels
        if (inUiOrDialogue && trimmed.Length >= 3 && char.IsUpper(trimmed[0]) && !trimmed.Contains("_"))
        {
            // Check if looks like a English word (letters only)
            if (Regex.IsMatch(trimmed, @"^[A-Za-z]+$"))
            {
                // Exclude common programming identifiers
                string[] reserved = { "True", "False", "Null", "None", "Default", "Left", "Right", "Top", "Bottom", "Width", "Height", "Color", "Scale", "Position", "Texture", "Sprite", "Font", "Graphics", "Device", "Input", "Keyboard", "Mouse", "GamePad", "Target", "Effect", "Value", "Index", "Count", "Length", "Size", "Name", "Type", "State", "Mode", "Level", "Version", "Time", "Date", "Player", "Server", "Client", "Buffer", "Array", "List", "Dictionary", "Vertex", "Index", "Mesh", "Normal", "Matrix", "Vector", "Light", "Camera", "Sound", "Music", "Audio" };
                foreach (var r in reserved)
                {
                    if (trimmed == r) return false;
                }
                return true;
            }
        }

        return false;
    }

    static string EscapeJson(string s)
    {
        return s.Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n")
                .Replace("\t", "\\t");
    }
}

static class StringExt
{
    public static bool AllNumbersOrSymbols(this string s)
    {
        foreach (char c in s)
        {
            if (char.IsLetter(c)) return false;
        }
        return true;
    }
}

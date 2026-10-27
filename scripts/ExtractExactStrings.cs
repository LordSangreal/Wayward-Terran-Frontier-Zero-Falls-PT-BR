using System;
using System.IO;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

class ExtractExactStrings
{
    static void Main(string[] args)
    {
        string exePath = @"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe";
        var assembly = AssemblyDefinition.ReadAssembly(exePath);

        var list = new List<string>();
        var seen = new HashSet<string>();

        foreach (var type in assembly.MainModule.Types)
        {
            ProcessType(type, list, seen);
        }

        Console.WriteLine("Total strings extracted: " + list.Count);

        using (var sw = new StreamWriter(@"d:\Tradução WTF Zero falls\scripts\exact_tutorial_strings.json", false, System.Text.Encoding.UTF8))
        {
            sw.WriteLine("[");
            for (int i = 0; i < list.Count; i++)
            {
                string s = list[i];
                string escaped = EscapeJson(s);
                sw.Write("  \"" + escaped + "\"");
                if (i < list.Count - 1) sw.WriteLine(",");
                else sw.WriteLine();
            }
            sw.WriteLine("]");
        }
    }

    static void ProcessType(TypeDefinition type, List<string> list, HashSet<string> seen)
    {
        bool isTarget = type.FullName.Contains("Tutorial") ||
                        type.FullName.Contains("OneAgent") ||
                        type.FullName.Contains("FirstFlight") ||
                        type.FullName.Contains("IntroDialogue");

        if (isTarget)
        {
            foreach (var m in type.Methods)
            {
                if (!m.HasBody) continue;
                foreach (var instr in m.Body.Instructions)
                {
                    if (instr.OpCode == OpCodes.Ldstr)
                    {
                        string s = (string)instr.Operand;
                        if (s != null && s.Length >= 2 && !seen.Contains(s))
                        {
                            if (IsDialogue(s))
                            {
                                seen.Add(s);
                                list.Add(s);
                            }
                        }
                    }
                }
            }
        }

        foreach (var nested in type.NestedTypes)
        {
            ProcessType(nested, list, seen);
        }
    }

    static bool IsDialogue(string s)
    {
        string trimmed = s.Trim();
        if (trimmed.Length < 3) return false;
        if (trimmed == "incomplete" || trimmed == "complete" || trimmed == "failed") return false;
        if (trimmed.StartsWith("station_") || trimmed.StartsWith("shipyard_")) return false;
        if (trimmed == "buyaship" || trimmed == "tutorial1" || trimmed == "first_flight" || trimmed == "airlocks_r_hard" || trimmed == "artifact_intro") return false;
        if (trimmed == "richbox" || trimmed == "pagging wrap" || trimmed == "page wrap" || trimmed == "tutorial Page" || trimmed == "header wrap") return false;
        if (trimmed == "Left" || trimmed == "Middle" || trimmed == "Close" || trimmed == "base" || trimmed == "view") return false;
        if (trimmed == "One" || trimmed == "OneL" || trimmed == "Bradley" || trimmed == "Marv" || trimmed == "Budd") return false;
        if (trimmed == "..." || trimmed == "...." || trimmed == "....." || trimmed == "......" || trimmed == ".......") return false;

        return true;
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

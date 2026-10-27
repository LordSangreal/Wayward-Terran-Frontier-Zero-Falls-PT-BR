using System;
using System.IO;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

class CollectDialogue
{
    static void Main(string[] args)
    {
        string exePath = @"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe";
        var assembly = AssemblyDefinition.ReadAssembly(exePath);

        var dialogueStrings = new HashSet<string>();

        foreach (var type in assembly.MainModule.Types)
        {
            ScanType(type, dialogueStrings);
        }

        using (var sw = new StreamWriter(@"d:\Tradução WTF Zero falls\scripts\all_dialogue_candidates.txt", false, System.Text.Encoding.UTF8))
        {
            foreach (var s in dialogueStrings)
            {
                sw.WriteLine(s);
            }
        }

        Console.WriteLine("Done. Total dialogue candidates: " + dialogueStrings.Count);
    }

    static void ScanType(TypeDefinition type, HashSet<string> set)
    {
        bool isDialogueType = type.FullName.Contains("Quest") ||
                              type.FullName.Contains("Agent") ||
                              type.FullName.Contains("Tutorial") ||
                              type.FullName.Contains("Convo") ||
                              type.FullName.Contains("Dialogue") ||
                              type.FullName.Contains("Conversation") ||
                              type.FullName.Contains("Mission");

        foreach (var method in type.Methods)
        {
            if (!method.HasBody) continue;

            bool isDialogueMethod = isDialogueType ||
                                    method.Name.IndexOf("Dialogue", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    method.Name.IndexOf("Convo", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    method.Name.IndexOf("Talk", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    method.Name.IndexOf("Quest", StringComparison.OrdinalIgnoreCase) >= 0;

            if (isDialogueMethod)
            {
                foreach (var instr in method.Body.Instructions)
                {
                    if (instr.OpCode == OpCodes.Ldstr)
                    {
                        string s = (string)instr.Operand;
                        if (IsValidDialogueString(s))
                        {
                            set.Add(s);
                        }
                    }
                }
            }
        }

        foreach (var nested in type.NestedTypes)
        {
            ScanType(nested, set);
        }
    }

    static bool IsValidDialogueString(string s)
    {
        if (string.IsNullOrEmpty(s)) return false;
        string trimmed = s.Trim();
        if (trimmed.Length < 4) return false;

        // Skip internal names / ids / assets
        if (s.StartsWith("Content/") || s.StartsWith("Content\\")) return false;
        if (s.EndsWith(".xnb") || s.EndsWith(".png") || s.EndsWith(".fx") || s.EndsWith(".wav")) return false;
        if (s.IndexOf(' ') < 0 && s.IndexOf('_') >= 0) return false; // single identifier with underscore like station_t1_repair
        if (s == "incomplete" || s == "complete" || s == "failed") return false;

        // Must look like human sentence or phrase (has a space, or punctuation, or ends with punctuation)
        if (s.Contains(" ") || s.EndsWith("?") || s.EndsWith(".") || s.EndsWith("!"))
        {
            return true;
        }

        return false;
    }
}

using System;
using System.IO;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class ScanAllSqlStrings {
    public static void Main(string[] args) {
        string origPath = @"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe";
        var asm = AssemblyDefinition.ReadAssembly(origPath);

        foreach (var t in asm.MainModule.Types) {
            ScanType(t);
        }
    }

    static void ScanType(TypeDefinition t) {
        foreach (var m in t.Methods) {
            if (!m.HasBody) continue;
            foreach (var i in m.Body.Instructions) {
                if (i.OpCode == OpCodes.Ldstr) {
                    string s = (string)i.Operand;
                    if (string.IsNullOrEmpty(s)) continue;
                    string lower = s.ToLowerInvariant().Trim();

                    // Check if it looks like SQL
                    if (lower.StartsWith("select ") || lower.StartsWith("insert ") || 
                        lower.StartsWith("update ") || lower.StartsWith("delete from") || 
                        lower.StartsWith("create table") || lower.StartsWith("alter table") ||
                        lower.StartsWith("' and ") || lower.StartsWith("', @") ||
                        lower.Contains("from characters") || lower.Contains("from sqlite") ||
                        lower.Contains("from unlocks") || lower.Contains("from modules") ||
                        lower.Contains("from turrets") || lower.Contains("from hulls")) {
                        
                        Console.WriteLine(t.FullName + "::" + m.Name + " => [" + s + "]");
                    }
                }
            }
        }

        foreach (var nested in t.NestedTypes) {
            ScanType(nested);
        }
    }
}

using System;
using System.IO;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class FindSqlTargets {
    public static void Main(string[] args) {
        string origPath = @"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe";
        var asm = AssemblyDefinition.ReadAssembly(origPath);

        var sqlTypes = new HashSet<string>();
        var sqlMethods = new HashSet<string>();

        foreach (var t in asm.MainModule.Types) {
            CheckType(t, sqlTypes, sqlMethods);
        }

        Console.WriteLine("Types using SQLite / SQL operations:");
        foreach (var t in sqlTypes) {
            Console.WriteLine("  " + t);
        }

        Console.WriteLine("\nTotal SQL methods found: " + sqlMethods.Count);
        foreach (var m in sqlMethods) {
            Console.WriteLine("  " + m);
        }
    }

    static void CheckType(TypeDefinition t, HashSet<string> sqlTypes, HashSet<string> sqlMethods) {
        foreach (var m in t.Methods) {
            if (!m.HasBody) continue;
            bool usesSql = false;
            foreach (var instr in m.Body.Instructions) {
                var mr = instr.Operand as MethodReference;
                if (mr != null) {
                    if (mr.DeclaringType.FullName.Contains("SQLite") ||
                        mr.Name == "ExecuteScalar" ||
                        mr.Name == "ExecuteReader" ||
                        mr.Name == "ExecuteNonQuery") {
                        usesSql = true;
                        break;
                    }
                }
            }
            if (usesSql) {
                sqlTypes.Add(t.FullName);
                sqlMethods.Add(m.DeclaringType.FullName + "::" + m.Name);
            }
        }

        foreach (var nested in t.NestedTypes) {
            CheckType(nested, sqlTypes, sqlMethods);
        }
    }
}

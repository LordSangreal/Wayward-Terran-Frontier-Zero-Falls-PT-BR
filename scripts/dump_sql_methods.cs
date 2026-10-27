using System;
using System.IO;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class DumpSql {
    public static void Main(string[] args) {
        string origPath = @"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe";
        string patchedPath = @"D:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe";

        Console.WriteLine("Reading original assembly: " + origPath);
        var origAsm = AssemblyDefinition.ReadAssembly(origPath);

        Console.WriteLine("Reading patched assembly: " + patchedPath);
        var patchedAsm = AssemblyDefinition.ReadAssembly(patchedPath);

        var origTypes = new Dictionary<string, TypeDefinition>();
        foreach (var t in origAsm.MainModule.Types) origTypes[t.FullName] = t;

        var patchedTypes = new Dictionary<string, TypeDefinition>();
        foreach (var t in patchedAsm.MainModule.Types) patchedTypes[t.FullName] = t;

        // Check CHARACTER_DATA in both
        if (origTypes.ContainsKey("CoOpSpRpG.CHARACTER_DATA")) {
            var origChar = origTypes["CoOpSpRpG.CHARACTER_DATA"];
            var patchChar = patchedTypes["CoOpSpRpG.CHARACTER_DATA"];

            Console.WriteLine("\n--- CHARACTER_DATA methods comparison ---");
            foreach (var om in origChar.Methods) {
                MethodDefinition pm = null;
                foreach (var m in patchChar.Methods) {
                    if (m.Name == om.Name && m.Parameters.Count == om.Parameters.Count) {
                        pm = m;
                        break;
                    }
                }

                if (om.HasBody) {
                    var oStrings = GetStrings(om);
                    var pStrings = pm != null && pm.HasBody ? GetStrings(pm) : new List<string>();

                    bool diff = false;
                    if (oStrings.Count != pStrings.Count) diff = true;
                    else {
                        for (int i = 0; i < oStrings.Count; i++) {
                            if (oStrings[i] != pStrings[i]) {
                                diff = true;
                                break;
                            }
                        }
                    }

                    if (diff || om.Name.IndexOf("research", StringComparison.OrdinalIgnoreCase) >= 0) {
                        Console.WriteLine("Method: " + om.FullName + (diff ? " [MODIFIED]" : " [UNMODIFIED]"));
                        Console.WriteLine("  ORIGINAL strings:");
                        foreach (var s in oStrings) Console.WriteLine("    \"" + s + "\"");
                        Console.WriteLine("  PATCHED strings:");
                        foreach (var s in pStrings) Console.WriteLine("    \"" + s + "\"");
                    }
                }
            }
        }

        // Also check any method in the entire assembly that uses SQLite or ExecuteScalar/ExecuteReader/ExecuteNonQuery
        Console.WriteLine("\n--- Searching all methods with SQLite or SQL execution ---");
        foreach (var t in origAsm.MainModule.Types) {
            CheckTypeForSql(t, patchedTypes);
        }
    }

    static void CheckTypeForSql(TypeDefinition origType, Dictionary<string, TypeDefinition> patchedTypes) {
        TypeDefinition patchType = null;
        patchedTypes.TryGetValue(origType.FullName, out patchType);

        foreach (var om in origType.Methods) {
            if (!om.HasBody) continue;
            bool usesSql = false;
            foreach (var instr in om.Body.Instructions) {
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
                MethodDefinition pm = null;
                if (patchType != null) {
                    foreach (var m in patchType.Methods) {
                        if (m.Name == om.Name && m.Parameters.Count == om.Parameters.Count) {
                            pm = m;
                            break;
                        }
                    }
                }

                var oStrings = GetStrings(om);
                var pStrings = pm != null && pm.HasBody ? GetStrings(pm) : new List<string>();

                bool diff = false;
                if (oStrings.Count != pStrings.Count) diff = true;
                else {
                    for (int i = 0; i < oStrings.Count; i++) {
                        if (oStrings[i] != pStrings[i]) {
                            diff = true;
                            break;
                        }
                    }
                }

                Console.WriteLine("SQL Method: " + om.FullName + (diff ? " [MODIFIED! CRITICAL!]" : " [OK]"));
                if (diff) {
                    for (int i = 0; i < Math.Max(oStrings.Count, pStrings.Count); i++) {
                        string o = i < oStrings.Count ? oStrings[i] : "<none>";
                        string p = i < pStrings.Count ? pStrings[i] : "<none>";
                        if (o != p) {
                            Console.WriteLine("   ORIG: \"" + o + "\"");
                            Console.WriteLine("   MOD : \"" + p + "\"");
                        }
                    }
                }
            }
        }

        foreach (var nested in origType.NestedTypes) {
            CheckTypeForSql(nested, patchedTypes);
        }
    }

    static List<string> GetStrings(MethodDefinition m) {
        var list = new List<string>();
        foreach (var instr in m.Body.Instructions) {
            if (instr.OpCode == OpCodes.Ldstr) {
                list.Add((string)instr.Operand);
            }
        }
        return list;
    }
}

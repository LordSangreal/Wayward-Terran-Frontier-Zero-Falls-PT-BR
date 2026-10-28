using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

class Program
{
    static void Main(string[] args)
    {
        string exePath = @"D:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe";
        var asm = AssemblyDefinition.ReadAssembly(exePath);

        // Find methods named "AddMessage", "Log", "Chat", "Print", "PostMessage", "Notify", "HUD"
        var chatCallers = new List<string>();

        foreach (var t in asm.MainModule.Types)
        {
            foreach (var m in t.Methods)
            {
                if (!m.HasBody) continue;
                for (int i = 0; i < m.Body.Instructions.Count; i++)
                {
                    var inst = m.Body.Instructions[i];
                    if (inst.OpCode == OpCodes.Call || inst.OpCode == OpCodes.Callvirt)
                    {
                        var target = inst.Operand as MethodReference;
                        if (target != null)
                        {
                            string tname = target.Name.ToLower();
                            if (tname.Contains("addmessage") || tname.Contains("addchat") || 
                                tname.Contains("addlog") || tname.Contains("addentry") ||
                                (target.DeclaringType.Name.Contains("Chat") && tname.Contains("add")) ||
                                (target.DeclaringType.Name.Contains("Log") && tname.Contains("add")))
                            {
                                // Look back for ldstr instructions in the preceding 5 instructions
                                for (int j = Math.Max(0, i - 6); j < i; j++)
                                {
                                    if (m.Body.Instructions[j].OpCode == OpCodes.Ldstr)
                                    {
                                        string s = m.Body.Instructions[j].Operand as string;
                                        if (!string.IsNullOrEmpty(s) && s.Length > 2)
                                        {
                                            chatCallers.Add(string.Format("{0}::{1} -> calls {2}::{3} with '{4}'",
                                                t.Name, m.Name, target.DeclaringType.Name, target.Name, s.Replace("\n", "\\n")));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        Console.WriteLine("Total chat/log calls with ldstr: " + chatCallers.Count);
        var unique = chatCallers.Distinct().ToList();
        File.WriteAllLines("chat_callers.txt", unique);
        foreach (var c in unique.Take(40))
        {
            Console.WriteLine(c);
        }
    }
}

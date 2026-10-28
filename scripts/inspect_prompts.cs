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

        // Find classes that implement interaction or have DrawPrompt/Draw
        string[] types = new string[] {
            "CoOpSpRpG.Airlock",
            "CoOpSpRpG.Door",
            "CoOpSpRpG.ConsoleAccess",
            "CoOpSpRpG.Bedroom",
            "CoOpSpRpG.InteractiveRoom",
            "CoOpSpRpG.WidgetPrompt"
        };

        foreach (var name in types)
        {
            var t = asm.MainModule.Types.FirstOrDefault(x => x.FullName == name);
            if (t == null) continue;
            Console.WriteLine("Type: " + t.FullName);
            foreach (var m in t.Methods)
            {
                if (!m.HasBody) continue;
                var ldstrs = m.Body.Instructions.Where(i => i.OpCode == OpCodes.Ldstr).Select(i => (string)i.Operand).ToList();
                if (ldstrs.Count > 0)
                {
                    Console.WriteLine(string.Format("  Method {0}: {1} strings: {2}", m.Name, ldstrs.Count, string.Join(" | ", ldstrs)));
                }
            }
        }
    }
}

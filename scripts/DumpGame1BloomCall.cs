using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

class DumpGame1BloomCall
{
    static void Main()
    {
        var asmOrig = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");

        foreach (var t in asmOrig.MainModule.Types)
        {
            if (t.Name == "Game1")
            {
                foreach (var m in t.Methods)
                {
                    if (m.Name == "loadStuff")
                    {
                        for (int i = 0; i < m.Body.Instructions.Count; i++)
                        {
                            var instr = m.Body.Instructions[i];
                            if (instr.OpCode == OpCodes.Newobj)
                            {
                                var mr = instr.Operand as MethodReference;
                                if (mr != null && mr.DeclaringType.Name.Contains("Bloom"))
                                {
                                    Console.WriteLine("FOUND NEWOBJ AT " + i + ": " + mr.FullName);
                                    for (int j = Math.Max(0, i - 10); j <= i + 2; j++)
                                    {
                                        Console.WriteLine("  " + m.Body.Instructions[j]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

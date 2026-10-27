using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

class Program
{
    static void Main()
    {
        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");
        foreach(var t in asm.MainModule.Types)
        {
            foreach(var nested in t.NestedTypes)
            {
                if (nested.Name.Contains("<>c"))
                {
                    foreach(var m in nested.Methods)
                    {
                        if (m.Name.Contains("getFirstDialogue"))
                        {
                            Console.WriteLine("Method: " + m.FullName);
                            foreach(var instr in m.Body.Instructions)
                            {
                                Console.WriteLine(instr.ToString());
                            }
                        }
                    }
                }
            }
        }
    }
}

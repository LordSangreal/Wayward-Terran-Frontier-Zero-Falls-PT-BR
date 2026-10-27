using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

class FindModUploadCalls
{
    static void Main()
    {
        var asm = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");

        foreach (var t in asm.MainModule.Types)
        {
            foreach (var m in t.Methods)
            {
                if (!m.HasBody) continue;
                foreach (var i in m.Body.Instructions)
                {
                    if (i.OpCode == OpCodes.Newobj)
                    {
                        var mr = i.Operand as MethodReference;
                        if (mr != null && mr.DeclaringType.Name == "ModUploadScreen")
                        {
                            Console.WriteLine("ModUploadScreen instantiated in: " + t.FullName + "." + m.Name);
                        }
                    }
                }
            }
        }
    }
}

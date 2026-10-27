using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

class DumpTutorialBag
{
    static void Main()
    {
        var asmOrig = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");

        foreach (var t in asmOrig.MainModule.Types)
        {
            if (t.Name.Contains("TUTORIAL_BAG") || t.Name.Contains("TutorialTemplate"))
            {
                foreach (var m in t.Methods)
                {
                    Console.WriteLine("================ " + t.Name + "." + m.Name + " ================");
                    if (m.HasBody)
                    {
                        foreach (var i in m.Body.Instructions)
                        {
                            Console.WriteLine("  " + i.ToString());
                        }
                    }
                }
            }
        }
    }
}

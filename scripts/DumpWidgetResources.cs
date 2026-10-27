using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

class DumpWidgetResources
{
    static void Main()
    {
        var asmOrig = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");

        foreach (var t in asmOrig.MainModule.Types)
        {
            if (t.Name.Contains("WidgetResources"))
            {
                foreach (var m in t.Methods)
                {
                    if (m.Name.Contains("LoadTutorials") || m.Name.Contains("Tutorial"))
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
}

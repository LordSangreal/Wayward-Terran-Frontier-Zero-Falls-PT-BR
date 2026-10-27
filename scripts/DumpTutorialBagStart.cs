using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

class DumpTutorialBagStart
{
    static void Main()
    {
        var asmPatch = AssemblyDefinition.ReadAssembly(@"D:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe");

        foreach (var t in asmPatch.MainModule.Types)
        {
            if (t.Name == "TUTORIAL_BAG")
            {
                foreach (var m in t.Methods)
                {
                    if (m.Name == "Init")
                    {
                        for (int i = 0; i < 35; i++)
                        {
                            Console.WriteLine(m.Body.Instructions[i].ToString());
                        }
                    }
                }
            }
        }
    }
}

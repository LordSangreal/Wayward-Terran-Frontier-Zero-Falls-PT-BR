using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

class DumpBloomCtor
{
    static void Main()
    {
        var asmOrig = AssemblyDefinition.ReadAssembly(@"d:\Tradução WTF Zero falls\backup_game_original\WTF.exe");
        var asmPatch = AssemblyDefinition.ReadAssembly(@"D:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe");

        foreach (var t in asmPatch.MainModule.Types)
        {
            if (t.Name == "BloomPostProcess")
            {
                foreach (var m in t.Methods)
                {
                    if (m.Name == ".ctor")
                    {
                        Console.WriteLine("BloomPostProcess..ctor:");
                        foreach (var i in m.Body.Instructions)
                        {
                            Console.WriteLine(i.ToString());
                        }
                    }
                }
            }
        }
    }
}

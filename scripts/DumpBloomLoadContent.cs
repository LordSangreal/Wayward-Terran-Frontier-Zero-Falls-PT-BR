using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

class DumpBloomLoadContent
{
    static void Main()
    {
        var asmPatch = AssemblyDefinition.ReadAssembly(@"D:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe");

        foreach (var t in asmPatch.MainModule.Types)
        {
            if (t.Name == "BloomPostProcess")
            {
                foreach (var m in t.Methods)
                {
                    if (m.Name == "LoadContent")
                    {
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

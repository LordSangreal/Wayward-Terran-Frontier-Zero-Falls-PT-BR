using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

class DumpFullTemplateCtor
{
    static void Main()
    {
        var asm = AssemblyDefinition.ReadAssembly(@"D:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe");

        foreach (var t in asm.MainModule.Types)
        {
            if (t.Name == "TutorialTemplate")
            {
                foreach (var m in t.Methods)
                {
                    if (m.Name == ".ctor" && m.Parameters.Count == 2)
                    {
                        Console.WriteLine("TutorialTemplate..ctor(XmlDocument, string):");
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

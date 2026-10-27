using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class VerifyPatchedMenu {
    public static void Main() {
        var asm = AssemblyDefinition.ReadAssembly(@"D:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe");
        foreach (var t in asm.MainModule.Types) {
            if (t.Name == "RootMenuRev2") {
                foreach (var m in t.Methods) {
                    if (m.Name == "createElements") {
                        foreach (var i in m.Body.Instructions) {
                            if (i.OpCode == OpCodes.Ldstr) {
                                string s = (string)i.Operand;
                                if (s == "Campanha" || s == "Arena" || s == "Opcoes" || s == "Configuracoes" || 
                                    s == "Extra" || s == "Teclas" || s == "Atalhos de teclado" || 
                                    s == "Video e Geral" || s == "Selecionar Nave" || s == "Editar Nave" || s == "Wiki") {
                                    Console.WriteLine("Button: [" + s + "]");
                                }
                            }
                        }
                    }
                }
            }
            if (t.Name == "CONFIG") {
                foreach (var m in t.Methods) {
                    if (m.Name == ".cctor") {
                        foreach (var i in m.Body.Instructions) {
                            if (i.OpCode == OpCodes.Ldstr) {
                                Console.WriteLine("CONFIG string: [" + i.Operand + "]");
                            }
                        }
                    }
                }
            }
        }
    }
}

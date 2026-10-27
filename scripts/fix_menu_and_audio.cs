using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class FixMenuAndAudio {
    public static void Main() {
        FixFile(@"d:\Tradução WTF Zero falls\translations_exe.json");
        FixFile(@"d:\Tradução WTF Zero falls\patcher\translations_exe.json");
    }

    static void FixFile(string path) {
        if (!File.Exists(path)) return;
        string[] lines = File.ReadAllLines(path);
        var regex = new Regex(@"^\s*""((?:\\.|[^""\\])*)""\s*:\s*""((?:\\.|[^""\\])*)""");
        
        var outputLines = new List<string>();

        foreach (var line in lines) {
            var m = regex.Match(line);
            if (m.Success) {
                string k = Regex.Unescape(m.Groups[1].Value);
                string v = Regex.Unescape(m.Groups[2].Value);

                // 1. Remove dangerous Fire cue
                if (k == "Fire") {
                    Console.WriteLine("Removed [Fire] from " + path);
                    continue; // Skip
                }

                // 2. Fix menu buttons
                if (k == "Settings") {
                    outputLines.Add("  \"Settings\": \"Opcoes\",");
                    continue;
                }
                if (k == "Keybinds") {
                    outputLines.Add("  \"Keybinds\": \"Teclas\",");
                    continue;
                }
                if (k == "Video & General") {
                    outputLines.Add("  \"Video & General\": \"Video e Geral\",");
                    continue;
                }
                if (k == "Select Ship") {
                    outputLines.Add("  \"Select Ship\": \"Selecionar Nave\",");
                    continue;
                }
                if (k == "Edit Ship") {
                    outputLines.Add("  \"Edit Ship\": \"Editar Nave\",");
                    continue;
                }
                if (k.StartsWith("Continued development of this game")) {
                    outputLines.Add("  \"Continued development of this game brought to you by hatred, mental illness, and sunk cost fallacy. Come visit our discord if you'd like to help test multiplayer.\": \"O desenvolvimento continuo deste jogo e oferecido a voce por odio, doenca mental e falacia dos custos irrecuperaveis. Visite nosso Discord se quiser ajudar a testar o multijogador.\",");
                    continue;
                }
                if (k == "Wiki") {
                    outputLines.Add("  \"Wiki\": \"Wiki\",");
                    continue;
                }
                if (k == "Load") {
                    outputLines.Add("  \"Load\": \"Carregar\",");
                    continue;
                }
                if (k == "Enter name") {
                    outputLines.Add("  \"Enter name\": \"Digitar nome\",");
                    continue;
                }
                if (k == "Exit") {
                    outputLines.Add("  \"Exit\": \"Sair\",");
                    continue;
                }
                if (k == "Upload flotila") {
                    outputLines.Add("  \"Upload flotila\": \"Enviar Flotilha\",");
                    continue;
                }
                if (k == "Sandbox") {
                    outputLines.Add("  \"Sandbox\": \"Modo Livre\",");
                    continue;
                }
            }
            outputLines.Add(line);
        }

        // Clean trailing comma
        for (int i = outputLines.Count - 1; i >= 0; i--) {
            string trimmed = outputLines[i].Trim();
            if (trimmed == "}") {
                for (int j = i - 1; j >= 0; j--) {
                    if (!string.IsNullOrEmpty(outputLines[j].Trim())) {
                        if (outputLines[j].EndsWith(",")) {
                            outputLines[j] = outputLines[j].TrimEnd(',');
                        }
                        break;
                    }
                }
                break;
            }
        }

        File.WriteAllLines(path, outputLines);
        Console.WriteLine("Updated " + path + " successfully.");
    }
}

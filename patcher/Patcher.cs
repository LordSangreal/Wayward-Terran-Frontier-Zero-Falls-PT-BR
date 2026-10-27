using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class Patcher {
    public static int Main(string[] args) {
        Console.WriteLine("==========================================================");
        Console.WriteLine("   Tradutor PT-BR - Wayward Terran Frontier: Zero Falls   ");
        Console.WriteLine("==========================================================");
        Console.WriteLine();

        string appDir = AppDomain.CurrentDomain.BaseDirectory;
        string dictFile = Path.Combine(appDir, "translations_exe.json");
        string blacklistFile = Path.Combine(appDir, "shader_blacklist.json");

        if (!File.Exists(dictFile)) {
            // Tenta procurar na pasta do patcher ou pai
            dictFile = Path.Combine(appDir, @"patcher\translations_exe.json");
            blacklistFile = Path.Combine(appDir, @"patcher\shader_blacklist.json");
        }

        if (!File.Exists(dictFile)) {
            Console.WriteLine("[ERRO] Arquivo de traducoes nao encontrado: " + dictFile);
            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadKey();
            return 1;
        }

        string gameDir = "";
        if (args.Length > 0 && Directory.Exists(args[0])) {
            gameDir = args[0];
        } else {
            // Tenta detectar automaticamente
            string[] possiblePaths = new string[] {
                Directory.GetCurrentDirectory(),
                appDir,
                @"D:\SteamLibrary\steamapps\common\Zero Falls",
                @"C:\Program Files (x86)\Steam\steamapps\common\Zero Falls",
                @"C:\SteamLibrary\steamapps\common\Zero Falls",
                @"E:\SteamLibrary\steamapps\common\Zero Falls",
                @"F:\SteamLibrary\steamapps\common\Zero Falls"
            };

            foreach (var path in possiblePaths) {
                if (File.Exists(Path.Combine(path, "WTF.exe"))) {
                    gameDir = path;
                    break;
                }
            }
        }

        if (string.IsNullOrEmpty(gameDir) || !File.Exists(Path.Combine(gameDir, "WTF.exe"))) {
            Console.WriteLine("Nao foi possivel detectar a pasta do jogo automaticamente.");
            Console.Write("Por favor, digite o caminho da pasta onde o jogo esta instalado: ");
            gameDir = Console.ReadLine().Trim('\"', ' ');
        }

        string gameExe = Path.Combine(gameDir, "WTF.exe");
        if (!File.Exists(gameExe)) {
            Console.WriteLine("[ERRO] WTF.exe nao foi encontrado em: " + gameDir);
            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadKey();
            return 1;
        }

        Console.WriteLine("Pasta do jogo detectada: " + gameDir);

        string backupExe = Path.Combine(gameDir, "WTF_backup.exe");
        if (!File.Exists(backupExe)) {
            Console.WriteLine("Criando backup do executavel original em: " + backupExe);
            File.Copy(gameExe, backupExe, false);
        } else {
            Console.WriteLine("Backup original ja existe em: " + backupExe);
        }

        Console.WriteLine("Carregando dicionario de traducoes...");
        var dict = LoadDictionary(dictFile);
        Console.WriteLine("Termos seguros carregados: " + dict.Count);

        var blacklist = LoadBlacklist(blacklistFile);
        Console.WriteLine("Blacklist de shaders carregada: " + blacklist.Count + " termos protegidos.");

        Console.WriteLine("Aplicando patch no executavel...");
        var readerParams = new ReaderParameters { ReadWrite = false };
        var asm = AssemblyDefinition.ReadAssembly(backupExe, readerParams);

        int replaced = 0;
        foreach (var type in asm.MainModule.Types) {
            replaced += ProcessType(type, dict, blacklist);
        }

        Console.WriteLine("Total de instrucoes ldstr modificadas com seguranca: " + replaced);
        string tempOut = Path.Combine(gameDir, "WTF_patched_temp.exe");
        asm.Write(tempOut);

        File.Copy(tempOut, gameExe, true);
        File.Delete(tempOut);
        Console.WriteLine();
        Console.WriteLine("==========================================================");
        Console.WriteLine("[SUCESSO] WTF.exe traduzido e protegido gravado com exito!");
        Console.WriteLine("==========================================================");
        return 0;
    }

    static int ProcessType(TypeDefinition type, Dictionary<string, string> dict, HashSet<string> blacklist) {
        int count = 0;
        foreach (var m in type.Methods) {
            if (!m.HasBody) continue;

            // Protecao CRITICA contra shaders, tecnicas e render targets de navegacao grafica
            if (m.Name == "drawBumpyStuff" || m.Name == "combinePasses" || m.Name == "applyToScreen" || m.Name == "drawGraphs") {
                continue;
            }

            // Protecao CRITICA para inicializacao de Content / Assets do MonoGame
            if (type.Name == "Game1" && (m.Name == ".ctor" || m.Name == "Initialize")) {
                continue;
            }

            // Tratamento especifico e seguro para o WidgetShipStatus (HUD de barras da nave)
            if (type.Name == "WidgetShipStatus" && m.Name == "CreateElemenets") {
                foreach (var i in m.Body.Instructions) {
                    if (i.OpCode == OpCodes.Ldstr) {
                        string val = (string)i.Operand;
                        if (val == "Shield") { i.Operand = "Escudo"; count++; }
                        else if (val == "Structure") { i.Operand = "Estrutura"; count++; }
                        else if (val == "Engines") { i.Operand = "Motores"; count++; }
                        else if (val == "Thrusters") { i.Operand = "Propulsores"; count++; }
                    }
                }
                continue;
            }

            // Para todos os outros metodos, aplicar o dicionario seguro
            foreach (var i in m.Body.Instructions) {
                if (i.OpCode == OpCodes.Ldstr) {
                    string s = i.Operand as string;
                    if (s == null) continue;

                    // Protecao CRITICA contra SQL, SQLite e connection strings
                    if (IsSqlOrTechnical(s)) continue;

                    // Se o termo estiver na blacklist de shaders e nao for um texto longo, nao substituir!
                    if (blacklist.Contains(s) && !s.Contains(" ")) {
                        continue;
                    }

                    if (dict.ContainsKey(s)) {
                        string newVal = dict[s];
                        if (s != newVal) {
                            i.Operand = newVal;
                            count++;
                        }
                    }
                }
            }
        }
        foreach (var nested in type.NestedTypes) {
            count += ProcessType(nested, dict, blacklist);
        }
        return count;
    }

    static bool IsSqlOrTechnical(string s) {
        if (string.IsNullOrEmpty(s)) return true;
        string lower = s.ToLowerInvariant().Trim();

        // Pasta Content do MonoGame
        if (lower == "content") return true;

        // SQLite connection strings & databases
        if (lower.Contains("data source=") || lower.Contains("version=3;") || lower.EndsWith(".sqlite") || lower.EndsWith(".db"))
            return true;

        // SQL DDL & DML
        if (lower.StartsWith("create table") || lower.StartsWith("delete from") || lower.StartsWith("insert into") || lower.StartsWith("drop table") || lower.StartsWith("pragma "))
            return true;

        if (lower.StartsWith("select ") && (lower.Contains(" from ") || lower.Contains(" count(") || lower.Contains(" where ")))
            return true;

        if (lower.Contains("primary key") || lower.Contains("values (") || lower.Contains("values("))
            return true;

        return false;
    }

    static Dictionary<string, string> LoadDictionary(string jsonPath) {
        var dict = new Dictionary<string, string>();
        string[] lines = File.ReadAllLines(jsonPath);
        var regex = new Regex(@"^\s*""((?:\\.|[^""\\])*)""\s*:\s*""((?:\\.|[^""\\])*)""");
        foreach (var line in lines) {
            var match = regex.Match(line);
            if (match.Success) {
                string k = Regex.Unescape(match.Groups[1].Value);
                string v = Regex.Unescape(match.Groups[2].Value);
                dict[k] = v;
            }
        }
        return dict;
    }

    static HashSet<string> LoadBlacklist(string jsonPath) {
        var set = new HashSet<string>();
        if (!File.Exists(jsonPath)) return set;
        string[] lines = File.ReadAllLines(jsonPath);
        var regex = new Regex(@"^\s*""((?:\\.|[^""\\])*)""");
        foreach (var line in lines) {
            var match = regex.Match(line);
            if (match.Success) {
                string val = Regex.Unescape(match.Groups[1].Value);
                set.Add(val);
            }
        }
        return set;
    }
}

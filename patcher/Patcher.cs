using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class Patcher {
    public static int Main(string[] args) {
        Console.WriteLine("==========================================================");
        Console.WriteLine("   Tradutor PT-BR v1.0.1 - Wayward Terran Frontier        ");
        Console.WriteLine("==========================================================");
        Console.WriteLine();

        string appDir = AppDomain.CurrentDomain.BaseDirectory;
        string dictFile = FindFile(appDir, "translations_exe.json");
        string blacklistFile = FindFile(appDir, "shader_blacklist.json");

        if (string.IsNullOrEmpty(dictFile) || !File.Exists(dictFile)) {
            Console.WriteLine("[ERRO] Arquivo translations_exe.json nao encontrado!");
            return 1;
        }

        string gameDir = "";
        if (args.Length > 0 && Directory.Exists(args[0])) {
            gameDir = args[0];
        } else {
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
            gameDir = Console.ReadLine();
            if (!string.IsNullOrEmpty(gameDir)) gameDir = gameDir.Trim('\"', ' ');
        }

        string gameExe = Path.Combine(gameDir, "WTF.exe");
        if (!File.Exists(gameExe)) {
            Console.WriteLine("[ERRO] WTF.exe nao foi encontrado em: " + gameDir);
            return 1;
        }

        Console.WriteLine("Pasta do jogo detectada: " + gameDir);

        string backupExe = Path.Combine(gameDir, "WTF_backup.exe");
        var gameFileInfo = new FileInfo(gameExe);
        bool needsNewBackup = false;

        if (!File.Exists(backupExe)) {
            needsNewBackup = true;
        } else {
            var backupFileInfo = new FileInfo(backupExe);
            if (gameFileInfo.Length != backupFileInfo.Length) {
                // Tamanhos diferentes: verificar se gameExe atual e um original limpo da Steam
                if (CheckIfOriginal(gameExe)) {
                    Console.WriteLine("[AVISO] Nova versao do jogo detectada na Steam!");
                    string oldBackup = Path.Combine(gameDir, "WTF_backup_anterior.exe");
                    try {
                        if (File.Exists(oldBackup)) File.Delete(oldBackup);
                        File.Move(backupExe, oldBackup);
                        Console.WriteLine("Backup da versao anterior arquivado em: " + oldBackup);
                    } catch (Exception ex) {
                        Console.WriteLine("Aviso ao mover backup anterior: " + ex.Message);
                    }
                    needsNewBackup = true;
                }
            }
        }

        if (needsNewBackup) {
            Console.WriteLine("Criando backup do executavel original em: " + backupExe);
            File.Copy(gameExe, backupExe, true);
        } else {
            Console.WriteLine("Backup original confirmado em: " + backupExe);
        }

        Console.WriteLine("Carregando dicionario de traducoes: " + dictFile);
        var dict = LoadDictionary(dictFile);
        Console.WriteLine("Termos seguros carregados: " + dict.Count);

        var blacklist = LoadBlacklist(blacklistFile);
        Console.WriteLine("Blacklist de shaders carregada: " + blacklist.Count + " termos protegidos.");

        Console.WriteLine("Aplicando patch no executavel...");
        var readerParams = new ReaderParameters { ReadWrite = false };
        var asm = AssemblyDefinition.ReadAssembly(backupExe, readerParams);

        // Traducao dos nomes de Raridade de Biomas/Recursos no Enum Rarity
        foreach (var type in asm.MainModule.Types) {
            if (type.FullName == "CoOpSpRpG.Rarity") {
                foreach (var f in type.Fields) {
                    if (f.Name == "Common") f.Name = "Comum";
                    else if (f.Name == "Uncommon") f.Name = "Incomum";
                    else if (f.Name == "Rare") f.Name = "Raro";
                    else if (f.Name == "Exotic") f.Name = "Exotico";
                }
            }
            // Desbloquear Singleplayer/Campanha se a versao de teste veio com openMP travado
            if (type.FullName == "CoOpSpRpG.CONFIG") {
                foreach (var m in type.Methods) {
                    if (m.Name == ".cctor") {
                        for (int i = 0; i < m.Body.Instructions.Count; i++) {
                            var instr = m.Body.Instructions[i];
                            if (instr.OpCode == OpCodes.Stsfld && instr.Operand != null && instr.Operand.ToString().Contains("openMP")) {
                                m.Body.Instructions[i - 1].OpCode = OpCodes.Ldc_I4_0;
                            }
                        }
                    }
                }
            }

            // Correcao de compatibilidade de save antigo (auto-migracao de tabelas SQLite como professions)
            if (type.FullName == "CoOpSpRpG.CHARACTER_DATA") {
                foreach (var m in type.Methods) {
                    if (m.Name == "loadWorldCharacter") {
                        var il = m.Body.GetILProcessor();
                        MethodDefinition createAll = null;
                        foreach (var method in type.Methods) {
                            if (method.Name == "createAllTables") {
                                createAll = method;
                                break;
                            }
                        }
                        for (int i = 0; i < m.Body.Instructions.Count; i++) {
                            var instr = m.Body.Instructions[i];
                            if (instr.OpCode == OpCodes.Callvirt && instr.Operand != null && instr.Operand.ToString().Contains("DbConnection::Open")) {
                                if (i > 55 && createAll != null) {
                                    var callCreate = il.Create(OpCodes.Call, createAll);
                                    il.InsertAfter(instr, callCreate);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        int replaced = 0;
        foreach (var type in asm.MainModule.Types) {
            replaced += ProcessType(type, dict, blacklist);
        }

        Console.WriteLine("Total de instrucoes ldstr modificadas com seguranca: " + replaced);
        string tempOut = Path.Combine(gameDir, "WTF_patched_temp.exe");
        asm.Write(tempOut);
        asm.Dispose();

        File.Copy(tempOut, gameExe, true);
        File.Delete(tempOut);
        Console.WriteLine();
        Console.WriteLine("==========================================================");
        Console.WriteLine("[SUCESSO] WTF.exe traduzido e protegido gravado com exito!");
        Console.WriteLine("==========================================================");
        return 0;
    }

    static string FindFile(string startDir, string fileName) {
        string[] candidates = new string[] {
            Path.Combine(startDir, fileName),
            Path.Combine(startDir, "patcher", fileName),
            Path.Combine(startDir, "..", fileName),
            Path.Combine(startDir, "..", "patcher", fileName),
            Path.Combine(startDir, "..", "scripts", fileName),
            Path.Combine(@"d:\Tradução WTF Zero falls", fileName),
            Path.Combine(@"d:\Tradução WTF Zero falls", "patcher", fileName)
        };
        foreach (var c in candidates) {
            if (File.Exists(c)) return Path.GetFullPath(c);
        }
        return null;
    }

    static int ProcessType(TypeDefinition type, Dictionary<string, string> dict, HashSet<string> blacklist) {
        int count = 0;

        // Protecao CRITICA para XML parsing e templates de tutorial
        if (type.Name == "TutorialTemplate" || type.Name == "TUTORIAL_BAG") {
            return 0;
        }

        // Protecao CRITICA contra modificacao de banco de dados SQLite
        if (type.Name == "WorldDatabase") {
            return 0;
        }

        // Protecao CRITICA para estagios de quest (nao alterar spawn points nem marcadores de mapa)
        if (type.Name == "AttackPlayerWithNonFactionStage") {
            return 0;
        }

        // Protecao CRITICA para classes de carregamento de arte e assets graficos
        if (type.Name == "LoaderProc" || type.Name == "MenuBackground" || type.Name == "MenuBackgroundV2" || type.Name == "MONSTERBAG") {
            return 0;
        }
        if (type.BaseType != null && type.BaseType.Name == "ParticleEffect") {
            return 0;
        }

        // Protecao CRITICA para CHARACTER_DATA: traduzir apenas notificacoes ao jogador, NUNCA instrucoes SQL!
        if (type.Name == "CHARACTER_DATA") {
            foreach (var m in type.Methods) {
                if (!m.HasBody) continue;
                foreach (var i in m.Body.Instructions) {
                    if (i.OpCode == OpCodes.Ldstr) {
                        string val = i.Operand as string;
                        if (val == null) continue;
                        if (IsSafeCharacterDataString(val) && dict.ContainsKey(val)) {
                            i.Operand = dict[val];
                            count++;
                        }
                    }
                }
            }
            return count;
        }

        foreach (var m in type.Methods) {
            if (!m.HasBody) continue;

            // Protecao CRITICA contra shaders, tecnicas e render targets de navegacao grafica
            if (m.Name == "drawBumpyStuff" || m.Name == "combinePasses" || m.Name == "applyToScreen" || m.Name == "drawGraphs") {
                continue;
            }

            // Protecao CRITICA para metodos de carregamento de conteudo e efeitos de particula
            if (m.Name == "LoadContent" || m.Name == "loadContent" || m.Name == "LoadParticleEffect") {
                continue;
            }

            // Protecao CRITICA para inicializacao de Content / Assets do MonoGame
            if (type.Name == "Game1" && (m.Name == ".ctor" || m.Name == "Initialize")) {
                continue;
            }

            // Protecao CRITICA para audio cues do XACT / MonoGame SoundBank
            if (type.Name == "LoaderProc" && m.Name == "loadSound") {
                continue;
            }
            if (type.Name == "ShipNavigationRev3" && (m.Name == "startSounds" || m.Name == "spawnFire")) {
                continue;
            }
            if (type.Name == "Ship" && m.Name == "playTravelCharge") {
                continue;
            }
            if (type.Name == "HackingScreen" && m.Name == "Update") {
                continue;
            }

            // Protecao CRITICA para categorias de modulos no projetista de naves
            if ((type.Name == "ShipDesignRev4" || type.Name == "ShipDesignRev3") && (m.Name == "InitCategories" || m.Name == "Init")) {
                continue;
            }

            // Protecao CRITICA para serializacao binaria de agentes / NPCs
            if (m.Name == "writeData") {
                continue;
            }

            // Protecao CRITICA para inicializacao de quests (nomes e IDs internos)
            if (type.BaseType != null && type.BaseType.Name == "TriggerEvent" && m.Name == ".ctor") {
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

    static bool IsSafeCharacterDataString(string s) {
        return s == "Character name already exists" ||
               s == "Character name error" ||
               s == "Level up! You are now level " ||
               s == "New research available. Press " ||
               s == " to open research" ||
               s == "New ship research available" ||
               s == "Ship hull unlocked" ||
               s == "Ship design added" ||
               s == "variant " ||
               s == "Tab " ||
               s == "Failed to upgrade an old profile";
    }

    static bool CheckIfOriginal(string exePath) {
        try {
            var asm = AssemblyDefinition.ReadAssembly(exePath);
            foreach (var type in asm.MainModule.Types) {
                if (type.FullName == "CoOpSpRpG.Rarity") {
                    foreach (var f in type.Fields) {
                        if (f.Name == "Common") return true;
                        if (f.Name == "Comum") return false;
                    }
                }
            }
        } catch { }
        return true;
    }

    static bool IsSqlOrTechnical(string s) {
        if (string.IsNullOrEmpty(s)) return true;
        string lower = s.ToLowerInvariant().Trim();

        // Tags XML reservadas da engine
        if (lower == "name" || lower == "page" || lower == "img" || lower == "src" || lower == "body" || 
            lower == "type" || lower == "color" || lower == "font" || lower == "text" || lower == "bind")
            return true;

        // Pasta Content do MonoGame, WMI e caminhos de assets
        if (lower == "content" || lower == @"root\cimv2" || lower.StartsWith("content/")) return true;

        // Cues de audio do XACT / MonoGame SoundBank
        if (lower == "fire" || lower == "warpspool" || lower == "klaxon_loop" || lower == "amb_reactor-loop") return true;

        // Marcadores de mapa (POI), waypoints e identificadores internos de quests
        if (lower == "introduction" || lower.StartsWith("waypoint_") || lower == "badguyspawn" || 
            lower == "pest" || lower == "hacking" || lower == "ship navigation" ||
            lower == "station_t1_repair" || lower == "ssc_drone_hive" || lower == "repair tutorial" ||
            lower == "farming" || lower == "feed the rats" || lower == "null quest" || lower == "null quest name" ||
            lower == "3 ssc bosses")
            return true;

        // Cues de audio e nomes de agentes tecnicos
        if (lower == "missile" || lower == "budd" || lower == "mick" || lower == "shady dealer" ||
            lower == "robo barkeep 880" || lower == "robo barkeep 890")
            return true;

        // Logs de rede / thread / exceptions tecnicas
        if (lower.StartsWith("net thread") || lower.StartsWith("server process") || lower.StartsWith("error running process") || lower.StartsWith("exception when sending"))
            return true;

        // Extensoes de arquivo e caminhos de diretorio
        if (s.StartsWith(".") && s.Length <= 5 && !s.Contains(" "))
            return true;
        if ((s.StartsWith("\\") || s.StartsWith("/")) && (s.EndsWith("\\") || s.EndsWith("/")))
            return true;
        if (lower.EndsWith(".xwb") || lower.EndsWith(".xsb") || lower.EndsWith(".xnb") || lower.EndsWith(".png") || lower.EndsWith(".dll"))
            return true;

        // SQLite connection strings & databases
        if (lower.Contains("data source=") || lower.Contains("version=3;") || lower.EndsWith(".sqlite") || lower.EndsWith(".db"))
            return true;

        // SQL DDL & DML
        if (lower.StartsWith("create table") || lower.StartsWith("delete from") || lower.StartsWith("insert into") || lower.StartsWith("insert or") || lower.StartsWith("drop table") || lower.StartsWith("pragma ") || lower.StartsWith("alter table") || lower.StartsWith("update "))
            return true;

        if (lower.StartsWith("select ") && (lower.Contains(" from ") || lower.Contains(" count(") || lower.Contains(" where ") || lower.EndsWith("from ")))
            return true;

        if (lower.StartsWith("' and") || lower.StartsWith("', @") || lower.StartsWith("@") || lower.Contains("column_name=") || lower.Contains("table_name="))
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

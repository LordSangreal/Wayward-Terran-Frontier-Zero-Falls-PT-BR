using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Windows.Forms;
using Microsoft.Win32;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class InstallerGUI : Form {
    static InstallerGUI() {
        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
            string shortName = new AssemblyName(args.Name).Name;
            string resName = shortName + ".dll";
            using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream(resName)) {
                if (s != null) {
                    byte[] b = new byte[s.Length];
                    s.Read(b, 0, b.Length);
                    return Assembly.Load(b);
                }
            }
            return null;
        };
    }

    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    private static extern bool AttachConsole(int dwProcessId);

    [STAThread]
    public static void Main(string[] args) {
        if (args.Length > 0 && (args[0] == "--cli" || args[0] == "--install")) {
            AttachConsole(-1);
            Console.WriteLine("==========================================================");
            Console.WriteLine("   Instalador Autonomo PT-BR - Wayward Terran Frontier    ");
            Console.WriteLine("==========================================================");
            string gDir = args.Length > 1 ? args[1] : FindSteamGameDir();
            if (string.IsNullOrEmpty(gDir) || !File.Exists(Path.Combine(gDir, "WTF.exe"))) {
                Console.WriteLine("[ERRO] Pasta do jogo invalida: " + gDir);
                Environment.Exit(1);
            }
            Console.WriteLine("Pasta do jogo: " + gDir);
            var inst = new InstallerGUI(true);
            inst.DoInstall(gDir);
            Environment.Exit(0);
            return;
        }

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new InstallerGUI());
    }

    private bool isCli = false;
    private TextBox txtGamePath;
    private Button btnBrowse;
    private Label lblStatus;
    private Button btnInstall;
    private Button btnRestore;
    private ProgressBar progressBar;
    private RichTextBox txtLog;
    private Label lblHeaderTitle;
    private Label lblHeaderSub;

    public InstallerGUI() : this(false) { }

    public InstallerGUI(bool cli) {
        this.isCli = cli;
        if (!cli) {
            InitializeComponent();
            DetectSteamPath();
        }
    }

    private void InitializeComponent() {
        this.Text = "Instalador da Tradução PT-BR v1.0.1 - Zero Falls";
        this.Size = new Size(740, 600);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.BackColor = Color.FromArgb(20, 26, 35);
        this.ForeColor = Color.White;

        // Header Panel
        Panel pnlHeader = new Panel {
            Dock = DockStyle.Top,
            Height = 90,
            BackColor = Color.FromArgb(13, 17, 23)
        };
        this.Controls.Add(pnlHeader);

        lblHeaderTitle = new Label {
            Text = "WAYWARD TERRAN FRONTIER: ZERO FALLS",
            Font = new Font("Segoe UI", 13.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(0, 215, 255),
            Location = new Point(20, 16),
            AutoSize = true
        };
        pnlHeader.Controls.Add(lblHeaderTitle);

        lblHeaderSub = new Label {
            Text = "Tradução Oficial em Português do Brasil (PT-BR) • Versão 1.0.1",
            Font = new Font("Segoe UI", 9.5f),
            ForeColor = Color.FromArgb(170, 205, 230),
            Location = new Point(21, 46),
            AutoSize = true
        };
        pnlHeader.Controls.Add(lblHeaderSub);

        // Path GroupBox
        GroupBox grpPath = new GroupBox {
            Text = " Pasta de Instalação do Jogo (Steam) ",
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(200, 225, 245),
            Location = new Point(20, 105),
            Size = new Size(685, 95)
        };
        this.Controls.Add(grpPath);

        txtGamePath = new TextBox {
            Font = new Font("Segoe UI", 9f),
            Location = new Point(16, 30),
            Size = new Size(535, 24),
            BackColor = Color.FromArgb(32, 40, 52),
            ForeColor = Color.White
        };
        txtGamePath.TextChanged += (s, e) => ValidateGamePath();
        grpPath.Controls.Add(txtGamePath);

        btnBrowse = new Button {
            Text = "Procurar...",
            Font = new Font("Segoe UI", 9f),
            Location = new Point(560, 28),
            Size = new Size(110, 28),
            BackColor = Color.FromArgb(48, 60, 78),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnBrowse.Click += (s, e) => BrowsePath();
        grpPath.Controls.Add(btnBrowse);

        lblStatus = new Label {
            Text = "Detectando jogo...",
            Font = new Font("Segoe UI", 8.5f),
            Location = new Point(16, 62),
            AutoSize = true
        };
        grpPath.Controls.Add(lblStatus);

        // Action Buttons
        btnInstall = new Button {
            Text = "▶  INSTALAR TRADUÇÃO PT-BR",
            Font = new Font("Segoe UI", 11f, FontStyle.Bold),
            Location = new Point(20, 215),
            Size = new Size(420, 48),
            BackColor = Color.FromArgb(16, 148, 72),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnInstall.FlatAppearance.BorderSize = 0;
        btnInstall.Click += (s, e) => StartInstall();
        this.Controls.Add(btnInstall);

        btnRestore = new Button {
            Text = "↺  Restaurar Original (Inglês)",
            Font = new Font("Segoe UI", 9.5f),
            Location = new Point(455, 215),
            Size = new Size(250, 48),
            BackColor = Color.FromArgb(58, 68, 84),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnRestore.FlatAppearance.BorderSize = 0;
        btnRestore.Click += (s, e) => StartRestore();
        this.Controls.Add(btnRestore);

        // Progress Bar
        progressBar = new ProgressBar {
            Location = new Point(20, 275),
            Size = new Size(685, 14),
            Style = ProgressBarStyle.Continuous
        };
        this.Controls.Add(progressBar);

        // Log Console
        txtLog = new RichTextBox {
            Location = new Point(20, 300),
            Size = new Size(685, 230),
            BackColor = Color.FromArgb(10, 14, 20),
            ForeColor = Color.FromArgb(160, 220, 245),
            Font = new Font("Consolas", 8.5f),
            ReadOnly = true,
            BorderStyle = BorderStyle.None
        };
        this.Controls.Add(txtLog);

        Log("Bem-vindo ao Instalador Oficial da Tradução PT-BR!");
        Log("Versão 1.0.1 compatível com a versão Steam (unstable beta).");
        Log("----------------------------------------------------------");
    }

    private void Log(string msg) {
        if (isCli) {
            Console.WriteLine(msg);
            return;
        }
        if (txtLog == null) return;
        if (txtLog.InvokeRequired) {
            txtLog.Invoke(new Action<string>(Log), msg);
            return;
        }
        txtLog.AppendText(msg + "\r\n");
        txtLog.SelectionStart = txtLog.Text.Length;
        txtLog.ScrollToCaret();
    }

    private void SetProgress(int val) {
        if (isCli || progressBar == null) return;
        if (progressBar.InvokeRequired) {
            progressBar.Invoke(new Action<int>(SetProgress), val);
            return;
        }
        progressBar.Value = Math.Max(0, Math.Min(100, val));
    }

    private void SetButtonsEnabled(bool enabled) {
        if (isCli || btnInstall == null) return;
        if (this.InvokeRequired) {
            this.Invoke(new Action<bool>(SetButtonsEnabled), enabled);
            return;
        }
        btnInstall.Enabled = enabled;
        btnRestore.Enabled = enabled;
        btnBrowse.Enabled = enabled;
        txtGamePath.Enabled = enabled;
    }

    private void DetectSteamPath() {
        string detected = FindSteamGameDir();
        if (!string.IsNullOrEmpty(detected)) {
            txtGamePath.Text = detected;
            lblStatus.Text = "✓ Jogo detectado automaticamente na biblioteca Steam!";
            lblStatus.ForeColor = Color.FromArgb(80, 220, 120);
            Log("Pasta do jogo detectada na Steam: " + detected);
        } else {
            lblStatus.Text = "Não foi possível detectar a pasta da Steam. Clique em 'Procurar...' para selecionar.";
            lblStatus.ForeColor = Color.FromArgb(240, 180, 60);
            Log("Aviso: Pasta da Steam não encontrada automaticamente.");
        }
    }

    private void ValidateGamePath() {
        string p = txtGamePath.Text.Trim('\"', ' ');
        if (File.Exists(Path.Combine(p, "WTF.exe"))) {
            lblStatus.Text = "✓ WTF.exe encontrado na pasta selecionada!";
            lblStatus.ForeColor = Color.FromArgb(80, 220, 120);
        } else {
            lblStatus.Text = "✗ WTF.exe não encontrado nesta pasta.";
            lblStatus.ForeColor = Color.FromArgb(250, 90, 90);
        }
    }

    private void BrowsePath() {
        using (var fbd = new FolderBrowserDialog()) {
            fbd.Description = "Selecione a pasta onde o Wayward Terran Frontier: Zero Falls está instalado:";
            if (fbd.ShowDialog() == DialogResult.OK) {
                txtGamePath.Text = fbd.SelectedPath;
            }
        }
    }

    private static string FindSteamGameDir() {
        string cur = AppDomain.CurrentDomain.BaseDirectory;
        if (File.Exists(Path.Combine(cur, "WTF.exe"))) return cur;

        List<string> steamLibraries = new List<string>();

        // 1. Check Registry
        string steamPath = null;
        try {
            using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam")) {
                if (key != null) steamPath = key.GetValue("SteamPath") as string;
            }
            if (string.IsNullOrEmpty(steamPath)) {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Valve\Steam")) {
                    if (key != null) steamPath = key.GetValue("InstallPath") as string;
                }
            }
        } catch { }

        if (!string.IsNullOrEmpty(steamPath)) {
            steamLibraries.Add(steamPath);
            string vdfPath = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
            if (File.Exists(vdfPath)) {
                try {
                    string content = File.ReadAllText(vdfPath);
                    var matches = Regex.Matches(content, @"\""path\""\s+\""([^\""]+)\""");
                    foreach (Match m in matches) {
                        string p = m.Groups[1].Value.Replace(@"\\", @"\");
                        if (!steamLibraries.Contains(p)) steamLibraries.Add(p);
                    }
                } catch { }
            }
        }

        // 2. Common drives fallback
        string[] fallbacks = new string[] {
            @"C:\Program Files (x86)\Steam",
            @"C:\SteamLibrary",
            @"D:\SteamLibrary",
            @"E:\SteamLibrary",
            @"F:\SteamLibrary",
            @"G:\SteamLibrary"
        };
        foreach (var fb in fallbacks) {
            if (!steamLibraries.Contains(fb)) steamLibraries.Add(fb);
        }

        // 3. Check for WTF.exe in each library
        foreach (var lib in steamLibraries) {
            string candidate = Path.Combine(lib, "steamapps", "common", "Zero Falls");
            if (File.Exists(Path.Combine(candidate, "WTF.exe"))) {
                return Path.GetFullPath(candidate);
            }
            if (File.Exists(Path.Combine(lib, "WTF.exe"))) {
                return Path.GetFullPath(lib);
            }
        }

        return null;
    }

    private void StartInstall() {
        string gameDir = txtGamePath.Text.Trim('\"', ' ');
        if (string.IsNullOrEmpty(gameDir) || !File.Exists(Path.Combine(gameDir, "WTF.exe"))) {
            MessageBox.Show("O arquivo WTF.exe não foi encontrado na pasta especificada.\nPor favor, verifique o caminho da pasta do jogo.", "Pasta Inválida", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Check if game is running
        var procs = Process.GetProcessesByName("WTF");
        if (procs.Length > 0) {
            MessageBox.Show("O jogo Zero Falls (WTF.exe) está em execução!\nPor favor, feche o jogo antes de instalar a tradução.", "Jogo Aberto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        SetButtonsEnabled(false);
        SetProgress(5);

        ThreadPool.QueueUserWorkItem(state => {
            try {
                DoInstall(gameDir);
            } catch (Exception ex) {
                Log("\r\n[ERRO FATAL] " + ex.Message);
                Log(ex.StackTrace);
                this.Invoke(new Action(() => {
                    MessageBox.Show("Ocorreu um erro durante a instalação:\n" + ex.Message, "Erro na Instalação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
            } finally {
                SetButtonsEnabled(true);
            }
        });
    }

    private void DoInstall(string gameDir) {
        Log("\r\n==========================================================");
        Log("INICIANDO INSTALAÇÃO DA TRADUÇÃO PT-BR v1.0.1");
        Log("==========================================================");

        string gameExe = Path.Combine(gameDir, "WTF.exe");
        string backupExe = Path.Combine(gameDir, "WTF_backup.exe");
        var gameFileInfo = new FileInfo(gameExe);
        bool needsNewBackup = false;

        if (!File.Exists(backupExe)) {
            needsNewBackup = true;
        } else {
            var backupFileInfo = new FileInfo(backupExe);
            if (gameFileInfo.Length != backupFileInfo.Length) {
                if (CheckIfOriginal(gameExe)) {
                    Log("[AVISO] Nova versão limpa da Steam detectada!");
                    string oldBackup = Path.Combine(gameDir, "WTF_backup_anterior.exe");
                    try {
                        if (File.Exists(oldBackup)) File.Delete(oldBackup);
                        File.Move(backupExe, oldBackup);
                        Log("Backup da versão anterior salvo em WTF_backup_anterior.exe");
                    } catch { }
                    needsNewBackup = true;
                }
            }
        }

        if (needsNewBackup) {
            Log("[1/4] Criando backup do executável original em WTF_backup.exe...");
            File.Copy(gameExe, backupExe, true);
        } else {
            Log("[1/4] Backup original confirmado: WTF_backup.exe");
        }
        SetProgress(20);

        // Backup world_params.sqlite
        string sqlitePath = Path.Combine(gameDir, "parameterdb", "world_params.sqlite");
        string sqliteBackup = Path.Combine(gameDir, "parameterdb", "world_params_backup.sqlite");
        if (File.Exists(sqlitePath) && !File.Exists(sqliteBackup)) {
            Log("Criando backup de segurança do banco de parâmetros...");
            File.Copy(sqlitePath, sqliteBackup, true);
        }

        // Copy / Extract external files
        Log("[2/4] Instalando arquivos externos (diários, tutoriais, banco de parâmetros)...");
        string localPtBr = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pt_br");
        Dictionary<string, string> dict = null;
        HashSet<string> blacklist = null;

        if (Directory.Exists(localPtBr)) {
            Log("Copiando arquivos da pasta local pt_br...");
            CopyDirectory(localPtBr, gameDir);
            string dictPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "patcher", "translations_exe.json");
            if (!File.Exists(dictPath)) dictPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "translations_exe.json");
            dict = LoadDictionary(dictPath);
            string blPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "patcher", "shader_blacklist.json");
            if (!File.Exists(blPath)) blPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "shader_blacklist.json");
            blacklist = LoadBlacklist(blPath);
        } else {
            // Extract from embedded payload.zip
            Log("Extraindo arquivos diretamente do pacote integrado...");
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("payload.zip")) {
                if (stream == null) throw new Exception("Recurso payload.zip não encontrado no executável!");
                using (var archive = new ZipArchive(stream)) {
                    foreach (var entry in archive.Entries) {
                        if (string.IsNullOrEmpty(entry.Name)) continue; // Directory entry
                        if (entry.FullName.StartsWith("pt_br/")) {
                            string relative = entry.FullName.Substring("pt_br/".Length);
                            string dest = Path.Combine(gameDir, relative.Replace('/', Path.DirectorySeparatorChar));
                            string dir = Path.GetDirectoryName(dest);
                            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                            entry.ExtractToFile(dest, true);
                        } else if (entry.Name == "translations_exe.json") {
                            using (var sr = new StreamReader(entry.Open())) {
                                dict = LoadDictionaryFromString(sr.ReadToEnd());
                            }
                        } else if (entry.Name == "shader_blacklist.json") {
                            using (var sr = new StreamReader(entry.Open())) {
                                blacklist = LoadBlacklistFromString(sr.ReadToEnd());
                            }
                        }
                    }
                }
            }
        }

        if (dict == null || dict.Count == 0) {
            throw new Exception("Falha ao carregar dicionário de traduções translations_exe.json!");
        }
        if (blacklist == null) blacklist = new HashSet<string>();

        Log("Arquivos externos instalados com sucesso!");
        SetProgress(50);

        // Apply Mono.Cecil Patch
        Log("[3/4] Modificando executável WTF.exe via Mono.Cecil...");
        var readerParams = new ReaderParameters { ReadWrite = false };
        var asm = AssemblyDefinition.ReadAssembly(backupExe, readerParams);

        // Patch TalentsScreenV headers
        PatchTalentsScreenHeaders(asm);

        // Rarity Enum
        foreach (var type in asm.MainModule.Types) {
            if (type.FullName == "CoOpSpRpG.Rarity") {
                foreach (var f in type.Fields) {
                    if (f.Name == "Common") f.Name = "Comum";
                    else if (f.Name == "Uncommon") f.Name = "Incomum";
                    else if (f.Name == "Rare") f.Name = "Raro";
                    else if (f.Name == "Exotic") f.Name = "Exotico";
                }
            }
            // openMP unlock
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
            // Save migration: createAllTables
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

        Log("Instruções modificadas com proteção de integridade: " + replaced);
        SetProgress(80);

        string tempOut = Path.Combine(gameDir, "WTF_patched_temp.exe");
        asm.Write(tempOut);
        asm.Dispose();

        File.Copy(tempOut, gameExe, true);
        File.Delete(tempOut);

        SetProgress(100);
        Log("[4/4] Gravação concluída com sucesso!");
        Log("==========================================================");
        Log("TRADUÇÃO PT-BR INSTALADA COM ÊXITO!");
        Log("Você já pode iniciar o jogo normalmente pela Steam!");
        Log("==========================================================");

        if (!isCli) {
            this.Invoke(new Action(() => {
                MessageBox.Show("Tradução PT-BR instalada com sucesso!\nVocê já pode abrir o jogo pela sua biblioteca da Steam.", "Instalação Concluída", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
        }
    }

    private void StartRestore() {
        string gameDir = txtGamePath.Text.Trim('\"', ' ');
        if (string.IsNullOrEmpty(gameDir) || !File.Exists(Path.Combine(gameDir, "WTF.exe"))) {
            MessageBox.Show("Pasta do jogo inválida.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var res = MessageBox.Show("Deseja restaurar o jogo para o original em inglês da Steam?", "Confirmar Restauração", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (res != DialogResult.Yes) return;

        SetButtonsEnabled(false);
        try {
            string gameExe = Path.Combine(gameDir, "WTF.exe");
            string backupExe = Path.Combine(gameDir, "WTF_backup.exe");
            string sqlitePath = Path.Combine(gameDir, "parameterdb", "world_params.sqlite");
            string sqliteBackup = Path.Combine(gameDir, "parameterdb", "world_params_backup.sqlite");

            if (File.Exists(backupExe)) {
                File.Copy(backupExe, gameExe, true);
                Log("Executável restaurado a partir de WTF_backup.exe.");
            } else {
                Log("Aviso: WTF_backup.exe não encontrado. Você pode verificar integridade na Steam.");
            }

            if (File.Exists(sqliteBackup)) {
                File.Copy(sqliteBackup, sqlitePath, true);
                Log("Banco de parâmetros original restaurado com sucesso.");
            }

            SetProgress(100);
            Log("Restauração concluída!");
            MessageBox.Show("Jogo restaurado para o original com sucesso!", "Restauração Concluída", MessageBoxButtons.OK, MessageBoxIcon.Information);
        } catch (Exception ex) {
            Log("[ERRO AO RESTAURAR] " + ex.Message);
            MessageBox.Show("Erro ao restaurar: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        } finally {
            SetButtonsEnabled(true);
        }
    }

    private static void CopyDirectory(string sourceDir, string destDir) {
        foreach (string dir in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories)) {
            string relative = dir.Substring(sourceDir.Length).TrimStart('\\', '/');
            string target = Path.Combine(destDir, relative);
            if (!Directory.Exists(target)) Directory.CreateDirectory(target);
        }
        foreach (string file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories)) {
            string relative = file.Substring(sourceDir.Length).TrimStart('\\', '/');
            string target = Path.Combine(destDir, relative);
            File.Copy(file, target, true);
        }
    }

    private static int ProcessType(TypeDefinition type, Dictionary<string, string> dict, HashSet<string> blacklist) {
        int count = 0;
        if (type.Name == "TutorialTemplate" || type.Name == "TUTORIAL_BAG") return 0;
        if (type.Name == "WorldDatabase") return 0;
        if (type.Name == "AttackPlayerWithNonFactionStage") return 0;
        if (type.Name == "LoaderProc" || type.Name == "MenuBackground" || type.Name == "MenuBackgroundV2" || type.Name == "MONSTERBAG") return 0;
        if (type.BaseType != null && type.BaseType.Name == "ParticleEffect") return 0;
        if (type.Name == "TriggerEvent" || type.Name == "AGENTBAG" || type.Name == "AgentTracker" || type.Name == "GameFile") return 0;

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
            if (m.Name == "drawBumpyStuff" || m.Name == "combinePasses" || m.Name == "applyToScreen" || m.Name == "drawGraphs") continue;
            if (m.Name == "LoadContent" || m.Name == "loadContent" || m.Name == "LoadParticleEffect") continue;
            if (type.Name == "Game1" && (m.Name == ".ctor" || m.Name == "Initialize")) continue;
            if (type.Name == "LoaderProc" && m.Name == "loadSound") continue;
            if (type.Name == "ShipNavigationRev3" && (m.Name == "startSounds" || m.Name == "spawnFire")) continue;
            if (type.Name == "Ship" && m.Name == "playTravelCharge") continue;
            if (type.Name == "HackingScreen" && m.Name == "Update") continue;

            string mNameLower = m.Name.ToLowerInvariant();
            if (mNameLower.StartsWith("read") || mNameLower.StartsWith("write") ||
                mNameLower.StartsWith("save") || mNameLower.StartsWith("loadgame") ||
                m.Name == "setPhase" || m.Name == "cleanUpQuests" || m.Name == "adoptAgent") {
                continue;
            }
            if ((type.Name == "ShipDesignRev4" || type.Name == "ShipDesignRev3") && (m.Name == "InitCategories" || m.Name == "Init")) continue;
            if (type.BaseType != null && type.BaseType.Name == "TriggerEvent" && m.Name == ".ctor") continue;

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

            foreach (var i in m.Body.Instructions) {
                if (i.OpCode == OpCodes.Ldstr) {
                    string s = i.Operand as string;
                    if (s == null) continue;
                    if (IsSqlOrTechnical(s)) continue;
                    if (i.Next != null && i.Next.Operand != null) {
                        string nextOp = i.Next.Operand.ToString();
                        if (nextOp.Contains("getAgent") || nextOp.Contains("adoptAgent") || nextOp.Contains("addAgent")) continue;
                    }
                    if (blacklist.Contains(s) && !s.Contains(" ")) continue;
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

    private static bool IsSafeCharacterDataString(string s) {
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

    private static bool CheckIfOriginal(string exePath) {
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

    private static bool IsSqlOrTechnical(string s) {
        if (string.IsNullOrEmpty(s)) return true;
        string lower = s.ToLowerInvariant().Trim();
        if (lower == "name" || lower == "page" || lower == "img" || lower == "src" || lower == "body" || 
            lower == "type" || lower == "color" || lower == "font" || lower == "text" || lower == "bind")
            return true;
        if (lower == "content" || lower == @"root\cimv2" || lower.StartsWith("content/")) return true;
        if (lower == "fire" || lower == "warpspool" || lower == "klaxon_loop" || lower == "amb_reactor-loop") return true;
        if (lower == "introduction" || lower.StartsWith("waypoint_") || lower == "badguyspawn" || 
            lower == "pest" || lower == "hacking" || lower == "ship navigation" ||
            lower == "station_t1_repair" || lower == "ssc_drone_hive" || lower == "repair tutorial" ||
            lower == "farming" || lower == "feed the rats" || lower == "null quest" || lower == "null quest name" ||
            lower == "3 ssc bosses")
            return true;
        if (lower == "missile" || lower == "budd" || lower == "mick" || lower == "shady dealer" ||
            lower == "robo barkeep 880" || lower == "robo barkeep 890")
            return true;
        if (lower.StartsWith("net thread") || lower.StartsWith("server process") || lower.StartsWith("error running process") || lower.StartsWith("exception when sending"))
            return true;
        if (s.StartsWith(".") && s.Length <= 5 && !s.Contains(" ")) return true;
        if ((s.StartsWith("\\") || s.StartsWith("/")) && (s.EndsWith("\\") || s.EndsWith("/"))) return true;
        if (lower.EndsWith(".xwb") || lower.EndsWith(".xsb") || lower.EndsWith(".xnb") || lower.EndsWith(".png") || lower.EndsWith(".dll")) return true;
        if (lower.Contains("data source=") || lower.Contains("version=3;") || lower.EndsWith(".sqlite") || lower.EndsWith(".db")) return true;
        if (lower.StartsWith("create table") || lower.StartsWith("delete from") || lower.StartsWith("insert into") || lower.StartsWith("insert or") || lower.StartsWith("drop table") || lower.StartsWith("pragma ") || lower.StartsWith("alter table") || lower.StartsWith("update ")) return true;
        if (lower.StartsWith("select ") && (lower.Contains(" from ") || lower.Contains(" count(") || lower.Contains(" where ") || lower.EndsWith("from "))) return true;
        if (lower.StartsWith("' and") || lower.StartsWith("', @") || lower.StartsWith("@") || lower.Contains("column_name=") || lower.Contains("table_name=")) return true;
        if (lower.Contains("primary key") || lower.Contains("values (") || lower.Contains("values(")) return true;
        return false;
    }

    private static Dictionary<string, string> LoadDictionary(string jsonPath) {
        if (!File.Exists(jsonPath)) return new Dictionary<string, string>();
        return LoadDictionaryFromString(File.ReadAllText(jsonPath));
    }

    private static Dictionary<string, string> LoadDictionaryFromString(string jsonContent) {
        var dict = new Dictionary<string, string>();
        string[] lines = jsonContent.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
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

    private static HashSet<string> LoadBlacklist(string jsonPath) {
        if (!File.Exists(jsonPath)) return new HashSet<string>();
        return LoadBlacklistFromString(File.ReadAllText(jsonPath));
    }

    private static HashSet<string> LoadBlacklistFromString(string jsonContent) {
        var set = new HashSet<string>();
        string[] lines = jsonContent.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
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

    private static void PatchTalentsScreenHeaders(AssemblyDefinition asmGame) {
        try {
            byte[] dllBytes = Convert.FromBase64String(TalentPatchBase64);
            var asmPatch = AssemblyDefinition.ReadAssembly(new MemoryStream(dllBytes));
            var patchType = asmPatch.MainModule.GetType("TalentPatch");
            MethodDefinition patchMethod = null;
            foreach (var m in patchType.Methods) {
                if (m.Name == "DrawCategoryHeader") {
                    patchMethod = m;
                    break;
                }
            }

            var targetType = asmGame.MainModule.GetType("CoOpSpRpG.TalentsScreenV");
            if (targetType == null || patchMethod == null) return;

            var newMethod = new MethodDefinition("DrawCategoryHeader", Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.Static, asmGame.MainModule.TypeSystem.Void);
            foreach (var p in patchMethod.Parameters) {
                newMethod.Parameters.Add(new ParameterDefinition(p.Name, p.Attributes, asmGame.MainModule.ImportReference(p.ParameterType)));
            }
            foreach (var v in patchMethod.Body.Variables) {
                newMethod.Body.Variables.Add(new VariableDefinition(asmGame.MainModule.ImportReference(v.VariableType)));
            }
            newMethod.Body.InitLocals = patchMethod.Body.InitLocals;

            var il = newMethod.Body.GetILProcessor();
            for (int i = 0; i < patchMethod.Body.Instructions.Count; i++) {
                var inst = patchMethod.Body.Instructions[i];
                object op = inst.Operand;
                if (op is TypeReference) op = asmGame.MainModule.ImportReference((TypeReference)op);
                else if (op is MethodReference) op = asmGame.MainModule.ImportReference((MethodReference)op);
                else if (op is FieldReference) op = asmGame.MainModule.ImportReference((FieldReference)op);

                Instruction newInst;
                if (op == null) newInst = il.Create(inst.OpCode);
                else if (op is TypeReference) newInst = il.Create(inst.OpCode, (TypeReference)op);
                else if (op is MethodReference) newInst = il.Create(inst.OpCode, (MethodReference)op);
                else if (op is FieldReference) newInst = il.Create(inst.OpCode, (FieldReference)op);
                else if (op is string) newInst = il.Create(inst.OpCode, (string)op);
                else if (op is int) newInst = il.Create(inst.OpCode, (int)op);
                else if (op is double) newInst = il.Create(inst.OpCode, (double)op);
                else if (op is float) newInst = il.Create(inst.OpCode, (float)op);
                else if (op is byte) newInst = il.Create(inst.OpCode, (byte)op);
                else if (op is sbyte) newInst = il.Create(inst.OpCode, (sbyte)op);
                else if (op is VariableDefinition) {
                    int vIdx = ((VariableDefinition)op).Index;
                    newInst = il.Create(inst.OpCode, newMethod.Body.Variables[vIdx]);
                }
                else if (op is ParameterDefinition) {
                    int pIdx = ((ParameterDefinition)op).Index;
                    newInst = il.Create(inst.OpCode, newMethod.Parameters[pIdx]);
                }
                else if (op is Instruction) {
                    newInst = il.Create(OpCodes.Nop);
                    newInst.OpCode = inst.OpCode;
                }
                else {
                    throw new Exception("Unhandled operand: " + op);
                }
                il.Append(newInst);
            }

            for (int i = 0; i < patchMethod.Body.Instructions.Count; i++) {
                var oldInst = patchMethod.Body.Instructions[i];
                if (oldInst.Operand is Instruction) {
                    int targetIdx = patchMethod.Body.Instructions.IndexOf((Instruction)oldInst.Operand);
                    newMethod.Body.Instructions[i].Operand = newMethod.Body.Instructions[targetIdx];
                }
            }

            targetType.Methods.Add(newMethod);

            MethodDefinition drawMethod = null;
            foreach (var m in targetType.Methods) {
                if (m.Name == "Draw") {
                    drawMethod = m;
                    break;
                }
            }
            if (drawMethod == null) return;

            var drawIl = drawMethod.Body.GetILProcessor();
            for (int i = 0; i < drawMethod.Body.Instructions.Count; i++) {
                var inst = drawMethod.Body.Instructions[i];
                if (inst.OpCode == OpCodes.Callvirt && inst.Operand != null && inst.Operand.ToString().Contains("SpriteBatch::Draw(Microsoft.Xna.Framework.Graphics.Texture2D,Microsoft.Xna.Framework.Vector2,Microsoft.Xna.Framework.Color)")) {
                    var p4Inst = drawMethod.Body.Instructions[i - 4];
                    if (p4Inst.Operand != null && p4Inst.Operand.ToString().Contains("categoryNameSpots")) {
                        var prevIndexInst = drawMethod.Body.Instructions[i - 3];
                        Instruction loadIndex;
                        if (prevIndexInst.Operand is VariableDefinition) {
                            loadIndex = drawIl.Create(prevIndexInst.OpCode, (VariableDefinition)prevIndexInst.Operand);
                        } else {
                            loadIndex = drawIl.Create(prevIndexInst.OpCode);
                        }
                        drawIl.InsertBefore(inst, loadIndex);
                        inst.OpCode = OpCodes.Call;
                        inst.Operand = newMethod;
                        break;
                    }
                }
            }
        } catch { }
    }

    private const string TalentPatchBase64 = "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AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==";
}

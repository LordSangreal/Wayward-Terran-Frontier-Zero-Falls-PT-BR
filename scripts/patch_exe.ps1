param(
    [string]$BaseDir = (Split-Path -Parent $PSScriptRoot)
)

[AppDomain]::CurrentDomain.add_AssemblyResolve({
    param($s, $e)
    $dll = ($e.Name.Split(',')[0]) + ".dll"
    $p = Join-Path $BaseDir $dll
    if (Test-Path $p) { return [System.Reflection.Assembly]::LoadFrom($p) }
    $p2 = Join-Path "D:\SteamLibrary\steamapps\common\Zero Falls" $dll
    if (Test-Path $p2) { return [System.Reflection.Assembly]::LoadFrom($p2) }
    return $null
})

$cecilPath = Join-Path $BaseDir "Mono.Cecil.dll"
[System.Reflection.Assembly]::LoadFrom($cecilPath) | Out-Null

$jsonPath = Join-Path $BaseDir "translations_exe.json"
$backupExe = Join-Path $BaseDir "backup_game_original\WTF.exe"
$gameExe = "D:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe"

if (-not (Test-Path $backupExe)) {
    Write-Host "[ERRO] Backup original nao encontrado em: $backupExe" -ForegroundColor Red
    exit 1
}

# Ler mapeamento JSON
$rawJson = Get-Content $jsonPath -Raw -Encoding UTF8
$dict = ConvertFrom-Json $rawJson

Write-Host "=== APLICANDO TRADUCAO PT-BR NO EXECUTAVEL (WTF.exe) ===" -ForegroundColor Cyan
Write-Host "Fonte: Backup original ($backupExe)"
Write-Host "Destino: Jogo ($gameExe)"
Write-Host "Termos no dicionario: $($dict.psobject.properties.Count)"

$asm = [Mono.Cecil.AssemblyDefinition]::ReadAssembly($backupExe)

$replacedCount = 0
$dictEntries = @{}
foreach ($prop in $dict.psobject.properties) {
    $dictEntries[$prop.Name] = $prop.Value
}

function ProcessTypeInstructions($t) {
    foreach ($m in $t.Methods) {
        if ($m.HasBody) {
            foreach ($i in $m.Body.Instructions) {
                if ($i.OpCode.Name -eq "ldstr") {
                    $val = [string]$i.Operand
                    if ($val -ne $null -and $script:dictEntries.ContainsKey($val)) {
                        $newVal = $script:dictEntries[$val]
                        if ($val -ne $newVal) {
                            $i.Operand = $newVal
                            $script:replacedCount++
                        }
                    }
                }
            }
        }
    }
    foreach ($nested in $t.NestedTypes) {
        ProcessTypeInstructions $nested
    }
}

foreach ($type in $asm.MainModule.Types) {
    ProcessTypeInstructions $type
}

Write-Host "Total de instrucoes modificadas: $replacedCount" -ForegroundColor Green

# Salvar o novo executavel
$tempOut = Join-Path $BaseDir "WTF_patched_temp.exe"
$asm.Write($tempOut)
Copy-Item $tempOut $gameExe -Force
Remove-Item $tempOut -Force

Write-Host "Executavel gravado com sucesso em: $gameExe" -ForegroundColor Green
Write-Host "Pronto para jogar!" -ForegroundColor Yellow

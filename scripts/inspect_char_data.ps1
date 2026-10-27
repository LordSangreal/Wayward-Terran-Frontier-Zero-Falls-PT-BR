Add-Type -Path "d:\Tradução WTF Zero falls\Mono.Cecil.dll"
$asm = [Mono.Cecil.AssemblyDefinition]::ReadAssembly("d:\Tradução WTF Zero falls\backup_game_original\WTF.exe")
$charData = $null
foreach ($t in $asm.MainModule.Types) {
    if ($t.Name -eq "CHARACTER_DATA") {
        $charData = $t
        break
    }
}

if ($charData -ne $null) {
    Write-Output "Found CHARACTER_DATA"
    foreach ($m in $charData.Methods) {
        $strings = @()
        if ($m.HasBody) {
            foreach ($instr in $m.Body.Instructions) {
                if ($instr.OpCode.Name -eq "ldstr") {
                    $strings += $instr.Operand
                }
            }
        }
        if ($strings.Count -gt 0) {
            Write-Output "Method: $($m.Name)"
            foreach ($s in $strings) {
                Write-Output "   ldstr: $s"
            }
        }
    }
} else {
    Write-Output "CHARACTER_DATA not found"
}

$baseDir = Split-Path -Parent $PSScriptRoot
$zipPath = Join-Path $baseDir "Zero_Falls_PTBR_v1.0.0.zip"
if (Test-Path $zipPath) {
    Remove-Item $zipPath -Force
}

$tempDir = Join-Path $env:TEMP "WTF_Release_Package"
if (Test-Path $tempDir) {
    Remove-Item $tempDir -Recurse -Force
}
New-Item -ItemType Directory -Path $tempDir | Out-Null

Copy-Item (Join-Path $baseDir "Instalar_Traducao.bat") $tempDir
Copy-Item (Join-Path $baseDir "Restaurar_Original.bat") $tempDir
Copy-Item (Join-Path $baseDir "README.md") $tempDir
Copy-Item (Join-Path $baseDir "pt_br") $tempDir -Recurse
Copy-Item (Join-Path $baseDir "patcher") $tempDir -Recurse

Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::CreateFromDirectory($tempDir, $zipPath)

Remove-Item $tempDir -Recurse -Force
Write-Output "Successfully created $zipPath with size $([math]::Round((Get-Item $zipPath).Length / 1MB, 2)) MB"

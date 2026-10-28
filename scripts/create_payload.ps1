$baseDir = Split-Path -Parent $PSScriptRoot
$tempDir = Join-Path $env:TEMP 'WTF_Payload_Temp'
if (Test-Path $tempDir) { Remove-Item $tempDir -Recurse -Force }
New-Item -ItemType Directory -Path $tempDir | Out-Null
Copy-Item (Join-Path $baseDir 'pt_br') $tempDir -Recurse
Copy-Item (Join-Path $baseDir 'patcher\translations_exe.json') $tempDir
Copy-Item (Join-Path $baseDir 'patcher\shader_blacklist.json') $tempDir
$zipPath = Join-Path $baseDir 'payload.zip'
if (Test-Path $zipPath) { Remove-Item $zipPath -Force }
Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::CreateFromDirectory($tempDir, $zipPath)
Remove-Item $tempDir -Recurse -Force
Write-Output ("Created payload.zip with size: " + [math]::Round((Get-Item $zipPath).Length / 1MB, 2) + " MB")

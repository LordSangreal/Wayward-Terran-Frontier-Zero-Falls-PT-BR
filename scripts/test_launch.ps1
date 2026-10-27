$p = Start-Process -FilePath "D:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe" -WorkingDirectory "D:\SteamLibrary\steamapps\common\Zero Falls" -PassThru
Start-Sleep -Seconds 12
if ($p.HasExited) {
    Write-Output "Process EXITED with code: $($p.ExitCode)"
} else {
    Write-Output "SUCCESS: Process STILL RUNNING! PID: $($p.Id)"
    Stop-Process -Id $p.Id -Force
}

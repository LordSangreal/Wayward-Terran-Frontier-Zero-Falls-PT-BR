$p = Start-Process -FilePath "D:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe" -WorkingDirectory "D:\SteamLibrary\steamapps\common\Zero Falls" -PassThru
Start-Sleep -Seconds 18
$alive = (Get-Process -Id $p.Id -ErrorAction SilentlyContinue)
if ($alive) {
    Write-Output "Game is running smoothly! WindowTitle: '$($alive.MainWindowTitle)', WorkingSet: $([math]::Round($alive.WorkingSet64 / 1MB, 2)) MB"
    Stop-Process -Id $p.Id -Force
    Write-Output "Test ended successfully."
} else {
    Write-Output "Game exited prematurely with code $($p.ExitCode)"
}

$steamPath = (Get-ItemProperty -Path 'HKCU:\Software\Valve\Steam' -Name SteamPath -ErrorAction SilentlyContinue).SteamPath
Write-Output ("SteamPath from HKCU: " + $steamPath)
if (-not $steamPath) {
    $steamPath = (Get-ItemProperty -Path 'HKLM:\SOFTWARE\WOW6432Node\Valve\Steam' -Name InstallPath -ErrorAction SilentlyContinue).InstallPath
    Write-Output ("SteamPath from HKLM: " + $steamPath)
}
if ($steamPath) {
    $vdfPath = Join-Path $steamPath 'steamapps\libraryfolders.vdf'
    Write-Output ("VDF Path: " + $vdfPath)
    if (Test-Path $vdfPath) {
        $content = Get-Content $vdfPath -Raw
        $regex = [regex]'\"path\"\s+\"([^\"]+)\"'
        $matches = $regex.Matches($content)
        foreach ($m in $matches) {
            $libPath = $m.Groups[1].Value.Replace('\\', '\')
            $gameExe = Join-Path $libPath 'steamapps\common\Zero Falls\WTF.exe'
            Write-Output ("Checking: " + $gameExe + " -> " + (Test-Path $gameExe))
        }
    }
}

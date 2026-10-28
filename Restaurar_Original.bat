@echo off
chcp 65001 >nul
title Restaurar Jogo Original - Zero Falls
cls
echo ==========================================================
echo        Restaurador do Jogo Original (Inglês)
echo ==========================================================
echo.

set "SCRIPT_DIR=%~dp0"
set "GAME_DIR="

if exist "%SCRIPT_DIR%WTF.exe" set "GAME_DIR=%SCRIPT_DIR%"
if not defined GAME_DIR if exist "D:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe" set "GAME_DIR=D:\SteamLibrary\steamapps\common\Zero Falls"
if not defined GAME_DIR if exist "C:\Program Files (x86)\Steam\steamapps\common\Zero Falls\WTF.exe" set "GAME_DIR=C:\Program Files (x86)\Steam\steamapps\common\Zero Falls"
if not defined GAME_DIR if exist "C:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe" set "GAME_DIR=C:\SteamLibrary\steamapps\common\Zero Falls"
if not defined GAME_DIR if exist "E:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe" set "GAME_DIR=E:\SteamLibrary\steamapps\common\Zero Falls"

if not defined GAME_DIR (
    set /p "GAME_DIR=Cole aqui o caminho da pasta do jogo: "
)

set "GAME_DIR=%GAME_DIR:"=%"

if exist "%GAME_DIR%\WTF_backup.exe" (
    echo Restaurando executável original a partir de WTF_backup.exe...
    copy /Y "%GAME_DIR%\WTF_backup.exe" "%GAME_DIR%\WTF.exe" >nul
    echo Executavel restaurado com sucesso!
) else (
    echo [AVISO] Nenhum backup WTF_backup.exe foi encontrado.
    echo Para restaurar o jogo original pelo Steam:
    echo 1. Clique com botão direito em Zero Falls na Steam.
    echo 2. Vá em Propriedades -> Arquivos Instalados -> Verificar integridade dos arquivos.
)

if exist "%GAME_DIR%\parameterdb\world_params_backup.sqlite" (
    echo Restaurando banco de parametros original...
    copy /Y "%GAME_DIR%\parameterdb\world_params_backup.sqlite" "%GAME_DIR%\parameterdb\world_params.sqlite" >nul
    echo Banco de parametros restaurado com sucesso!
)

echo.
echo Operação concluída.
pause

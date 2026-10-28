@echo off
chcp 65001 >nul
title Instalador da Tradução PT-BR v1.0.1 - Zero Falls
cls
echo ==========================================================
echo    Instalador da Tradução PT-BR v1.0.1 - Zero Falls
echo ==========================================================
echo.

set "SCRIPT_DIR=%~dp0"
set "GAME_DIR="

:: 1. Tenta detectar caminhos padroes da Steam
if exist "%SCRIPT_DIR%WTF.exe" set "GAME_DIR=%SCRIPT_DIR%"
if not defined GAME_DIR if exist "D:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe" set "GAME_DIR=D:\SteamLibrary\steamapps\common\Zero Falls"
if not defined GAME_DIR if exist "C:\Program Files (x86)\Steam\steamapps\common\Zero Falls\WTF.exe" set "GAME_DIR=C:\Program Files (x86)\Steam\steamapps\common\Zero Falls"
if not defined GAME_DIR if exist "C:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe" set "GAME_DIR=C:\SteamLibrary\steamapps\common\Zero Falls"
if not defined GAME_DIR if exist "E:\SteamLibrary\steamapps\common\Zero Falls\WTF.exe" set "GAME_DIR=E:\SteamLibrary\steamapps\common\Zero Falls"

:: 2. Se nao encontrou, solicita ao usuario
if not defined GAME_DIR (
    echo Não foi possível detectar a pasta do jogo automaticamente.
    echo.
    echo Por favor, abra a Steam, clique com o botão direito em "Zero Falls",
    echo vá em "Gerenciar" -> "Navegar pelos arquivos locais" e copie o caminho.
    echo.
    set /p "GAME_DIR=Cole aqui o caminho da pasta do jogo: "
)

:: Remove aspas
set "GAME_DIR=%GAME_DIR:"=%"

if not exist "%GAME_DIR%\WTF.exe" (
    echo.
    echo [ERRO] O arquivo WTF.exe não foi encontrado em: "%GAME_DIR%"
    echo Certifique-se de que selecionou a pasta correta do jogo.
    echo.
    pause
    exit /b 1
)

echo.
echo [1/3] Pasta do jogo confirmada: "%GAME_DIR%"
echo.
echo [2/3] Copiando arquivos externos traduzidos (Tutoriais, Diários, Lojas, Banco de Parâmetros)...
if exist "%GAME_DIR%\parameterdb\world_params.sqlite" if not exist "%GAME_DIR%\parameterdb\world_params_backup.sqlite" (
    copy /Y "%GAME_DIR%\parameterdb\world_params.sqlite" "%GAME_DIR%\parameterdb\world_params_backup.sqlite" >nul
)
xcopy "%SCRIPT_DIR%pt_br\*" "%GAME_DIR%\" /E /Y /I >nul
if errorlevel 1 (
    echo [ERRO] Falha ao copiar arquivos de pt_br para a pasta do jogo.
    pause
    exit /b 1
)
echo Arquivos externos copiados com sucesso!
echo.

echo [3/3] Aplicando patch de tradução no executável WTF.exe...
"%SCRIPT_DIR%patcher\Patcher.exe" "%GAME_DIR%"
if errorlevel 1 (
    echo.
    echo [ERRO] Ocorreu uma falha durante o patch do executável.
    pause
    exit /b 1
)

echo.
echo ==========================================================
echo        TRADUÇÃO INSTALADA COM SUCESSO!
echo ==========================================================
echo.
echo Você já pode iniciar o jogo normalmente pela sua Steam!
echo.
pause

# -*- coding: utf-8 -*-
import os
import shutil
import subprocess

BASE_DIR = r"d:\Tradução WTF Zero falls"
PT_BR = os.path.join(BASE_DIR, "pt_br")
BACKUP = os.path.join(BASE_DIR, "backup_game_original")
GAME_DIR = r"D:\SteamLibrary\steamapps\common\Zero Falls"
PATCHER_EXE = os.path.join(BASE_DIR, "scripts", "Patcher.exe")

def apply():
    print("=== APLICANDO TRADUCAO COMPLETA PT-BR NO JOGO ===")
    print(f"Destino: {GAME_DIR}\n")

    if not os.path.exists(GAME_DIR):
        print(f"[ERRO CRITICO] Diretorio do jogo nao encontrado: {GAME_DIR}")
        return False

    os.makedirs(BACKUP, exist_ok=True)
    applied = 0

    # 1. Aplicar arquivos externos
    for root, dirs, files in os.walk(PT_BR):
        for file in files:
            rel = os.path.relpath(os.path.join(root, file), PT_BR)
            pt_file = os.path.join(PT_BR, rel)
            target_file = os.path.join(GAME_DIR, rel)
            backup_file = os.path.join(BACKUP, rel)

            if os.path.exists(target_file) and not os.path.exists(backup_file):
                os.makedirs(os.path.dirname(backup_file), exist_ok=True)
                shutil.copy2(target_file, backup_file)

            os.makedirs(os.path.dirname(target_file), exist_ok=True)
            shutil.copy2(pt_file, target_file)
            print(f"  [ARQUIVO] {rel}")
            applied += 1

    # 2. Aplicar correcao no executavel WTF.exe
    if os.path.exists(PATCHER_EXE):
        print("\nExecutando patcher no binario WTF.exe...")
        res = subprocess.run([PATCHER_EXE], capture_output=True, text=True)
        print(res.stdout)
        if res.returncode == 0:
            print("  [OK] WTF.exe atualizado com sucesso!")
        else:
            print(f"  [ERRO] Falha no Patcher.exe: {res.stderr}")

    print(f"\nConcluido! {applied} arquivos externos e o executavel WTF.exe foram traduzidos.")
    return True

if __name__ == "__main__":
    apply()

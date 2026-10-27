# -*- coding: utf-8 -*-
import os
import shutil

BASE_DIR = r"d:\Tradução WTF Zero falls"
ORIGINAL = os.path.join(BASE_DIR, "original")
BACKUP = os.path.join(BASE_DIR, "backup_game_original")
GAME_DIR = r"D:\SteamLibrary\steamapps\common\Zero Falls"

def restore():
    print("=== RESTAURANDO ARQUIVOS ORIGINAIS NO JOGO ===")
    print(f"Destino: {GAME_DIR}\n")

    if not os.path.exists(GAME_DIR):
        print(f"[ERRO CRITICO] Diretorio do jogo nao encontrado: {GAME_DIR}")
        return False

    restored = 0
    # 1. Restaurar arquivos externos
    for root, dirs, files in os.walk(ORIGINAL):
        for file in files:
            rel = os.path.relpath(os.path.join(root, file), ORIGINAL)
            orig_file = os.path.join(ORIGINAL, rel)
            target_file = os.path.join(GAME_DIR, rel)

            if os.path.exists(orig_file):
                os.makedirs(os.path.dirname(target_file), exist_ok=True)
                shutil.copy2(orig_file, target_file)
                print(f"  [RESTAURADO] {rel}")
                restored += 1

    # 2. Restaurar executavel WTF.exe original
    backup_exe = os.path.join(BACKUP, "WTF.exe")
    game_exe = os.path.join(GAME_DIR, "WTF.exe")
    if os.path.exists(backup_exe):
        shutil.copy2(backup_exe, game_exe)
        print("  [RESTAURADO] WTF.exe original")
        restored += 1

    print(f"\nConcluido! {restored} itens restaurados para o original em ingles.")
    return True

if __name__ == "__main__":
    restore()

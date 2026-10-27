# -*- coding: utf-8 -*-
import os
import xml.etree.ElementTree as ET

BASE_DIR = r"d:\Tradução WTF Zero falls"
PT_BR = os.path.join(BASE_DIR, "pt_br")
ORIG = os.path.join(BASE_DIR, "original")

def validate():
    print("=== VALIDANDO ARQUIVOS TRADUZIDOS (FASE 1) ===")
    errors = 0
    warnings = 0
    checked = 0

    for root, dirs, files in os.walk(PT_BR):
        for file in files:
            rel = os.path.relpath(os.path.join(root, file), PT_BR)
            pt_path = os.path.join(PT_BR, rel)
            orig_path = os.path.join(ORIG, rel)

            if not os.path.exists(orig_path):
                print(f"[AVISO] Arquivo {rel} não existe no original!")
                warnings += 1
                continue

            checked += 1

            # Validação XML
            if file.lower().endswith(".xml"):
                try:
                    tree = ET.parse(pt_path)
                    root_elem = tree.getroot()
                    if root_elem.tag != "Tutorial":
                        print(f"[ERRO] {rel}: Raiz inválida: {root_elem.tag}")
                        errors += 1
                    else:
                        print(f"[OK XML] {rel} (Bem-formado, {len(root_elem.findall('Page'))} páginas)")
                except Exception as ex:
                    print(f"[ERRO XML] {rel}: Falha de sintaxe: {ex}")
                    errors += 1

            # Validação TXT e TDB
            elif file.lower().endswith((".txt", ".tdb")):
                with open(pt_path, "r", encoding="utf-8") as f:
                    content = f.read()
                if len(content.strip()) == 0:
                    print(f"[ERRO] {rel} está vazio!")
                    errors += 1
                else:
                    lines = content.splitlines()
                    print(f"[OK TEXTO] {rel} ({len(lines)} linhas, {len(content)} caracteres)")

    print(f"\nResultado: {checked} arquivos verificados. Erros: {errors}, Avisos: {warnings}")
    return errors == 0

if __name__ == "__main__":
    validate()

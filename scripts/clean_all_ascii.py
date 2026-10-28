# -*- coding: utf-8 -*-
import json
import os
import re
import unicodedata
import sqlite3

BASE_DIR = r"d:\Tradução WTF Zero falls"
TRANS_JSON = os.path.join(BASE_DIR, "translations_exe.json")
PATCHER_JSON = os.path.join(BASE_DIR, r"patcher\translations_exe.json")

CHAR_MAP = {
    '\u200b': '',      # zero-width space
    '\u200e': '',      # LTR mark
    '\u200f': '',      # RTL mark
    '\u2018': "'",     # left single quote
    '\u2019': "'",     # right single quote
    '\u201a': "'",     # single low-9 quote
    '\u201b': "'",     # single high-reversed-9 quote
    '\u201c': '"',     # left double quote
    '\u201d': '"',     # right double quote
    '\u201e': '"',     # double low-9 quote
    '\u2013': '-',     # en dash
    '\u2014': '-',     # em dash
    '\u2026': '...',   # ellipsis
    '\u00a0': ' ',     # non-breaking space
    '\u00ab': '"',     # left-pointing double angle
    '\u00bb': '"',     # right-pointing double angle
}

def strict_ascii(text):
    if not text or not isinstance(text, str):
        return text
    
    # Substituir caracteres tipograficos comuns
    for orig, rep in CHAR_MAP.items():
        text = text.replace(orig, rep)
        
    # Normalizar diacriticos
    nfkd = unicodedata.normalize('NFKD', text)
    res = ''.join([c for c in nfkd if not unicodedata.combining(c)])
    
    # Qualquer outro caracter acima de 127 vira ASCII compativel ou e descartado
    final_chars = []
    for ch in res:
        o = ord(ch)
        if o <= 127:
            final_chars.append(ch)
        else:
            # tenta encode ascii
            try:
                final_chars.append(ch.encode('ascii', 'ignore').decode('ascii'))
            except:
                pass
    return ''.join(final_chars)

def sanitize_json(file_path):
    print(f"Sanitizando {file_path}...")
    with open(file_path, 'r', encoding='utf-8-sig') as f:
        d = json.load(f)
    
    cleaned = 0
    new_d = {}
    for k, v in d.items():
        new_v = strict_ascii(v)
        if new_v != v:
            cleaned += 1
        new_d[k] = new_v
        
    print(f"  {cleaned} strings ajustadas para estrito ASCII.")
    with open(file_path, 'w', encoding='utf-8-sig') as f:
        json.dump(new_d, f, indent=2, ensure_ascii=False)

def sanitize_db(db_path):
    if not os.path.exists(db_path):
        return
    print(f"Sanitizando DB {db_path}...")
    conn = sqlite3.connect(db_path)
    c = conn.cursor()
    
    # crafts
    c.execute("SELECT rowid, name, description, profession FROM crafts")
    rows = c.fetchall()
    for rowid, name, desc, prof in rows:
        c.execute("UPDATE crafts SET name = ?, description = ?, profession = ? WHERE rowid = ?",
                  (strict_ascii(name), strict_ascii(desc), strict_ascii(prof), rowid))
        
    # quests
    c.execute("SELECT rowid, name, description FROM quests")
    rows = c.fetchall()
    for rowid, name, desc in rows:
        c.execute("UPDATE quests SET name = ?, description = ? WHERE rowid = ?",
                  (strict_ascii(name), strict_ascii(desc), rowid))
        
    conn.commit()
    conn.close()
    print("  DB sanitizado!")

def main():
    sanitize_json(TRANS_JSON)
    sanitize_json(PATCHER_JSON)
    sanitize_db(r"D:\SteamLibrary\steamapps\common\Zero Falls\parameterdb\world_params.sqlite")
    sanitize_db(os.path.join(BASE_DIR, r"pt_br\parameterdb\world_params.sqlite"))
    print("\nSanitizacao completa concluida!")

if __name__ == '__main__':
    main()

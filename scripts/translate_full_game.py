# -*- coding: utf-8 -*-
import json
import os
import re
import time
import urllib.request
import urllib.parse
import unicodedata
import concurrent.futures

BASE_DIR = r"d:\Tradução WTF Zero falls"
TRANS_JSON = os.path.join(BASE_DIR, "translations_exe.json")
FILTERED_JSON = os.path.join(BASE_DIR, r"scripts\filtered_to_translate.json")

def strip_accents(text):
    if not text:
        return text
    nfkd = unicodedata.normalize('NFKD', text)
    return ''.join([c for c in nfkd if not unicodedata.combining(c)])

def clean_term(pt_text):
    t = pt_text
    t = t.replace("travel drive", "motor de viagem")
    t = t.replace("Travel Drive", "Motor de viagem")
    t = t.replace("cloning vat", "tanque de clonagem")
    t = t.replace("cloning vats", "tanques de clonagem")
    t = t.replace("matter furnace", "fornalha de materia")
    t = t.replace("airlock", "eclusa")
    t = t.replace("airlocks", "eclusas")
    t = t.replace("shipyard", "estaleiro")
    t = t.replace("hangar", "hangar")
    t = t.replace("bridge", "ponte de comando")
    return strip_accents(t)

def translate_single_text(text):
    lead_len = len(text) - len(text.lstrip(' '))
    trail_len = len(text) - len(text.rstrip(' '))
    core = text.strip(' ')
    if not core:
        return text

    # Try up to 3 times
    for attempt in range(3):
        try:
            url = 'https://translate.googleapis.com/translate_a/single?client=gtx&sl=en&tl=pt&dt=t&q=' + urllib.parse.quote(core)
            req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64)'})
            with urllib.request.urlopen(req, timeout=12) as resp:
                data = json.loads(resp.read().decode('utf-8'))
                raw_pt = ''.join([part[0] for part in data[0] if part[0]])
                cleaned = clean_term(raw_pt)
                return (' ' * lead_len) + cleaned + (' ' * trail_len)
        except Exception:
            time.sleep(0.5 * (attempt + 1))

    # fallback
    return strip_accents(text)

def run():
    print("Iniciando traducao massiva do restante do jogo...")

    with open(TRANS_JSON, "r", encoding="utf-8-sig") as f:
        translations = json.load(f)

    with open(FILTERED_JSON, "r", encoding="utf-8") as f:
        candidates = json.load(f)

    # Get unique strings to translate
    pending = []
    seen = set()
    for item in candidates:
        s = item['text']
        if s not in translations and s not in seen:
            seen.add(s)
            pending.append(s)

    total_pending = len(pending)
    print(f"Total de strings unicas a traduzir: {total_pending}")
    if total_pending == 0:
        print("Todas as strings ja foram traduzidas!")
        return

    # Process in batches of 200 with ThreadPoolExecutor
    batch_size = 200
    completed = 0
    start_time = time.time()

    for i in range(0, total_pending, batch_size):
        batch = pending[i:i + batch_size]
        batch_start = time.time()

        with concurrent.futures.ThreadPoolExecutor(max_workers=12) as executor:
            results = list(executor.map(translate_single_text, batch))

        for original, translated in zip(batch, results):
            translations[original] = translated

        completed += len(batch)
        batch_elapsed = time.time() - batch_start
        total_elapsed = time.time() - start_time
        speed = completed / total_elapsed if total_elapsed > 0 else 0
        rem_sec = (total_pending - completed) / speed if speed > 0 else 0

        # Save checkpoint to disk
        with open(TRANS_JSON, "w", encoding="utf-8") as f:
            json.dump(translations, f, indent=2, ensure_ascii=False)

        print(f"[{completed}/{total_pending}] ({completed*100.0/total_pending:.1f}%) - Velocidade: {speed:.1f} str/s - Tempo restante est.: {rem_sec/60.0:.1f} min")

    print("\nTraducao de todos os textos concluida com sucesso!")
    print(f"Total acumulado no dicionario: {len(translations)} termos.")

if __name__ == "__main__":
    run()

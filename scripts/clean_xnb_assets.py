import json
import os

game_root = r"D:\SteamLibrary\steamapps\common\Zero Falls"
content_dir = os.path.join(game_root, "Content")

xnb_assets = set()
for root, dirs, files in os.walk(content_dir):
    for f in files:
        if f.endswith(".xnb"):
            base = os.path.splitext(f)[0]
            xnb_assets.add(base)
            # also rel path without .xnb
            rel = os.path.relpath(os.path.join(root, base), content_dir).replace("\\", "/")
            xnb_assets.add(rel)

print(f"Total de assets .xnb encontrados na pasta Content: {len(xnb_assets)}")

with open(r"d:\Tradução WTF Zero falls\translations_exe.json", "r", encoding="utf-8") as f:
    d = json.load(f)

removed_assets = []
for k in list(d.keys()):
    k_strip = k.strip()
    if k_strip in xnb_assets:
        removed_assets.append(k)
        del d[k]

print(f"Total de nomes de assets .xnb removidos do dicionario: {len(removed_assets)}")
for a in removed_assets:
    print(f"  REMOVED ASSET: {repr(a)}")

with open(r"d:\Tradução WTF Zero falls\translations_exe.json", "w", encoding="utf-8") as f:
    json.dump(d, f, indent=2, ensure_ascii=False)

print(f"Dicionario atualizado: {len(d)} termos.")

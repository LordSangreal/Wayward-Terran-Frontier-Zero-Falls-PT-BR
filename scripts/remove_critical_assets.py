import json
import os

with open(r'd:\Tradução WTF Zero falls\translations_exe.json', 'r', encoding='utf-8') as f:
    d = json.load(f)

# List of game directories on disk in D:\SteamLibrary\steamapps\common\Zero Falls
game_dirs = [
    'Content', 'Data', 'Tutorials', 'grey sales', 'orange sales', 
    'Cultivo', 'Mineracao', 'Geral', 'Fonts', 'Textures', 'Shaders', 
    'Effects', 'Audio', 'Music', 'Sounds', 'profiles'
]

critical_removals = []
for k in list(d.keys()):
    k_strip = k.strip()
    if k_strip in game_dirs:
        critical_removals.append(k)
        del d[k]
        continue

    # Also check if it's an asset name like "BloomExtract", "SplashScreen", etc.
    if k_strip in ['BloomExtract', 'BloomCombine', 'GaussianBlur', 'LinearGauss', 'DepthMask', 'Fxaa', 'LightShafts', 'InteriorCombineHDR', 'BloomCombineHDR']:
        critical_removals.append(k)
        del d[k]
        continue

    # Check font asset names
    if any(k_strip.startswith(font) for font in ['Forgotten', 'LCDFont', 'BigFont', 'MedFont', 'Font1']):
        critical_removals.append(k)
        del d[k]
        continue

print(f"Chaves criticas de pastas/assets removidas: {len(critical_removals)}")
for c in critical_removals:
    print(f"  REMOVED: {repr(c)}")

with open(r'd:\Tradução WTF Zero falls\translations_exe.json', 'w', encoding='utf-8') as f:
    json.dump(d, f, indent=2, ensure_ascii=False)

print(f"Total apos remocao: {len(d)}")

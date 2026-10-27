import json

with open(r'd:\Tradução WTF Zero falls\scripts\all_game_strings.json', 'r', encoding='utf-8-sig') as f:
    candidates = json.load(f)
with open(r'd:\Tradução WTF Zero falls\translations_exe.json', 'r', encoding='utf-8') as f:
    d = json.load(f)
with open(r'd:\Tradução WTF Zero falls\scripts\shader_blacklist.json', 'r', encoding='utf-8-sig') as f:
    shader_blacklist = set(json.load(f))

untranslated = [c for c in candidates if c['text'] not in d]
single_words = [c for c in untranslated if ' ' not in c['text'].strip() and c['text'].strip() not in shader_blacklist]

print(f"Total single words: {len(single_words)}")
for c in single_words:
    txt = c['text']
    loc = c['location']
    # skip sprites or IDs
    if not (txt.endswith('L') or txt.endswith('T') or txt.endswith('S') or txt.endswith('.png')):
        print(f"{loc:32}: {repr(txt)}")

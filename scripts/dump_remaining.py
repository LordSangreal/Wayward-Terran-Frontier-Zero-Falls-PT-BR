import json

with open(r'd:\Tradução WTF Zero falls\scripts\all_game_strings.json', 'r', encoding='utf-8-sig') as f:
    candidates = json.load(f)
with open(r'd:\Tradução WTF Zero falls\translations_exe.json', 'r', encoding='utf-8') as f:
    d = json.load(f)
with open(r'd:\Tradução WTF Zero falls\scripts\shader_blacklist.json', 'r', encoding='utf-8-sig') as f:
    shader_blacklist = set(json.load(f))

untranslated = [c for c in candidates if c['text'] not in d]
multi_words = [c for c in untranslated if ' ' in c['text'].strip() and c['text'].strip() not in shader_blacklist]

print(f"Total multi_words restantes: {len(multi_words)}")
with open(r'd:\Tradução WTF Zero falls\scripts\remaining_to_translate.json', 'w', encoding='utf-8') as f:
    json.dump(multi_words, f, indent=2, ensure_ascii=False)

for c in multi_words:
    print(f"{c['location']:32}: {repr(c['text'])}")

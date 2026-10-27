import json

with open(r'd:\Tradução WTF Zero falls\scripts\all_game_strings.json', 'r', encoding='utf-8-sig') as f:
    candidates = json.load(f)

with open(r'd:\Tradução WTF Zero falls\translations_exe.json', 'r', encoding='utf-8') as f:
    d = json.load(f)

with open(r'd:\Tradução WTF Zero falls\scripts\shader_blacklist.json', 'r', encoding='utf-8-sig') as f:
    shader_blacklist = set(json.load(f))

untranslated = [c for c in candidates if c['text'] not in d]

print(f"Total de candidatos a textos de usuario: {len(candidates)}")
print(f"Ja traduzidos no dicionario: {len(candidates) - len(untranslated)}")
print(f"Nao traduzidos no dicionario: {len(untranslated)}")

# Breakdown of untranslated
in_shader_blacklist = [c for c in untranslated if c['text'].strip() in shader_blacklist]
single_words = [c for c in untranslated if ' ' not in c['text'].strip() and c not in in_shader_blacklist]
multi_words = [c for c in untranslated if ' ' in c['text'].strip() and c not in in_shader_blacklist]

print(f"  - Na blacklist de shaders (proibidos de alterar para nao crashar): {len(in_shader_blacklist)}")
print(f"  - Palavras unicas / codigos / nomes proprios: {len(single_words)}")
print(f"  - Frases / multi-palavras restantes: {len(multi_words)}")

print("\n--- Exemplos de frases restantes (se houver): ---")
for c in multi_words[:25]:
    print(f"{c['location']:30}: {repr(c['text'])}")

print("\n--- Exemplos de palavras unicas restantes: ---")
for c in single_words[:25]:
    print(f"{c['location']:30}: {repr(c['text'])}")

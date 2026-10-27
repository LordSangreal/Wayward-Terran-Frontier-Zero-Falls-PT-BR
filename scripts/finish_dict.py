# -*- coding: utf-8 -*-
import json
import unicodedata

def strip_accents(text):
    nfkd = unicodedata.normalize('NFKD', text)
    return ''.join([c for c in nfkd if not unicodedata.combining(c)])

path = r'd:\Tradução WTF Zero falls\translations_exe.json'
with open(path, 'r', encoding='utf-8') as f:
    d = json.load(f)

fixes = {
    'Your summary sucked': 'Seu resumo foi pessimo',
    "Didn't listen, I'll ask you again later.": 'Nao prestei atencao, pergunto depois.',
    "I'm feeling almost independent enough to try this strange new research screen on my own. Thanks!": 'Estou me sentindo quase independente para testar essa tela de pesquisa sozinho. Valeu!'
}

for k, v in fixes.items():
    d[k] = strip_accents(v)

for bad in [r'\t|\r', '\t|\r', '']:
    if bad in d:
        del d[bad]

# Verify all values are stripped of accents
for k in list(d.keys()):
    d[k] = strip_accents(d[k])

with open(path, 'w', encoding='utf-8') as f:
    json.dump(d, f, indent=2, ensure_ascii=False)

print('Updated translations_exe.json successfully. Total count:', len(d))

import json

with open(r'd:\Tradução WTF Zero falls\translations_exe.json', 'r', encoding='utf-8') as f:
    d = json.load(f)

with open(r'd:\Tradução WTF Zero falls\scripts\all_game_strings.json', 'r', encoding='utf-8-sig') as f:
    candidates = json.load(f)

missing = [c for c in candidates if c['text'] not in d]
single_words = [c for c in missing if ' ' not in c['text'].strip() and len(c['text'].strip()) > 1]
sentences = [c for c in missing if ' ' in c['text'].strip()]

print('Total missing:', len(missing))
print('Sentences / multi-word phrases:', len(sentences))
print('Single words:', len(single_words))

print('\nSample single words (first 30):')
for s in single_words[:30]:
    print(f"{s['location']:25}: {repr(s['text'])}")

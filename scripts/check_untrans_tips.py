import json

with open('all_tips.txt', 'r', encoding='utf-8') as f:
    lines = [line.strip() for line in f if line.strip()]

with open('translations_exe.json', 'r', encoding='utf-8-sig') as f:
    trans = json.load(f)

untrans = []
for line in lines:
    parts = line.split(' => ')
    if len(parts) == 2:
        caller, text = parts
        pt = trans.get(text, None)
        if pt is None or pt == text:
            untrans.append((caller, text, pt))

print(f'Total tip strings: {len(lines)}')
print(f'Untranslated in trans or missing: {len(untrans)}')
for c, t, p in untrans[:50]:
    print(f'[{c}] "{t}" -> {repr(p)}')

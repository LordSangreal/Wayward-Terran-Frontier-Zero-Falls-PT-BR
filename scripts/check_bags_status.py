import json

with open('translations_exe.json', 'r', encoding='utf-8-sig') as f:
    trans = json.load(f)

for name in ['ITEMBAG', 'LOOTBAG', 'TILEBAG']:
    fname = f'CoOpSpRpG_{name}_clean.txt'
    with open(fname, 'r', encoding='utf-8') as f:
        keys = [l.strip() for l in f if l.strip()]
    
    untrans = []
    missing = []
    translated = []
    
    for k in keys:
        if k not in trans:
            missing.append(k)
        elif trans[k] == k:
            untrans.append(k)
        else:
            translated.append(k)
            
    print(f'=== {name} ===')
    print(f'Total: {len(keys)}, Translated: {len(translated)}, Untrans (k==v): {len(untrans)}, Missing: {len(missing)}')
    if untrans:
        print('  Sample untrans (k==v):', untrans[:10])
    if missing:
        print('  Sample missing:', missing[:10])

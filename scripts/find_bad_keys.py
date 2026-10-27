import json
import re

with open(r'd:\Tradução WTF Zero falls\translations_exe.json', 'r', encoding='utf-8') as f:
    d = json.load(f)

bad_keys = []
for k in d.keys():
    k_lower = k.lower()
    if 'data source=' in k_lower or 'version=3' in k_lower:
        bad_keys.append(k)
    elif k_lower.startswith('create table') or k_lower.startswith('select ') or k_lower.startswith('insert into') or k_lower.startswith('delete from'):
        bad_keys.append(k)
    elif 'primary key' in k_lower or 'pragma' in k_lower:
        bad_keys.append(k)
    elif k.endswith('.sqlite') or k.endswith('.db'):
        bad_keys.append(k)
    elif '=' in k and (';' in k or k.endswith('=')):
        bad_keys.append(k)
    elif k.startswith('{') and k.endswith('}'):
        bad_keys.append(k)
    elif k.startswith('[') and k.endswith(']'):
        # Check if it's regex or code
        if '|' in k or '\\' in k or '^' in k:
            bad_keys.append(k)

print(f"Total bad/technical keys found: {len(bad_keys)}")
for k in bad_keys:
    print(f"  REMOVE: {repr(k)} -> {repr(d[k])}")

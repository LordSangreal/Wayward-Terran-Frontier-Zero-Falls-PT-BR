import json
import re

with open('translations_exe.json', 'r', encoding='utf-8-sig') as f:
    trans = json.load(f)

untrans = [k for k, v in trans.items() if k == v]

print(f'Total untranslated (k==v): {len(untrans)}')

# Patterns to filter out code, technical tokens, single chars, format tokens
def is_translatable(s):
    s = s.strip()
    if len(s) <= 1:
        return False
    # Only numbers and punctuation
    if not re.search(r'[a-zA-Z]', s):
        return False
    # File paths or extensions
    if re.search(r'(\.png|\.xnb|\.txt|\.fx|\.hlsl|\.wav|\.ogg|\.json|\.xml|\\|/|@|\.bin|\.dds)$', s, re.IGNORECASE):
        return False
    if '\\' in s or s.startswith('/') or s.startswith('.'):
        return False
    # Shader/render technical identifiers
    if re.search(r'^(ps_|vs_|technique|sampler|matrix|vec|float|texture\d|diffuse|specular|ambient|render|blend|depth|world|view|proj)', s, re.IGNORECASE):
        return False
    # Single identifiers like "varName" or camelCase / PascalCase without spaces if short
    if ' ' not in s:
        # Check if it looks like a variable name (e.g. m_bSomething, g_pEffect, etc)
        if re.search(r'^(m_|g_|b_|v_|s_|e_|c_|is_|has_|on_)', s):
            return False
        # If it has underscores or camelCase like mStationAccess
        if '_' in s and not re.search(r'^[a-zA-Z]+_[a-zA-Z]+$', s): # allow some simple words
            return False
    # Format strings like {0} {1} alone
    if re.match(r'^\{[0-9:a-zA-Z\s]+\}$', s):
        return False
    return True

candidates = [k for k in untrans if is_translatable(k)]
ignored = [k for k in untrans if not is_translatable(k)]

print(f'Translatable English texts: {len(candidates)}')
print(f'Ignored technical strings: {len(ignored)}')

with open('translatable_candidates.json', 'w', encoding='utf-8') as f:
    json.dump(candidates, f, indent=2, ensure_ascii=False)

print('\nSample 30 translatable candidates:')
for c in candidates[:30]:
    print(' ', repr(c))

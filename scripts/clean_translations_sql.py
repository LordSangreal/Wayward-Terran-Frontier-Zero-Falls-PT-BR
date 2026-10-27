# -*- coding: utf-8 -*-
import json
import re

TRANS_JSON = r"d:\Tradução WTF Zero falls\translations_exe.json"

with open(TRANS_JSON, "r", encoding="utf-8") as f:
    d = json.load(f)

print(f"Total antes da limpeza: {len(d)}")

keys_to_remove = set()

for k in d.keys():
    k_clean = k.strip()
    k_lower = k_clean.lower()

    # 1. SQL queries and DDL
    sql_starts = ['create table', 'select ', 'delete from', 'insert into', 'update ', 'drop table', 'pragma ']
    if any(k_lower.startswith(s) for s in sql_starts):
        # Unless it's user UI dialogue like "Select tool" or "Select Ship"
        if k_lower.startswith('select ') and not any(term in k_lower for term in ['from', '*', 'count', 'where', 'limit', 'id,', 'top,']):
            continue
        keys_to_remove.add(k)
        continue

    # 2. SQLite / Database connection strings
    if 'data source=' in k_lower or 'version=3' in k_lower or k_lower.endswith('.sqlite') or k_lower.endswith('.db'):
        keys_to_remove.add(k)
        continue

    # 3. SQL fragments with SQL keywords
    if any(frag in k_lower for frag in ['primary key', 'values (', 'values(', 'set @', 'where @', 'order by']):
        keys_to_remove.add(k)
        continue

    # 4. Code syntax / technical tokens
    if 'new PlanetGauntlet' in k or 'database.' in k or 'PLAYER.' in k or 'Puppet' in k:
        keys_to_remove.add(k)
        continue

    # 5. Format strings like {0:X2}
    if re.match(r'^\{[0-9]:[A-Za-z0-9]+\}$', k_clean):
        keys_to_remove.add(k)
        continue

    # 6. Technical coordinate prefixes like ' X=', ' Y=', ' bad='
    if k_clean in ['X=', 'Y=', 'bad=', 'id:']:
        keys_to_remove.add(k)
        continue

print(f"Chaves tecnicas/SQL encontradas para remover: {len(keys_to_remove)}")
for k in sorted(keys_to_remove):
    del d[k]

with open(TRANS_JSON, "w", encoding="utf-8") as f:
    json.dump(d, f, indent=2, ensure_ascii=False)

print(f"Dicionario limpo com sucesso! Total restante: {len(d)} termos seguros.")

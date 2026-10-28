import json
import os

BASE_DIR = r"d:\Tradução WTF Zero falls"
TRANS_JSON = os.path.join(BASE_DIR, "translations_exe.json")
PATCHER_JSON = os.path.join(BASE_DIR, r"patcher\translations_exe.json")

AGENT_NAMES_FIX = {
    "One": "One",
    "Anna": "Anna",
    "Bard": "Bard",
    "Budd": "Budd",
    "Emma": "Emma",
    "Mick": "Mick",
    "Xorb": "Xorb",
    "Scot": "Scot",
    "Gary": "Gary",
    "Greg": "Greg",
    "Jake": "Jake",
    "Jane": "Jane",
    "Kali": "Kali",
    "Kyle": "Kyle",
    "Liam": "Liam",
    "Tony": "Tony",
    "Val": "Val",
    "Wade": "Wade",
    "Zero": "Zero",
    "Zoey": "Zoey",
    "Marv": "Marv",
    "Darke": "Darke"
}

for path in [TRANS_JSON, PATCHER_JSON]:
    with open(path, 'r', encoding='utf-8-sig') as f:
        d = json.load(f)
    for k, v in AGENT_NAMES_FIX.items():
        d[k] = v
    with open(path, 'w', encoding='utf-8-sig') as f:
        json.dump(d, f, indent=2, ensure_ascii=False)
    print(f"Fixed agent names in {path}")

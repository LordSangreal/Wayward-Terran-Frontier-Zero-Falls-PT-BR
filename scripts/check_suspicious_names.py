import json

with open('translations_exe.json', 'r', encoding='utf-8-sig') as f:
    d = json.load(f)

# Find suspicious single-word translations
AGENT_NAMES = [
    "One", "Zero", "Anna", "Bard", "Budd", "Emma", "Gary", "Greg", "Jake", "Jane",
    "Kali", "Kyle", "Liam", "Mick", "Xorb", "Scot", "Tony", "Val", "Wade", "Zoey",
    "Marv", "Darke"
]

print("Agent names in dict:")
for name in AGENT_NAMES:
    if name in d:
        print(f"  {repr(name)} -> {repr(d[name])}")

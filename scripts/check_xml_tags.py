import json

with open(r'd:\Tradução WTF Zero falls\translations_exe.json', 'r', encoding='utf-8') as f:
    d = json.load(f)

xml_tags = [
    'Name', 'Page', 'img', 'src', 'body', 'Title', 'title', 
    'entry', 'item', 'desc', 'value', 'type', 'color', 'font', 'bind',
    'width', 'height', 'left', 'right', 'top', 'bottom', 'id', 'text', 'tag'
]

found = []
for t in xml_tags:
    if t in d:
        found.append((t, d[t]))

print("Found XML/DOM tags in translations_exe.json:", len(found))
for k, v in found:
    print(f"  {repr(k)} -> {repr(v)}")

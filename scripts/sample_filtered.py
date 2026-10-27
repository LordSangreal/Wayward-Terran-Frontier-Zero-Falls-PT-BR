import json

with open(r'd:\Tradução WTF Zero falls\scripts\filtered_to_translate.json', 'r', encoding='utf-8') as f:
    items = json.load(f)

print('--- Start sample ---')
for item in items[0:10]:
    print(f"{item['location']:30}: {repr(item['text'][:60])}")

print('\n--- Middle sample 1 ---')
for item in items[1000:1010]:
    print(f"{item['location']:30}: {repr(item['text'][:60])}")

print('\n--- Middle sample 2 ---')
for item in items[2500:2510]:
    print(f"{item['location']:30}: {repr(item['text'][:60])}")

print('\n--- End sample ---')
for item in items[4500:4510]:
    print(f"{item['location']:30}: {repr(item['text'][:60])}")

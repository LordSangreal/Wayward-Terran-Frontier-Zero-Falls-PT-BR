import concurrent.futures
import time
import urllib.request
import urllib.parse
import json

def translate_gt(text):
    if not text.strip():
        return text
    url = 'https://translate.googleapis.com/translate_a/single?client=gtx&sl=en&tl=pt&dt=t&q=' + urllib.parse.quote(text)
    req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0'})
    with urllib.request.urlopen(req, timeout=10) as resp:
        data = json.loads(resp.read().decode('utf-8'))
        return ''.join([part[0] for part in data[0] if part[0]])

test_strings = [
    "Advanced Shield Emitter",
    "The drillbore device is a huge hollow rocket propelled drill",
    "Rocks. This is like dirt, except you can't walk in it",
    "Neutralino Shard",
    "Iron Ore",
    "Gold Ore",
    "Platinum Ore",
    "I see you chose the easy way out.",
    "It was the bullets which got me in the end.",
    "I need a nap before I try again."
] * 3 # 30 strings

start = time.time()
with concurrent.futures.ThreadPoolExecutor(max_workers=10) as executor:
    results = list(executor.map(translate_gt, test_strings))

elapsed = time.time() - start
print(f"Translated {len(results)} strings in {elapsed:.2f} seconds ({len(results)/elapsed:.1f} strings/sec)!")
for orig, res in zip(test_strings[:5], results[:5]):
    print(f"  {orig} -> {res}")

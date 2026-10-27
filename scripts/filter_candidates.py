import json
import re

with open(r'd:\Tradução WTF Zero falls\scripts\all_game_strings.json', 'r', encoding='utf-8-sig') as f:
    candidates = json.load(f)

with open(r'd:\Tradução WTF Zero falls\translations_exe.json', 'r', encoding='utf-8') as f:
    existing = json.load(f)

with open(r'd:\Tradução WTF Zero falls\scripts\shader_blacklist.json', 'r', encoding='utf-8-sig') as f:
    shader_blacklist = set(json.load(f))

KNOWN_NAMES = {
    'Anna', 'Bard', 'Budd', 'Emma', 'Gary', 'Gene', 'Greg', 'Hope', 'Jake', 
    'Jane', 'Kali', 'Kyle', 'Marv', 'Micky', 'One', 'Tony', 'Zero', 'Farby', 
    'Ted', 'Tedd', 'Frank', 'Bill', 'Bob', 'Alice', 'Charlie', 'Dave', 'Eve'
}

def is_safe_translatable(item):
    text = item['text']
    loc = item['location']
    
    if text in existing:
        return False
        
    trimmed = text.strip()
    if len(trimmed) < 2:
        return False
        
    # Strictly reject any shader technique or parameter string!
    if trimmed in shader_blacklist or text in shader_blacklist:
        return False
        
    # Check sprite names like AnnaL, AnnaT, FarbyL, FarbyS, etc.
    if re.match(r'^[A-Z][a-z]+[A-Z]$', trimmed):
        return False
        
    # Check proper names in .ctor or Agent constructors
    if trimmed in KNOWN_NAMES and ('.ctor' in loc or 'Agent' in loc):
        return False
        
    # Check single uppercase word in constructor
    if '.ctor' in loc and ' ' not in trimmed and trimmed[0].isupper() and len(trimmed) < 12:
        if not any(k in loc for k in ['ITEMBAG', 'LOOTBAG', 'TILEBAG', 'TURRET_BAG']):
            return False
            
    # Check code tags or IDs
    if re.match(r'^[a-z0-9_]+$', trimmed) and '_' in trimmed:
        return False
        
    # Check shader/technique/pass methods
    if any(k in loc.lower() for k in ['shader', 'technique', 'render', 'effect']):
        return False
        
    # Single words
    if ' ' not in trimmed:
        if trimmed in ['Accept.', 'Reject.', 'Nevermind.', 'Credits', 'Credits?', 'Done', 'Cancel', 'Close', 'Back', 'Help', 'Options', 'Play', 'Exit', 'Quit', 'Save', 'Load', 'Delete', 'Repair', 'Scrap', 'Build', 'Spawn', 'Equip', 'Unequip', 'Inventory', 'Cargo', 'Research', 'Status', 'Map', 'Log', 'Missions', 'Talents']:
            return True
        if trimmed.endswith('.') or trimmed.endswith('?') or trimmed.endswith('!'):
            return True
        # For items/tiles/loot, single word names like "Iron", "Gold", "Hull", "Armor", "Shields"
        if any(k in loc for k in ['ITEMBAG', 'LOOTBAG', 'TILEBAG', 'TURRET_BAG', 'CHARACTER_DATA']):
            if re.match(r'^[A-Za-z]+$', trimmed) and len(trimmed) >= 3 and trimmed not in shader_blacklist:
                return True
        return False

    # Multi-word string: check if looks like technical path or format
    if '/' in text or '\\' in text:
        if not (' ' in text and ('.' in text or ',' in text or '?' in text or '!' in text)):
            return False
            
    return True

filtered = [c for c in candidates if is_safe_translatable(c)]
print(f'Total candidates: {len(candidates)}')
print(f'Safe filtered for translation: {len(filtered)}')

with open(r'd:\Tradução WTF Zero falls\scripts\filtered_to_translate.json', 'w', encoding='utf-8') as f:
    json.dump(filtered, f, indent=2, ensure_ascii=False)

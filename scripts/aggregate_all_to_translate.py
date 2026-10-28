import json
import os
import re

BASE_DIR = r"d:\Tradução WTF Zero falls"

# Load current translations
with open(os.path.join(BASE_DIR, "translations_exe.json"), 'r', encoding='utf-8-sig') as f:
    main_trans = json.load(f)

# Load shader blacklist
with open(os.path.join(BASE_DIR, r"patcher\shader_blacklist.json"), 'r', encoding='utf-8-sig') as f:
    blacklist = set(json.load(f))

# Additional technical blacklist
EXCLUDE_WORDS = {
    "none", "null", "true", "false", "item", "default", "width", "height",
    "viewport", "bordertechnique", "graphtechnique", "cargopod", "damagetexture",
    "ore", "scanerphase", "scanpointy", "solid", "singlesheet", "shield", "dustsimple",
    "depthtexture", "diffusetexture", "dominantlight", "pointlight", "emisstexture",
    "arcstexture", "shieldtexture", "backgroundtexture", "normaltexture", "reflectivitytexture",
    "reflectivitysource", "lut", "bottexture", "spectexture", "sdesign", "cargobay",
    "texture01", "texture7", "lineworld", "phasecontrast", "gradientcorrection", "masktex",
    "view", "projection", "debugspawn", "state", "throttle", "time", "_active",
    "-airlock", "-bridge", "incomplete", "complete", "failed"
}

def is_user_translatable(s):
    if not s or not isinstance(s, str):
        return False
    s_str = s.strip()
    if len(s_str) < 2:
        return False
    if s_str.lower() in EXCLUDE_WORDS or s_str in blacklist:
        return False
    # Only numbers, symbols
    if not re.search(r'[a-zA-Z]', s_str):
        return False
    # File paths or extensions
    if re.search(r'(\.png|\.xnb|\.txt|\.fx|\.hlsl|\.wav|\.ogg|\.json|\.xml|\\|/|@|\.bin|\.dds)$', s_str, re.IGNORECASE):
        return False
    if '\\' in s_str or (s_str.startswith('/') and ' ' not in s_str):
        return False
    if re.search(r'^(ps_|vs_|technique|sampler|matrix|vec|float|texture\d|diffuse|specular|ambient|render|blend|depth|world|view|proj)', s_str, re.IGNORECASE):
        return False
    # Technical internal tokens with underscores
    if '_' in s_str and ' ' not in s_str:
        return False
    return True

all_candidates = set()

# 1. Untranslated in translations_exe.json
for k, v in main_trans.items():
    if k == v and is_user_translatable(k):
        all_candidates.add(k)

# 2. Batch files
batch_files = ["batch_items_skills.json", "batch_quests.json", "batch_agents.json"]
for bf in batch_files:
    p = os.path.join(BASE_DIR, bf)
    if os.path.exists(p):
        with open(p, 'r', encoding='utf-8-sig') as f:
            data = json.load(f)
            for k in data:
                if is_user_translatable(k):
                    if k not in main_trans or main_trans[k] == k:
                        all_candidates.add(k)

# 3. Clean string dumps
dump_files = [
    "CoOpSpRpG_ITEMBAG_clean.txt",
    "CoOpSpRpG_LOOTBAG_clean.txt",
    "CoOpSpRpG_TILEBAG_clean.txt",
    "CoOpSpRpG_Bedroom_clean.txt",
    "CoOpSpRpG_Airlock_clean.txt",
    "CoOpSpRpG_Door_clean.txt",
    "CoOpSpRpG_ConsoleAccess_clean.txt",
    "CoOpSpRpG_InteractiveRoom_clean.txt",
    "CoOpSpRpG_SPManifest_clean.txt",
    "CoOpSpRpG_TradeOverlay_clean.txt",
    "CoOpSpRpG_VNavigationRev3_clean.txt",
    "CoOpSpRpG_WidgetPrompt_clean.txt",
    "CoOpSpRpG_LogisticsScreenRev3_clean.txt",
    r"scripts\new_unmapped_candidates.txt"
]

for df in dump_files:
    p = os.path.join(BASE_DIR, df)
    if os.path.exists(p):
        with open(p, 'r', encoding='utf-8') as f:
            for line in f:
                s = line.strip()
                if is_user_translatable(s):
                    if s not in main_trans or main_trans[s] == s:
                        all_candidates.add(s)

# 4. all_tips.txt
p = os.path.join(BASE_DIR, "all_tips.txt")
if os.path.exists(p):
    with open(p, 'r', encoding='utf-8') as f:
        for line in f:
            parts = line.strip().split(" => ")
            if len(parts) == 2:
                s = parts[1]
                if is_user_translatable(s):
                    if s not in main_trans or main_trans[s] == s:
                        all_candidates.add(s)

# 5. chat_callers.txt
p = os.path.join(BASE_DIR, "chat_callers.txt")
if os.path.exists(p):
    with open(p, 'r', encoding='utf-8') as f:
        for line in f:
            # format: Caller -> calls WidgetChat::AddMessage with 'text'
            m = re.search(r"with '(.*)'$", line.strip())
            if m:
                s = m.group(1).replace("\\n", "\n")
                if is_user_translatable(s):
                    if s not in main_trans or main_trans[s] == s:
                        all_candidates.add(s)

print(f"Total unified unique candidate strings to translate: {len(all_candidates)}")

# Save to unified JSON
out_path = os.path.join(BASE_DIR, r"scripts\unified_to_translate.json")
with open(out_path, 'w', encoding='utf-8') as f:
    json.dump(sorted(list(all_candidates)), f, indent=2, ensure_ascii=False)

print(f"Saved to {out_path}")

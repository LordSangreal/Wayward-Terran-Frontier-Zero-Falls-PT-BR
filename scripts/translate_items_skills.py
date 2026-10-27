# -*- coding: utf-8 -*-
import json
import os
import re
import unicodedata

BASE_DIR = r"d:\Tradução WTF Zero falls"
ITEMS_JSON = os.path.join(BASE_DIR, "batch_items_skills.json")
TRANS_JSON = os.path.join(BASE_DIR, "translations_exe.json")

def strip_accents(text):
    nfkd = unicodedata.normalize('NFKD', text)
    return ''.join([c for c in nfkd if not unicodedata.combining(c)])

TUNING_PREFIX = "A tuning kit can be used while inside a ship to provide a small perminant bonus to that ship's performance attributes. Using multiple kits of the same type will provide a stacking additive bonus up to a certain limit.\n\nThis kit provides the following bonus: "
TUNING_PT = "Um kit de ajuste pode ser usado dentro de uma nave para conceder um pequeno bonus permanente aos seus atributos. O uso de varios kits do mesmo tipo acumula o bonus ate certo limite.\n\nEste kit concede o seguinte bonus: "

RULES = [
    # Tuning kit header
    (r"A tuning kit can be used while inside a ship to provide a small perminant bonus to that ship's performance attributes\. Using multiple kits of the same type will provide a stacking additive bonus up to a certain limit\.\n\nThis kit provides the following bonus:\s*",
     "Um kit de ajuste concede um bonus permanente a nave. Kits do mesmo tipo acumulam o bonus ate certo limite.\n\nEste kit concede o seguinte bonus: "),
    
    # Tuning kit bonuses
    (r"Forward acceleration is increased by (\d+)%", r"Aceleracao frontal aumentada em \1%"),
    (r"Lateral acceleration is increased by (\d+)%", r"Aceleracao lateral aumentada em \1%"),
    (r"Rotational acceleration is increased by (\d+)%", r"Aceleracao de rotacao aumentada em \1%"),
    (r"Turret rate of fire is increased by (\d+)%", r"Cadencia de tiro das torretas aumentada em \1%"),
    (r"Sensor strength is increased by (\d+)%", r"Potencia dos sensores aumentada em \1%"),
    (r"Turret traverse speed is increased by (\d+)%", r"Velocidade de giro das torretas aumentada em \1%"),
    (r"Turret reload rate is increased by (\d+)%", r"Velocidade de recarga das torretas aumentada em \1%"),
    (r"Turret range is increased by (\d+)%", r"Alcance das torretas aumentado em \1%"),
    (r"Armor tile damage reduction is increased by (\d+)%", r"Reducao de dano da blindagem aumentada em \1%"),
    (r"Shield tile damage reduction is increased by (\d+)%", r"Reducao de dano dos escudos aumentada em \1%"),
    (r"All incoming damage is reduced by (\d+)%", r"Todo dano recebido e reduzido em \1%"),
    (r"Damage with projectile weapons is increased by (\d+)%", r"Dano de armas balisticas aumentado em \1%"),
    (r"Damage with beam weapons is increased by (\d+)%", r"Dano de armas de raio aumentado em \1%"),
    (r"Chance to evade shots is increased by ([\d\.]+)%", r"Chance de esquivar de disparos aumentada em \1%"),
    (r"Double shot chance is increased by ([\d\.]+)%", r"Chance de disparo duplo aumentada em \1%"),
    (r"Damage from missiles is reduced by (\d+)%", r"Dano de misseis reduzido em \1%"),
    (r"Damage from beams is reduced by (\d+)%", r"Dano de raios reduzido em \1%"),
    (r"Turret magazines hold (\d+)% more ammo", r"Carregadores de torreta comportam \1% mais municao"),
    (r"Turrets use (\d+)% less energy when firing", r"Torretas consomem \1% menos energia ao disparar"),
    (r"Bullets and missiles slow enemy turn rate by (\d+)% for (\d+) seconds", r"Projeteis e misseis reduzem o giro inimigo em \1% por \2 segundos"),
    (r"Shield generators store (\d+)% more shield energy", r"Geradores de escudo armazenam \1% mais energia"),
    (r"Reduces the amount of shield energy needed to create 1 pixel of shield by (\d+)%", r"Reduz em \1% a energia para criar 1 pixel de escudo"),
    (r"Maximum travel drive speed increased by (\d+)", r"Velocidade maxima do motor de viagem aumentada em \1"),
    (r"Travel drive spool rate \+(\d+)%", r"Taxa de ativacao do motor de viagem +\1%"),

    # Stats and terms
    (r"^missile tracking$", "rastreamento de misseis"),
    (r"^missile range$", "alcance de misseis"),
    (r"^shield damage$", "dano ao escudo"),
    (r"^armor damage$", "dano a blindagem"),
    (r"^ionic discharge$", "descarga ionica"),
    (r"^impact absorb$", "absorcao de impacto"),
    (r"^impact harden$", "resistencia a impacto"),

    # Schematics
    (r"^Design schematics for a (small|medium|large|tiny) (.+?)\.?$", r"Esquema de projeto: \2 (\1)"),
    (r"^Design schematics for (.+?)\.?$", r"Esquema de projeto: \1"),
    (r"^Unlock crafting of (.+?) in the inventory.*$", r"Desbloqueia a fabricacao de \1 no inventario."),
    (r"^(.+?) crafting$", r"Fabricacao de \1"),

    # Common Item Descriptions
    (r"^Stores up to (\d+) energy in an internal capacitor bank\.", r"Armazena ate \1 de energia em um banco interno de capacitores."),
    (r"^Generates (\d+) units of shield per second\.", r"Gera \1 unidades de escudo por segundo."),
    (r"^Consumes (\d+) energy per second while active\.", r"Consome \1 de energia por segundo enquanto ativo."),
    (r"^Produces (\d+) energy per second\.", r"Produz \1 de energia por segundo.")
]

def translate_entry(s):
    # Check regex rules
    for pattern, repl in RULES:
        if re.search(pattern, s, re.IGNORECASE):
            res = re.sub(pattern, repl, s, flags=re.IGNORECASE)
            return res

    # Sizes
    size_map = {
        "Small ": "Pequeno ",
        "Medium ": "Medio ",
        "Large ": "Grande ",
        "Tiny ": "Minusculo ",
        "Heavy ": "Pesado ",
        "Light ": "Leve "
    }
    for k, v in size_map.items():
        if s.startswith(k):
            sub = s[len(k):]
            if "Capacitor" in sub:
                return f"Capacitor {v.strip().lower()}"
            if "Reactor" in sub:
                return f"Reator {v.strip().lower()}"
            if "Shield" in sub:
                return f"Escudo {v.strip().lower()}"
            if "Engine" in sub:
                return f"Motor {v.strip().lower()}"
            if "Thruster" in sub:
                return f"Propulsor {v.strip().lower()}"

    return None

def main():
    print("Executando expansão da tradução de itens, peças e módulos...")
    with open(ITEMS_JSON, "r", encoding="utf-8-sig") as f:
        items = json.load(f)

    with open(TRANS_JSON, "r", encoding="utf-8-sig") as f:
        trans = json.load(f)

    added = 0
    for k in items:
        if k in trans:
            continue
        res = translate_entry(k)
        if res:
            clean = strip_accents(res)
            trans[k] = clean
            added += 1

    with open(TRANS_JSON, "w", encoding="utf-8") as f:
        json.dump(trans, f, indent=2, ensure_ascii=False)

    print(f"Sucesso! {added} novas traduções adicionadas.")
    print(f"Total acumulado: {len(trans)} termos no dicionário.")

if __name__ == "__main__":
    main()

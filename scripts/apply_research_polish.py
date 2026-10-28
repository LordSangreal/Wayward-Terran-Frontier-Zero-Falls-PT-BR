# -*- coding: utf-8 -*-
import json
import os
import re

def clean_ascii(s):
    # Ensure strict ASCII compliance (32-126 plus \n, \r, \t)
    out = []
    for c in s:
        code = ord(c)
        if 32 <= code <= 126 or c in ('\n', '\r', '\t'):
            out.append(c)
        else:
            # Map common accented characters to ASCII equivalents
            mapping = {
                u'á': 'a', u'à': 'a', u'ã': 'a', u'â': 'a', u'ä': 'a',
                u'é': 'e', u'è': 'e', u'ê': 'e', u'ë': 'e',
                u'í': 'i', u'ì': 'i', u'î': 'i', u'ï': 'i',
                u'ó': 'o', u'ò': 'o', u'õ': 'o', u'ô': 'o', u'ö': 'o',
                u'ú': 'u', u'ù': 'u', u'û': 'u', u'ü': 'u',
                u'ç': 'c',
                u'Á': 'A', u'À': 'A', u'Ã': 'A', u'Â': 'A', u'Ä': 'A',
                u'É': 'E', u'È': 'E', u'Ê': 'E', u'Ë': 'E',
                u'Í': 'I', u'Ì': 'I', u'Î': 'I', u'Ï': 'I',
                u'Ó': 'O', u'Ò': 'O', u'Õ': 'O', u'Ô': 'O', u'Ö': 'O',
                u'Ú': 'U', u'Ù': 'U', u'Û': 'U', u'Ü': 'U',
                u'Ç': 'C',
                u'“': '"', u'”': '"', u'‘': "'", u'’': "'",
                u'–': '-', u'—': '-'
            }
            out.append(mapping.get(c, ''))
    return "".join(out)

with open('translations_exe.json', 'r', encoding='utf-8-sig') as f:
    d = json.load(f)

overrides = {
    # 1. Research details overlap fix (starts with \n and uses compact lines)
    " \n \n  - You will be able to remove this module from existing designs \n \n - You will be able to add this module to any ship design \n \n - You will get progress in Tier ": "\n- Remover de designs existentes\n- Adicionar a qualquer projeto de nave\n- Progresso no nivel ",
    "Research to unlock the following module: \n - ": "Pesquise para desbloquear este modulo:\n- ",
    "\n  - You will be able to remove this module from existing designs\n  - You will be able to add this module to any ship design": "\n- Remover de designs existentes\n- Adicionar a qualquer projeto de nave",

    # 2. Research category filters & tier consistency
    "General": "Geral",
    "Ships": "Naves",
    "Tier 1": "Nivel 1",
    "Tier 2": "Nivel 2",
    "Tier 3": "Nivel 3",
    "Tier 1 progress": "Progresso de nivel 1",
    "Tier 2 progress": "Progresso de nivel 2",
    "Tier 3 progress": "Progresso de nivel 3",

    # 3. WidgetResources / Stats HUD headers
    "[Maneuvering stats]": "[Estatisticas de Manobra]",
    "[Turret stats]": "[Estatisticas de Torretas]",
    "[Defensive stats]": "[Estatisticas Defensivas]",
    "[Point defense stats]": "[Defesa de Ponto]",
    "[Misc stats]": "[Outras Estatisticas]",
    "Skills": "Habilidades",
    "Reputations": "Reputacoes",
    "Acceleration - ": "Aceleracao - ",
    "Turning - ": "Giro - ",
    "Strafing - ": "Deslocamento lateral - ",
    "Fire rate multiplier - ": "Multiplicador de cadencia - ",
    "Damage multiplier - ": "Multiplicador de dano - ",
    "Crit chance - ": "Chance de critico - ",
    "Double shot chance - ": "Chance de tiro duplo - ",
    "Turret traverse rate - ": "Velocidade de giro da torreta - ",
    "Bullet duration multiplier - ": "Multiplicador de duracao do projetil - ",
    "Bullet velocity multiplier - ": "Multiplicador de velocidade do projetil - ",
    "Reload speed - ": "Velocidade de recarga - ",
    "Accuracy - ": "Precisao - ",
    "Dodge - ": "Esquiva - ",
    "Glance - ": "Raspao - ",
    "Glance hardness - ": "Dureza do raspao - ",
    "Shield hardness - ": "Dureza do escudo - ",
    "Armor hardness - ": "Dureza da blindagem - ",
    "PD Range - ": "Alcance PD - ",
    "PD Damage - ": "Dano PD - ",
    "PD Recharge - ": "Recarga PD - ",
    "PD Hit chance - ": "Chance de acerto PD - ",
    "Sensors - ": "Sensores - ",
    "Visibility - ": "Visibilidade - ",
    "Boost charge - ": "Carga do propulsor - ",
    "Ore yield - ": "Rendimento de minerio - ",
    " - Current level: ": " - Nivel atual: ",

    # 4. Turret mounts & drone cleanups
    "Tiny Gun Mount\nCan only fire in 1 direction\nbut charges much faster than turret mounts": "Suporte de Arma Minusculo\nSo pode disparar em 1 direcao\nmas carrega muito mais rapido que torretas",
    "Small Energy T2 Turret Mount": "Suporte de Torreta de Energia T2 Pequeno",
    "Medium Energy Turret Mount": "Suporte de Torreta de Energia Medio",
    "Tiny Ballistic Turret Mount": "Suporte de Torreta Balistica Minusculo",
    "Small Ballistic Turret Mount": "Suporte de Torreta Balistica Pequeno",
    "Medium Ballistic Turret Mount": "Suporte de Torreta Balistica Medio",
    "Tiny Energy Turret Mount": "Suporte de Torreta de Energia Minusculo",
    "Small Energy Turret Mount": "Suporte de Torreta de Energia Pequeno",
    "Small Turret Mount\nYeah, this is the small one": "Suporte de Torreta Pequeno\nSim, este e o modelo pequeno",
    "Tiny Turret Mount\nThe smallest mount that exists": "Suporte de Torreta Minusculo\nO menor suporte que existe",
    "Projectile Attack Drones 4x": "Drones de Ataque com Projeteis 4x",
    "Siege Missile Magazine": "Compartimento de Misseis de Cerco",
    "Attack Drones 2x": "Drones de Ataque 2x",
    "Small Turret Mount": "Suporte de Torreta Pequeno",
    "Tiny Gun Mount": "Suporte de Arma Minusculo",
    "Tiny Turret Mount": "Suporte de Torreta Minusculo",
    "View range": "Alcance de visao",

    # 5. Schematics translations (clean, fluent Portuguese, strict ASCII)
    "Design schematics for a hail turret which fires short, intense bursts of inaccurate bullets": "Esquema de projeto para torreta de granizo que dispara rajadas curtas e intensas.",
    "Design schematics for a small vulcan turret, an extremely fast firing and accurate machine gun. This one is for small ballistic mounts": "Esquema de projeto para torreta vulcan pequena, metralhadora rapida e precisa para suportes balisticos pequenos.",
    "Design schematics for a small rocket turret.": "Esquema de projeto para torreta de foguetes pequena.",
    "Design schematics for a medium rocket turret.": "Esquema de projeto para torreta de foguetes media.",
    "Design schematics for a tiny railgun repeater.": "Esquema de projeto para repetidor eletromagnetico minusculo.",
    "Design schematics for a small railgun repeater.": "Esquema de projeto para repetidor eletromagnetico pequeno.",
    "Design schematics for a small railgun array.": "Esquema de projeto para matriz eletromagnetica pequena.",
    "Design schematics for a medium railgun array.": "Esquema de projeto para matriz eletromagnetica media.",
    "Design schematics for a small rail gun.": "Esquema de projeto para canhao eletromagnetico pequeno.",
    "Design schematics for a medium rail gun.": "Esquema de projeto para canhao eletromagnetico medio.",
    "Design schematics for a large rail gun.": "Esquema de projeto para grande canhao eletromagnetico.",
    "Design schematics for a deadly large vulcan turret.": "Esquema de projeto para torreta vulcan grande e letal.",
    "Design schematics for a small flak turret.": "Esquema de projeto para torreta flak pequena.",
    "Design schematics for a medium flak turret.": "Esquema de projeto para torreta flak media.",
    "Design schematics for a small cluster turret. An upgraded version of the flak turret.": "Esquema de projeto para torreta de fragmentacao pequena, versao aprimorada da flak.",
    "Design schematics for a medium cluster turret.": "Esquema de projeto para torreta de fragmentacao media.",
    "Design schematics for a large cluster turret.": "Esquema de projeto para torreta de fragmentacao grande.",
    "Design schematics for a small antimatter rocket turret.": "Esquema de projeto para torreta de foguetes de antimateria pequena.",
    "Design schematics for a medium antimatter rocket turret.": "Esquema de projeto para torreta de foguetes de antimateria media.",
    "Design schematics for a heavy antimatter rocket turret.": "Esquema de projeto para torreta de foguetes de antimateria pesada.",
    "Design schematics for a 65mm mass driver turret.": "Esquema de projeto para torreta propulsora de massa de 65mm.",
    "Design schematics for a 95mm mass driver turret.": "Esquema de projeto para torreta propulsora de massa de 95mm.",
    "Design schematics for a 130mm mass driver turret.": "Esquema de projeto para torreta propulsora de massa de 130mm.",
    "Design schematics for a medium graviton turret.": "Esquema de projeto para torreta de graviton media.",
    "Design schematics for a large graviton turret.": "Esquema de projeto para torreta de graviton grande.",
    "Design schematics for a heavy graviton turret.": "Esquema de projeto para torreta de graviton pesada.",
    "Design schematics for a dual 620mm gun turret designed for heavy mounts.": "Esquema de projeto para torreta dupla de 620mm para suportes pesados.",
    "Design schematics for a hellbore cannon to be attached on a large turret mount.": "Esquema de projeto para canhao infernal em suporte de torreta grande.",
    "Design schematics for a hellbore cannon to be attached on a heavy turret mount.": "Esquema de projeto para canhao infernal em suporte de torreta pesado.",
    "Design schematics for a HellHammer cannon to be attached on a heavy turret mount.": "Esquema de projeto para canhao Martelo Infernal em suporte pesado.",
    "Design schematics for a heavy railgun to be attached on a heavy turret mount.": "Esquema de projeto para canhao eletromagnetico em suporte pesado.",
    "Design schematics for a double large railgun to be attached on a heavy turret mount.": "Esquema de projeto para canhao eletromagnetico duplo grande em suporte pesado.",
    "Design schematics for a super heavy railgun to be attached on a heavy turret mount.": "Esquema de projeto para canhao eletromagnetico superpesado em suporte pesado.",
    "Design schematics for a 620mm gun turret designed for large mounts.": "Esquema de projeto para torreta de canhao de 620mm para suportes grandes.",
    "Design schematics for a triple 620mm gun turret designed for heavy mounts.": "Esquema de projeto para torreta tripla de canhao de 620mm para suportes pesados.",
    "Design schematics for a vortex coil gun to be attached on a large turret mount.": "Esquema de projeto para bobina de vortice em suporte de torreta grande.",
    "Design schematics for a heavy vortex coil gun to be attached on a heavy turret mount.": "Esquema de projeto para bobina de vortice pesada em suporte pesado.",
    "Design schematics for a dual vortex coil gun to be attached on a heavy turret mount.": "Esquema de projeto para bobina de vortice dupla em suporte pesado.",
    "Design schematics for a pulse beam to be attached on a small energy turret mount.": "Esquema de projeto para feixe de pulso em suporte de energia pequeno.",
    "Design schematics for a pulse beam to be attached on a medium energy turret mount.": "Esquema de projeto para feixe de pulso em suporte de energia medio.",
    "Design schematics for a pulse beam to be attached on a large energy turret mount.": "Esquema de projeto para feixe de pulso em suporte de energia grande.",
    "Design schematics for a pulse weapon to be attached on a small energy turret mount.": "Esquema de projeto para arma de pulso em suporte de energia pequeno.",
    "Design schematics for a pulse weapon to be attached on a medium energy turret mount.": "Esquema de projeto para arma de pulso em suporte de energia medio.",
    "Design schematics for a pulse weapon to be attached on a large energy turret mount.": "Esquema de projeto para arma de pulso em suporte de energia grande.",
    "Design schematics for a ballistic weapon to be attached on a large ballistic turret mount.": "Esquema de projeto para arma balistica em suporte balistico grande.",
    "Design schematics for a ballistic weapon to be attached on a heavy ballistic turret mount.": "Esquema de projeto para arma balistica em suporte balistico pesado.",
    "Design schematics for a ballistic weapon to be attached on a super heavy ballistic turret mount.": "Esquema de projeto para arma balistica em suporte balistico superpesado.",
    "Design schematics for a ballistic weapon": "Esquema de projeto para uma arma balistica.",
    "Design schematics for a tier 3 point defense energy weapon": "Esquema de projeto para arma de energia de defesa de ponto nivel 3.",
    "Design schematics for a small tier 3 pulse beam energy weapon": "Esquema de projeto para feixe de pulso de energia nivel 3 pequeno.",
    "Design schematics for a medium tier 3 pulse beam energy weapon": "Esquema de projeto para feixe de pulso de energia nivel 3 medio.",
    "Design schematics for a large tier 3 pulse beam energy weapon": "Esquema de projeto para feixe de pulso de energia nivel 3 grande.",
    "Design schematics for a heavy tier 3 pulse beam energy weapon": "Esquema de projeto para feixe de pulso de energia nivel 3 pesado.",
    "Design schematics for a super heavy tier 3 pulse beam energy weapon": "Esquema de projeto para feixe de pulso de energia nivel 3 superpesado."
}

updated_count = 0
for k, v in overrides.items():
    cleaned_v = clean_ascii(v)
    d[k] = cleaned_v
    updated_count += 1

print(f"Applied {updated_count} overrides to dictionary.")

# Ensure ALL values in dictionary are strictly ASCII
non_ascii_fixed = 0
for k, v in list(d.items()):
    cleaned_v = clean_ascii(v)
    if cleaned_v != v:
        d[k] = cleaned_v
        non_ascii_fixed += 1

print(f"Fixed {non_ascii_fixed} non-ASCII values across dictionary.")

# Write back to root and patcher
with open('translations_exe.json', 'w', encoding='utf-8') as f:
    json.dump(d, f, ensure_ascii=False, indent=2)

with open('patcher/translations_exe.json', 'w', encoding='utf-8') as f:
    json.dump(d, f, ensure_ascii=False, indent=2)

print("Saved clean dictionary to translations_exe.json and patcher/translations_exe.json successfully.")

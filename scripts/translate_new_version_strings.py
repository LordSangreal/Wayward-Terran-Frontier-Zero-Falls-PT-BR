# -*- coding: utf-8 -*-
import json
import os
import re
import time
import unicodedata
import urllib.request
import urllib.parse
from concurrent.futures import ThreadPoolExecutor, as_completed

BASE_DIR = r"d:\Tradução WTF Zero falls"
TRANS_JSON = os.path.join(BASE_DIR, "translations_exe.json")
FILTERED_JSON = os.path.join(BASE_DIR, r"scripts\filtered_new_strings.json")
NEW_DICT_JSON = os.path.join(BASE_DIR, r"scripts\new_translations_patch.json")

def strip_accents(text):
    if not text:
        return text
    nfkd = unicodedata.normalize('NFKD', text)
    return ''.join([c for c in nfkd if not unicodedata.combining(c)])

# Termos de classes, mecânicas e habilidades do Zero Falls
MANUAL_TERMS = {
    "Wage slave": "Operario assalariado",
    "Computer programmer": "Programador de computador",
    "Treasure hunter": "Cacador de tesouros",
    "Class mechanic: Preparation": "Mecanica de classe: Preparacao",
    "Class mechanic: Focus": "Mecanica de classe: Foco",
    "Class mechanic: Stamina": "Mecanica de classe: Resistencia",
    "Rune selection:": "Selecao de runa:",
    "Character background:": "Historico do personagem:",
    "Create character": "Criar personagem",
    "Starting Ability: ": "Habilidade inicial: ",
    "Max Smelting": "Fundicao maxima",
    "Max Weaponsmithing": "Armaria maxima",
    "Max engineering": "Engenharia maxima",
    "Max salvaging": "Desmanche maximo",
    "Max smelting": "Fundicao maxima",
    "Max weaponsmithing": "Armaria maxima",
    "Max archeology": "Arqueologia maxima",
    "Max artificing": "Artificie maximo",
    "Ore Yield": "Rendimento de minerio",
    "Turret fire rate": "Cadencia de tiro da torreta",
    "Crit chance": "Chance de critico",
    "Critical damage": "Dano critico",
    "Turret traverse": "Giro da torreta",
    "Turret reload rate": "Velocidade de recarga da torreta",
    "Turret accuracy ": "Precisao da torreta ",
    "Turret accuracy": "Precisao da torreta",
    "Bullet range ": "Alcance do projetil ",
    "Travel drive charge": "Carga do motor de viagem",
    "Rotational acceleration": "Aceleracao de rotacao",
    "Forward acceleration": "Aceleracao frontal",
    "Glancing hardness": "Dureza de raspao",
    "Shield energy use": "Consumo de energia do escudo",
    "Double shot chance": "Chance de disparo duplo",
    "Glance chance added": "Chance de raspao adicionada",
    "Armor hardness": "Dureza da blindagem",
    "Glance chance": "Chance de raspao",
    "Glance hardness": "Dureza de raspao",
    "Shield projected": "Escudo projetado",
    "Shield projection duration": "Duracao da projecao de escudo",
    "Set price per stack": "Definir preco por lote",
    "EVA into space? This will likely kill you.": "Sair em AEV no espaco? Isso provavelmente vai te matar.",
    "Auto pilot destination plotted": "Destino do piloto automatico tracado",
    "Logged out": "Desconectado",
    "Message from ": "Mensagem de ",
    "MSCS Pod": "Capsula MSCS",
    "Colony nexus": "Nucleo da colonia",
    "Station Deploy Kit": "Kit de implantacao de estacao",
    "Maximum Stacks": "Acumulos maximos",
    "Level Required": "Nivel necessario",
    "Base level": "Nivel base",
    "Adds a random rune": "Adiciona uma runa aleatoria",
    "Adds guaranteed rune: ": "Adiciona runa garantida: ",
    "Digital id": "Identidade digital",
    "POI data": "Dados de PDI",
    "This item begins a quest": "Este item inicia uma missao",
    "Generate New World": "Gerar Novo Mundo",
    "Sandbox world": "Mundo Sandbox",
    "Generated Worlds": "Mundos Gerados",
    "Parameter DBs": "Bancos de Parametros",
    "Magic missile": "Missil magico",
    "Magic missile (2)": "Missil magico (2)",
    "Magic missile (3)": "Missil magico (3)",
    "Logistics Assistance": "Assistencia logistica",
    "Enhanced support": "Suporte aprimorado",
    "Support affinity": "Afinidade de suporte",
    "Regenerating shields": "Escudos regenerativos",
    "Additional hardness": "Dureza adicional",
    "Target turn rate": "Taxa de giro do alvo",
    "Command Presence": "Presenca de comando",
    "Command Assault": "Comando de assalto",
    "Secondary Loaders": "Carregadores secundarios",
    "Inertial Pulse": "Pulso inercial",
    "Instant Poise": "Postura instantanea",
    "Poise over duration": "Postura durante o tempo",
    "Engines Overloaded": "Motores sobrecarregados",
    "Flank speed": "Velocidade de flanco",
    "Engines restarting": "Reiniciando motores",
    "Neutrino Pulse": "Pulso de neutrino",
    "Flux Vent": "Descarga de fluxo",
    "Running dark": "Navegacao furtiva",
    "Lock In": "Foco travado",
    "Go dark": "Entrar em modo furtivo",
    "Alpha strike": "Ataque alfa",
    "Alpha strike (2)": "Ataque alfa (2)",
    "Alpha strike (3)": "Ataque alfa (3)",
    "Range Extended": "Alcance estendido",
    "Active targeting": "Mira ativa",
    "Shield transfer": "Transferencia de escudo",
    "Bullet Master V": "Mestre de Projeteis V",
    "The Shootifier": "O Disparador",
    "Bullet Apprentice": "Aprendiz de Projeteis",
    "Bullet Journeyman": "Praticante de Projeteis",
    "Aimfast Extreme": "Mira Rapida Extrema",
    "Crit Cube": "Cubo Critico",
    "Bullet Go Fast": "Projetil Veloz",
    "Hot Silicon": "Silicio Quente",
    "Mysterious Sand": "Areia Misteriosa",
    "Plasma Absorber": "Absorvedor de Plasma",
    "Plasma Deflector": "Defletor de Plasma",
    "Cosmic Mirror": "Espelho Cosmico",
    "Plasma Evader": "Evasor de Plasma",
    "Hardened Evader": "Evasor Blindado",
    "Shot Bouncer": "Rebatedor de Tiros",
    "Local map": "Mapa local",
    "Districts map": "Mapa de distritos",
    "Hazards map": "Mapa de perigos",
    "University map": "Mapa da universidade",
    "Empire map": "Mapa do imperio",
    "Station image": "Imagem da estacao",
    "Acceleration -": "Aceleracao -",
    "Turning -": "Giro -",
    "Passenger delivery": "Transporte de passageiros",
    "Artifact Squiggle": "Artefato Serpentina",
    "Artifact Sphere": "Artefato Esfera",
    "Artifact Triangle": "Artefato Triangulo",
    "Artifact Connecty Bit": "Artefato Conector",
    "Artifact Tube": "Artefato Tubo",
    "Electrical wiring": "Fiacao eletrica",
    "Advanced Mechanism": "Mecanismo Avancado"
}

def clean_term(pt_text):
    t = pt_text
    t = t.replace("travel drive", "motor de viagem")
    t = t.replace("Travel drive", "Motor de viagem")
    t = t.replace("Travel Drive", "Motor de viagem")
    t = t.replace("cloning vat", "tanque de clonagem")
    t = t.replace("matter furnace", "fornalha de materia")
    t = t.replace("airlock", "eclusa")
    t = t.replace("shipyard", "estaleiro")
    t = t.replace("hangar", "hangar")
    t = t.replace("bridge", "ponte de comando")
    t = t.replace("waypoint", "ponto de rota")
    t = t.replace("poise", "postura")
    t = t.replace("Poise", "Postura")
    t = t.replace("buff", "efeito benefico")
    return strip_accents(t)

def translate_gt(text):
    if text in MANUAL_TERMS:
        return clean_term(MANUAL_TERMS[text])
    
    # Se for string de número/símbolo simples
    if re.match(r'^[\d\s\+\-\%\.\,\:\;\[\]\(\)\/]+$', text):
        return text

    for attempt in range(3):
        try:
            url = 'https://translate.googleapis.com/translate_a/single?client=gtx&sl=en&tl=pt&dt=t&q=' + urllib.parse.quote(text)
            req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0'})
            with urllib.request.urlopen(req, timeout=10) as resp:
                data = json.loads(resp.read().decode('utf-8'))
                res = ''.join([part[0] for part in data[0] if part[0]])
                return clean_term(res)
        except Exception as e:
            time.sleep(0.5 * (attempt + 1))
    return clean_term(text)

def main():
    with open(FILTERED_JSON, 'r', encoding='utf-8') as f:
        strings = json.load(f)

    print(f"Total de novas strings para traduzir: {len(strings)}")
    new_translations = {}

    with ThreadPoolExecutor(max_workers=8) as executor:
        future_to_str = {executor.submit(translate_gt, s): s for s in strings}
        completed = 0
        for future in as_completed(future_to_str):
            s = future_to_str[future]
            try:
                trans = future.result()
                new_translations[s] = trans
            except Exception as e:
                new_translations[s] = clean_term(s)
            completed += 1
            if completed % 100 == 0 or completed == len(strings):
                print(f"Progresso: {completed}/{len(strings)}")

    with open(NEW_DICT_JSON, 'w', encoding='utf-8') as f:
        json.dump(new_translations, f, indent=2, ensure_ascii=False)

    print("Novas traducoes salvas em:", NEW_DICT_JSON)

    # Merge into translations_exe.json
    with open(TRANS_JSON, 'r', encoding='utf-8-sig') as f:
        main_dict = json.load(f)

    print(f"Dicionario principal tinha {len(main_dict)} termos.")
    added = 0
    for k, v in new_translations.items():
        if k not in main_dict or main_dict[k] != v:
            main_dict[k] = v
            added += 1

    print(f"Adicionados/atualizados {added} termos no dicionario principal.")

    with open(TRANS_JSON, 'w', encoding='utf-8-sig') as f:
        json.dump(main_dict, f, indent=2, ensure_ascii=False)

    # Also update patcher/translations_exe.json
    patcher_json = os.path.join(BASE_DIR, r"patcher\translations_exe.json")
    with open(patcher_json, 'w', encoding='utf-8-sig') as f:
        json.dump(main_dict, f, indent=2, ensure_ascii=False)

    print("translations_exe.json atualizado com sucesso no patcher e na raiz!")

if __name__ == '__main__':
    main()

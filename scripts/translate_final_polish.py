# -*- coding: utf-8 -*-
import json
import os
import re
import time
import urllib.request
import urllib.parse
import unicodedata
import concurrent.futures

BASE_DIR = r"d:\Tradução WTF Zero falls"
TRANS_JSON = os.path.join(BASE_DIR, "translations_exe.json")
ALL_STRINGS_JSON = os.path.join(BASE_DIR, r"scripts\all_game_strings.json")
BLACKLIST_JSON = os.path.join(BASE_DIR, r"scripts\shader_blacklist.json")

def strip_accents(text):
    if not text:
        return text
    nfkd = unicodedata.normalize('NFKD', text)
    return ''.join([c for c in nfkd if not unicodedata.combining(c)])

def clean_term(pt):
    t = pt
    t = t.replace("travel drive", "motor de viagem")
    t = t.replace("Travel Drive", "Motor de viagem")
    t = t.replace("cloning vat", "tanque de clonagem")
    t = t.replace("matter furnace", "fornalha de materia")
    t = t.replace("airlock", "eclusa")
    t = t.replace("shipyard", "estaleiro")
    t = t.replace("hangar", "hangar")
    t = t.replace("bridge", "ponte de comando")
    return strip_accents(t)

# Direct translations for specific UI and rank terms
DIRECT_TERMS = {
    'Initiate': 'Iniciado',
    'Apprentice': 'Aprendiz',
    'Accolyte': 'Acolito',
    'Adept': 'Adepto',
    'Guide': 'Guia',
    'Paragon': 'Exemplo',
    'Hero': 'Heroi',
    'Harbinger': 'Precursor',
    'Intern': 'Estagiario',
    'Associate': 'Associado',
    'Executive': 'Executivo',
    'Administrator': 'Administrador',
    'Lieutenant': 'Tenente',
    'Commander': 'Comandante',
    'Captain': 'Capitao',
    'Major': 'Major',
    'Independent': 'Independente',
    'Recruit': 'Recrutar',
    'Respawn': 'Renascer',
    'Provision': 'Abastecer',
    'Select': 'Selecionar',
    'Quicksave': 'Salvar Rapido',
    'Categories': 'Categorias',
    'Descripton': 'Descricao',
    'Description': 'Descricao',
    'Description:': 'Descricao:',
    'Apply': 'Aplicar',
    'Movement': 'Movimento',
    'Activate': 'Ativar',
    'Fullscreen': 'Tela Cheia',
    'Borderless': 'Sem Bordas',
    'Windowed': 'Em Janela',
    'Gameplay': 'Jogabilidade',
    'Unlocked': 'Desbloqueado',
    'Centered': 'Centralizado',
    'Control': 'Controle',
    'Fight': 'Lutar',
    'Blueprints': 'Projetos',
    'Artifact': 'Artefato',
    'Missile': 'Missil',
    'Quantity ': 'Quantidade ',
    'Price: ': 'Preco: ',
    'Scrap: ': 'Sucata: ',
    'Credits: ': 'Creditos: ',
    'Name: ': 'Nome: ',
    'Submit': 'Confirmar',
    'LEAVE': 'SAIR',
    'DELETE': 'EXCLUIR',
    'Loading...': 'Carregando...',
    'Loading: ': 'Carregando: ',
    'Bound ': 'Vinculado ',
    ' to: ': ' para: ',
    ' seconds': ' segundos',
    ' shots': ' disparos',
    ' credits': ' creditos',
    ' exp': ' exp',
    ' scrap': ' sucata',
    ' beds:': ' camas:',
    'Quality ': 'Qualidade ',
    'Mount: ': 'Suporte: ',
    'Stage ': 'Fase ',
    ' begins': ' comeca',
    'Disconnected: ': 'Desconectado: ',
    'Consumable: ': 'Consumivel: ',
    'Requires ': 'Requer ',
    'Deliver ': 'Entregar ',
    'Hack': 'Hackear',
    'Abort': 'Abortar',
    'Replace ': 'Substituir ',
    'Move up / accelerate ship': 'Mover para cima / acelerar nave',
    'Move down / decelerate ship': 'Mover para baixo / desacelerar nave',
    'Gun inaccuracy': 'Imprecisao da arma',
    'Bullet duration': 'Duracao do projetil',
    'Bullet damage': 'Dano do projetil',
    'Bullet velocity': 'Velocidade do projetil',
    'Aimed Volley': 'Disparo Mirado',
    'Smart Engineering': 'Engenharia Inteligente',
    'Reactor heat capacity': 'Capacidade de calor do reator',
    'Radiator cooling': 'Resfriamento do radiador',
    'Reactor output': 'Potencia do reator',
    'Reactor efficiency': 'Eficiencia do reator',
    'Field specialist': 'Especialista em campos',
    'Capacitor capacity': 'Capacidade do capacitor',
    'Activation cost': 'Custo de ativacao',
    'Energy turret shot cost': 'Custo de disparo da torreta de energia',
    'Gunnery command': 'Comando de artilharia',
    'Improved gunnery performance': 'Desempenho de artilharia aprimorado',
    'Reload rate': 'Taxa de recarga',
    'Fire rate': 'Cadencia de tiro',
    'Armor damage received': 'Dano recebido na blindagem',
    'Shields damage received': 'Dano recebido nos escudos',
    'All damage received': 'Todo dano recebido',
    'Keen pilot': 'Piloto habil',
    'Improved ship handling': 'Manuseio da nave aprimorado',
    'Lateral accel': 'Aceleracao lateral',
    'Siege Anchor': 'Ancora de Cerco',
    'Bonus Range': 'Alcance Bonus',
    'Sensor strength': 'Potencia dos sensores',
    'Scan range': 'Alcance do escaneamento',
    'Shield specialist': 'Especialista em escudos',
    'Shield reserve': 'Reserva de escudo',
    'Shield hardness': 'Dureza do escudo',
    'Shield regen cost': 'Custo de regeneracao de escudo',
    'Shield reinforcement': 'Reforco de escudo',
    'Turret Range': 'Alcance da Torreta',
    'Turret Tracking': 'Rastreamento da Torreta',
    'Turret Damage': 'Dano da Torreta',
    'Mining Anchor': 'Ancora de Mineracao',
    'Ore yield': 'Rendimento de minerio',
    'Detection range': 'Alcance de deteccao',
    'Gravity wake': 'Rastro de gravidade',
    'Graviton web': 'Teia de gravitons',
    'Speed multiplier': 'Multiplicador de velocidade',
    'Engine recalibration': 'Recalibracao dos motores',
    'Repairing armor': 'Reparando blindagem',
    'Nanite lattice': 'Rede de nanitas',
    'Molecular Destabilization': 'Desestabilizacao Molecular',
    'Explodes randomly': 'Explode aleatoriamente',
    'Signature radius': 'Raio da assinatura',
    'Cloud acceleration': 'Aceleracao em nuvens',
    'Cloud damage': 'Dano em nuvens',
    'Ignite chance': 'Chance de incendiar',
    'Ignite range': 'Alcance de ignicao',
    'Armor damage taken': 'Dano de armadura sofrido',
    'Hull damage taken': 'Dano no casco sofrido',
    'Hull integrity': 'Integridade do casco',
    'Volatile Arcs': 'Arcos Volateis'
}

def translate_single_text(text):
    lead_len = len(text) - len(text.lstrip(' '))
    trail_len = len(text) - len(text.rstrip(' '))
    core = text.strip(' ')
    if not core:
        return text

    if core in DIRECT_TERMS:
        return (' ' * lead_len) + strip_accents(DIRECT_TERMS[core]) + (' ' * trail_len)

    for attempt in range(3):
        try:
            url = 'https://translate.googleapis.com/translate_a/single?client=gtx&sl=en&tl=pt&dt=t&q=' + urllib.parse.quote(core)
            req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0'})
            with urllib.request.urlopen(req, timeout=10) as resp:
                data = json.loads(resp.read().decode('utf-8'))
                raw_pt = ''.join([part[0] for part in data[0] if part[0]])
                cleaned = clean_term(raw_pt)
                return (' ' * lead_len) + cleaned + (' ' * trail_len)
        except Exception:
            time.sleep(0.5)

    return strip_accents(text)

def run():
    with open(TRANS_JSON, "r", encoding="utf-8-sig") as f:
        translations = json.load(f)

    with open(BLACKLIST_JSON, "r", encoding="utf-8-sig") as f:
        blacklist = set(json.load(f))

    with open(ALL_STRINGS_JSON, "r", encoding="utf-8-sig") as f:
        candidates = json.load(f)

    to_translate = []
    seen = set()

    # 1. Apply direct terms first
    for k, v in DIRECT_TERMS.items():
        if k not in translations:
            translations[k] = strip_accents(v)

    # 2. Add candidates that are safe
    for item in candidates:
        text = item['text']
        loc = item['location']
        if text in translations or text in seen:
            continue

        trimmed = text.strip()
        if trimmed in blacklist:
            continue
        if any(trimmed.endswith(ext) for ext in ['.tac', '.dbin', '.hbin', '.nbin', '.bin', '.ini', '.bdm', '.xml*', '.png', '.xnb']):
            continue
        if trimmed.startswith('database.') or trimmed.startswith('PLAYER.') or trimmed.startswith('*.') or trimmed.startswith('h:mm'):
            continue
        if re.match(r'^[A-Z][a-z]+[L|T|S]$', trimmed): # AnnaL, AnnaT
            continue

        # Check if it has letters
        if not any(c.isalpha() for c in trimmed):
            continue

        seen.add(text)
        to_translate.append(text)

    print(f"Novos termos complementares identificados: {len(to_translate)}")
    if to_translate:
        with concurrent.futures.ThreadPoolExecutor(max_workers=10) as executor:
            results = list(executor.map(translate_single_text, to_translate))

        for orig, trans in zip(to_translate, results):
            translations[orig] = trans

    # Clean accents across entire dictionary
    for k in list(translations.keys()):
        translations[k] = strip_accents(translations[k])

    with open(TRANS_JSON, "w", encoding="utf-8") as f:
        json.dump(translations, f, indent=2, ensure_ascii=False)

    print(f"Dicionario translations_exe.json atualizado com {len(translations)} termos no total.")

if __name__ == "__main__":
    run()

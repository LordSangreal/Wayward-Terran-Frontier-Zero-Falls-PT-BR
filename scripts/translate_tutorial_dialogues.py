# -*- coding: utf-8 -*-
import json
import os
import re
import time
import urllib.request
import urllib.parse
import unicodedata

BASE_DIR = r"d:\Tradução WTF Zero falls"
TRANS_JSON = os.path.join(BASE_DIR, "translations_exe.json")
MISSING_JSON = os.path.join(BASE_DIR, r"scripts\missing_tutorial_strings.json")

def strip_accents(text):
    if not text:
        return text
    nfkd = unicodedata.normalize('NFKD', text)
    return ''.join([c for c in nfkd if not unicodedata.combining(c)])

CUSTOM_FIXES = {
    "Dear diary, clone number ": "Querido diario, clone numero ",
    " activation in progress. Cognitive function results: tolerable. Combat aptitude training results: embarrassing. Complete lack of empathy test results: outstanding! Special note: this one looks even dumber than the last. Will test self esteem and listening skills before recycling.":
        " em ativacao. Resultados da funcao cognitiva: toleraveis. Resultados do treino de combate: vergonhosos. Falta total de empatia: excelente! Nota especial: este parece ainda mais burro que o anterior. Vou testar a autoestima e obediencia antes de reciclar.",
    
    "You can access the research screen by pressing the ": "Voce pode acessar a tela de pesquisa pressionando ",
    "Target the shipyard and press ": "Selecione o estaleiro como alvo e pressione ",
    " to enter its hangar.": " para entrar no hangar.",
    "You can dock with any station that has a hangar by targeting the station with ": "Voce pode acoplar em qualquer estacao com hangar selecionando-a com ",
    " and then flying over the station and hitting ": " e depois voando ate a estacao e apertando ",
    "You can get to the station faster by activating travel drive. Press ": "Voce pode chegar mais rapido a estacao ativando o motor de viagem. Pressione ",
    " to activate travel drive.": " para ativar o motor de viagem.",
    " to launch into space with your currently equipped ship.": " para lancar ao espaco com a sua nave equipada.",
    " key.": " .",
    " key": "",
    " to move": " para mover",
    " to fire primary weapons": " para disparar as armas primarias",
    " fire missiles at target": " dispara misseis no alvo",
    "Hold ": "Segure ",
    " to turn your ship towards the mouse.\nAlternately use ": " para virar sua nave em direcao ao mouse.\nAlternativamente use ",
    " to rotate the ship.": " para girar a nave.",
    "While holding ": "Enquanto segura ",
    " will strafe left and right.": " fara manobras laterais para esquerda e direita.",
    "Alright, our destination isn't far from here. Just undock from the station by pressing ": "Certo, nosso destino nao e longe daqui. Basta desacoplar da estacao pressionando ",
    " and we can be on our way.": " e podemos partir.",
    "Also, don't forget that you can hold ": "Alem disso, nao esqueca que voce pode segurar ",
    " to move the ship sideways.": " para mover a nave lateralmente.",
    "Right. Hold ": "Certo. Segure ",
    " to approach sideways, simple.": " para se aproximar de lado, simples.",
    "Most gameplay mechanics can be accessed through a room of some type. Just enter a room and press ": "A maioria das mecanicas do jogo pode ser acessada pelas salas. Basta entrar em uma sala e pressionar ",
    " to access the room's function.": " para acessar a funcao da sala.",
    "Yup, you can access your research at any time from the hotkey ": "Sim, voce pode acessar suas pesquisas a qualquer momento pela tecla de atalho ",
    " or by clicking the research progress widget in the top right. Most things you can research must first be found in the world somewhere.": " ou clicando no icone de progresso de pesquisa no canto superior direito. A maioria das coisas precisa ser encontrada pelo mundo antes de poder ser pesquisada.",
    "You can also select a heading by holding down ": "Voce tambem pode escolher uma direcao segurando ",
    " in this mode ": " neste modo ",
    " and ": " e ",
    " will accelerate the ship laterally. Don't expect much, this ship is an old heavy piece of junk. Try that now.": " vai acelerar a nave lateralmente. Nao espere muito, esta nave e uma sucata velha e pesada. Tente agora.",
    "Next I'd like you to try both the reverse thrusters ": "Agora gostaria que voce testasse os propulsores de reversao ",
    " and the main engines ": " e os motores principais ",
    " and be careful, this is the only ship we have. I don't want you flying into an asteroid or something stupid and ruining flight training.": " e tenha cuidado, esta e a unica nave que temos. Nao quero voce batendo em um asteroide ou fazendo alguma burrice e arruinando o treino.",
    "Hilarious as that sounds, I'm on a schedule and I don't have all eternity. Press ": "Por mais engracado que pareca, estou com pressa e nao tenho a eternidade. Pressione ",
    " to toggle travel drive. And do watch out for asteroids...or don't, actually yeah don't bother.": " para alternar o motor de viagem. E cuidado com os asteroides... ou nao, quer saber, nem se preocupe.",
    "There's the spirit. You should be able to find a station that will buy gold ore from you by opening your map with the ": "Esse e o espirito. Voce deve conseguir encontrar uma estacao que compre minerio de ouro abrindo o mapa com a tecla ",
    " key. There's lots of active commerce in this system.": " . Ha muito comercio ativo neste sistema.",
    "Hey heads up: This is a repair station. They sell ship repair services. I you ever want your ship repaired, target one of these stations and press ": "Fique atento: Esta e uma estacao de reparos. Eles vendem servicos de concerto de naves. Se quiser consertar sua nave, selecione uma dessas estacoes e pressione ",
    " to hail them. For a small fee they will send repair drones to patch up your ship.": " para chama-los. Por uma pequena taxa eles enviarao drones para remendar sua nave.",
    "It's no big deal, airlocks are easily repaired. In the future, try undocking first with the ": "Nao e nada demais, eclusas sao faceis de consertar. No futuro, tente desacoplar primeiro com a tecla "
}

def translate_gt(text):
    url = 'https://translate.googleapis.com/translate_a/single?client=gtx&sl=en&tl=pt&dt=t&q=' + urllib.parse.quote(text)
    req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0'})
    with urllib.request.urlopen(req, timeout=10) as resp:
        data = json.loads(resp.read().decode('utf-8'))
        return ''.join([part[0] for part in data[0] if part[0]])

def clean_term(pt_text):
    # Apply standard game terms
    t = pt_text
    t = t.replace("travel drive", "motor de viagem")
    t = t.replace("Travel Drive", "Motor de viagem")
    t = t.replace("cloning vat", "tanque de clonagem")
    t = t.replace("cloning vats", "tanques de clonagem")
    t = t.replace("matter furnace", "fornalha de materia")
    t = t.replace("airlock", "eclusa")
    t = t.replace("airlocks", "eclusas")
    t = t.replace("shipyard", "estaleiro")
    t = t.replace("hangar", "hangar")
    t = t.replace("bridge", "ponte de comando")
    return strip_accents(t)

def run():
    with open(TRANS_JSON, "r", encoding="utf-8-sig") as f:
        translations = json.load(f)

    with open(MISSING_JSON, "r", encoding="utf-8") as f:
        missing_strings = json.load(f)

    print(f"Total de falas pendentes do tutorial/inicio: {len(missing_strings)}")

    count = 0
    errors = 0
    for idx, s in enumerate(missing_strings):
        if s in translations:
            continue

        if s in CUSTOM_FIXES:
            pt = strip_accents(CUSTOM_FIXES[s])
            translations[s] = pt
            count += 1
            print(f"[{idx+1}/{len(missing_strings)}] CUSTOM: {repr(s[:35])} -> {repr(pt[:35])}")
            continue

        # Check leading and trailing whitespace
        lead_len = len(s) - len(s.lstrip(' '))
        trail_len = len(s) - len(s.rstrip(' '))
        core = s.strip(' ')

        if not core:
            translations[s] = s
            continue

        try:
            raw_pt = translate_gt(core)
            processed_pt = clean_term(raw_pt)
            final_pt = (' ' * lead_len) + processed_pt + (' ' * trail_len)
            translations[s] = final_pt
            count += 1
            if (idx + 1) % 25 == 0 or idx < 10:
                print(f"[{idx+1}/{len(missing_strings)}] {repr(core[:30])} -> {repr(final_pt[:30])}")
            time.sleep(0.08) # avoid rate limits
        except Exception as e:
            print(f"[ERRO {idx+1}] Falha ao traduzir: {repr(s[:30])} - {e}")
            errors += 1
            time.sleep(1)

    print(f"\nConcluido! {count} novas falas traduzidas adicionadas. Erros: {errors}")
    with open(TRANS_JSON, "w", encoding="utf-8") as f:
        json.dump(translations, f, indent=2, ensure_ascii=False)
    print(f"Dicionario translations_exe.json atualizado com {len(translations)} termos no total.")

if __name__ == "__main__":
    run()

# -*- coding: utf-8 -*-
import json
import os
import re
import unicodedata

BASE_DIR = r"d:\Tradução WTF Zero falls"
QUESTS_JSON = os.path.join(BASE_DIR, "batch_quests.json")
TRANS_JSON = os.path.join(BASE_DIR, "translations_exe.json")

def strip_accents(text):
    nfkd = unicodedata.normalize('NFKD', text)
    return ''.join([c for c in nfkd if not unicodedata.combining(c)])

QUEST_TRANSLATIONS = {
    "Lightning arcs from your ship periodically destroying an incoming missile":
        "Relampagos saltam da sua nave periodicamente, destruindo misseis inimigos aproximando-se.",
    "Well you are welcome to meet me in the bar of our station any time and we can talk about it more.":
        "Bem, sinta-se a vontade para me encontrar no bar da nossa estacao quando quiser para conversarmos mais.",
    "The Seven Stars Coalition has a commander who specializes in the deployment and research of drone weapons technology. Destroy his ship and we can unlock new drone weapon technology. We will also gain another piece of the data required to construct an artifact activator.":
        "A Coalizao Sete Estrelas tem um comandante especializado em armas de drones. Destrua a nave dele para desbloquearmos tecnologia de drones e mais dados para o ativador de artefatos.",
    "The Seven Stars Coalition has a commander who specializes in the deployment and research of missile weapons technology. Destroy his ship and we can unlock new missile weapon technology. We will also gain another piece of the data required to construct an artifact activator.":
        "A Coalizao Sete Estrelas tem um comandante especializado em armas de misseis. Destrua a nave dele para desbloquearmos nova tecnologia de misseis e dados para o ativador de artefatos.",
    "The Seven Stars Coalition has a commander who specializes in the deployment and research of beam weapons technology. Destroy his ship and we can unlock new beam weapon technology. We will also gain another piece of the data required to construct an artifact activator.":
        "A Coalizao Sete Estrelas tem um comandante especializado em tecnologia de armas de raio. Destrua a nave dele para desbloquearmos armas de raio e dados do ativador de artefatos.",
    "While it might be hilarious to capture a space station that is full of civilians, thus crushing everyone inside into orange goo, it has unfortunately been considered already and there are measures in place that prevent your device from working here.":
        "Embora fosse hilario capturar uma estacao cheia de civis, esmagando todos em gosma laranja, infelizmente ja pensaram nisso e ha contramedidas ativas contra seu dispositivo aqui.",
    "This jerk thinks he can ambush us and take our things. Go to his home base and blow up stations until he comes out to fight, then blow him up real good.":
        "Aquele idiota acha que pode nos emboscar e roubar nossas coisas. Va ate a base dele e exploda estacoes ate ele sair para lutar, depois acabe com a raca dele.",
    "Now who in the itchy useless piece of...dang it...you blew up my least favorite space station and I don't even know you!":
        "Mas que droga de... caramba... voce explodiu a estacao espacial que eu menos gostava e eu nem te conheco!",
    "The SSC is laying siege to pirate's cove with a number of large warships. Well what are you waiting for? Go blow them up!":
        "A SSC esta cercando a enseada dos piratas com grandes navios de guerra. O que esta esperando? Va explodi-los!",
    "Yup, I've added the coordinates for a single station to your map. I'll add more every time you find me a new cartography room.":
        "Sim, adicionei as coordenadas de uma estacao ao seu mapa. Adicionarei mais a cada nova sala de cartografia encontrada.",
    "Destroy stations and turrets in the SSC blockade around the red sector of space.":
        "Destrua estacoes e torretas no bloqueio da SSC ao redor do setor vermelho do espaco.",
    "We don't need to destroy all of them, we just need to make a hole. Even without a hole our unique relationship with mortality will allow us to slip past the defenses without too much trouble, so this can be a long term project":
        "Nao precisamos destruir todos, basta abrir uma brecha. E mesmo sem brecha, nossa relacao peculiar com a morte nos permite passar sem problemas. E um projeto de longo prazo.",
    "Rendezvous with the pirate captain who supposedly has an army and wants to join forces with you against the SSC":
        "Encontre-se com o capitao pirata que supostamente tem um exercito e quer unir forcas contra a SSC.",
    "You have pissed off the SSC. Get back to your station asap and broadcast the codes without being killed":
        "Voce enfureceu a SSC. Volte a sua estacao o mais rapido possivel e transmita os codigos sem morrer.",
    "Return to your home station while tony is a member of your crew and he will be moved to your bar.":
        "Volte para a sua estacao natal enquanto Tony fizer parte da sua tripulacao para que ele seja transferido ao seu bar.",
    "We're in luck! The station here has a functional cloning vat. Dock at the station if you want to spend some grey goo on bringing back a lost crew member.":
        "Estamos com sorte! A estacao aqui tem um tanque de clonagem funcionando. Acople se quiser gastar gosma cinza para reviver um tripulante perdido.",
    "Hey I've seen this before, this area is full of mines. Hold on, I will manually calibrate the sensors for detecting them.":
        "Ei, ja vi isso antes, esta area esta cheia de minas espaciais. Espere, vou calibrar os sensores manualmente para detecta-las.",
    "This station was overrun by monsters and the crew were all killed before they could extract the sweet sweet loot.":
        "Esta estacao foi dominada por monstros e a tripulacao foi morta antes de conseguir retirar o tao precioso saque.",
    "Scans indicate that the remaining crew made their final stand inside the tunnels of the asteroid mine. You should try and gain access to the tunnel network through the back part of the station.":
        "Escaneres indicam que a tripulacao restante resistiu nos tuneis da mina do asteroide. Tente acessar a rede de tuneis pelos fundos da estacao.",
    "Well that's why you're here. Go see what was left behind by the miners, they should have been armed.":
        "E por isso que voce esta aqui. Va ver o que os mineradores deixaram para tras, eles deviam estar armados.",
    "What? No! You're a daring space pirate who gets into gun fights and explores dungeons and collects gold and loot!":
        "O que? Nao! Voce e um audacioso pirata espacial que troca tiros, explora labirintos e recolhe ouro e espolios!",
    "Go to the pirate hideout, but make sure and bring a cargo bay full of gold wire with you.":
        "Va ate o esconderijo pirata, mas certifique-se de levar o compartimento de carga cheio de fios de ouro."
}

def run():
    print("Traduzindo pacote de missões, objetivos e eventos de história...")
    with open(QUESTS_JSON, "r", encoding="utf-8-sig") as f:
        quests = json.load(f)

    with open(TRANS_JSON, "r", encoding="utf-8-sig") as f:
        trans = json.load(f)

    added = 0
    for k, pt in QUEST_TRANSLATIONS.items():
        if k in quests and k not in trans:
            clean = strip_accents(pt)
            trans[k] = clean
            added += 1

    with open(TRANS_JSON, "w", encoding="utf-8") as f:
        json.dump(trans, f, indent=2, ensure_ascii=False)

    print(f"[OK] {added} novas missões e objetivos traduzidos adicionados ao dicionário.")
    print(f"Total acumulado: {len(trans)} termos.")

if __name__ == "__main__":
    run()

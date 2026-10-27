# -*- coding: utf-8 -*-
"""
Dicionário de Tradução PT-BR sem acentos para o executável WTF.exe.
Todos os termos são mantidos em ASCII sem acentos para garantir 100%
de compatibilidade com as fontes bitmap do jogo (MonoGame SpriteFont).
"""
import json
import os

BASE_DIR = r"d:\Tradução WTF Zero falls"
DICT_PATH = os.path.join(BASE_DIR, "translations_exe.json")

TRANSLATIONS = {
    # --- HUD e Interface em Tempo Real ---
    "Open Journal": "Abrir Diario",
    "Close Journal": "Fechar Diario",
    "Resources": "Recursos",
    "Capacitor": "Capacitor",
    "Capacitors": "Capacitores",
    "Shield": "Escudo",
    "Structure": "Estrutura",
    "Engines": "Motores",
    "Thrusters": "Propulsores",
    "Travel Drive": "Motor de Viagem",
    "travel drive": "motor de viagem",
    "Docking": "Acoplagem",
    "Dock": "Acoplar",
    "Undock": "Desacoplar",
    "to Dock": "para Acoplar",
    "Turrets": "Torretas",
    "turrets": "torretas",
    "Target": "Alvo",
    "Cargo": "Carga",
    "Inventory": "Inventario",
    "Speed": "Velocidade",
    "Living Crew": "Tripulacao Viva",
    "Living Passengers": "Passageiros Vivos",
    "Deploy station": "Implantar estacao",
    "Teleport to Station": "Teleportar para Estacao",
    "Research Lab": "Laboratorio de Pesquisa",
    "Remaining O2 in your suit's tanks.": "O2 restante nos tanques do seu traje.",
    "You die if this reaches 0": "Voce morre se chegar a 0",
    "-Press [SPACE] to continue-": "-Pressione [ESPACO] para continuar-",

    # --- Menu Principal e Opções ---
    "Campaign Core": "Campanha Principal",
    "New World": "Novo Mundo",
    "Load World": "Carregar Mundo",
    "Arena Core": "Arena Principal",
    "Resume Game": "Continuar Jogo",
    "Save Game": "Salvar Jogo",
    "Load Game": "Carregar Jogo",
    "Log out": "Desconectar",
    "New save": "Novo save",
    "Options": "Opcoes",
    "Audio": "Audio",
    "Graphics": "Graficos",
    "Controls": "Controles",
    "Back": "Voltar",
    "Apply": "Aplicar",
    "Accept": "Aceitar",
    "Cancel": "Cancelar",
    "Exit": "Sair",
    "New Character": "Novo Personagem",
    "Confirm": "Confirmar",
    "Delete": "Excluir",

    # --- Filtros do Diário e Inventário ---
    "By name": "Por nome",
    "By distance": "Por distancia",
    "By type": "Por tipo",

    # --- Status do Personagem (Necessidades) ---
    "Hunger grows over time. Reduce it by eating. If you are extremely hungry you will begin gaining fatigue at a rate that scales with your current hunger. Being very hungry also disables your immune system preventing you from naturally reducing infection.": 
        "A fome aumenta com o tempo. Reduza comendo. Se estiver com muita fome, ganhara fadiga proporcionalmente. Fome extrema tambem desativa sua imunidade.",
    
    "Thirst grows over time. Reduce it by drinking. If you are extremely thirsty you will begin experiencing symptoms of dehydration.":
        "A sede aumenta com o tempo. Reduza bebendo liquidos. Se estiver com muita sede, sofrera sintomas de desidratacao.",

    "Describes how badly you need to use a restroom. As it approaches 100% there is a growing chance that your character will decide to relieve itself in its space suit. This is unsanitary and will increase infection.":
        "Indica a urgencia de ir ao banheiro. Perto de 100%, ha risco de urinar no traje. Isso e anti-higienico e aumenta infeccoes.",

    # --- Mensagens de Combate e Alertas ---
    "A weapon mount failed to find sufficient grey gas in ship cargo to manufacture its ammo":
        "Um suporte de arma nao encontrou gas cinza suficiente na carga para fabricar municao",
    "A weapon mount failed to find sufficient grey goo in ship cargo to manufacture its ammo":
        "Um suporte de arma nao encontrou gosma cinza suficiente na carga para fabricar municao",
    "A weapon mount failed to find sufficient grey goo in ship cargo to manufacture missiles":
        "Um suporte de arma nao encontrou gosma cinza suficiente na carga para fabricar misseis",
    "-insufficient ammo": "-municao insuficiente",
    "Toggle travel drive": "Alternar motor de viagem",
    "Preparing Travel drive": "Preparando motor de viagem",
    "Travel drive charge rate is increased by 5%": "Taxa de carga do motor de viagem aumentada em 5%",

    # --- Quests e Tutoriais Iniciais ---
    "Enter hangar": "Entrar no hangar",
    "Walk to the nearby hangar and press F to launch with your ship into space": "Va ate o hangar proximo e pressione F para lancar sua nave no espaco",
    "Enter bridge": "Entrar na ponte",
    "Walk to the bridge and press F to control your ship": "Va ate a ponte e pressione F para pilotar sua nave",
    "Approach the shipyard": "Aproxime-se do estaleiro",
    "Station Upgrades": "Melhorias da Estacao",
    "Upgrade your stations ability to create bigger ships by researching some repairs to the station's existing capacitors":
        "Aprimore a estacao para fabricar naves maiores pesquisando reparos nos capacitores existentes",
    "Explore a bit": "Explorar um pouco",
    "There is an abandoned asteroid mine near your home base. If it was abandoned in a hurry then it should have some stuff worth scavenging.":
        "Ha uma mina de asteroides abandonada perto da base. Se foi abandonada as pressas, deve ter itens valiosos.",
    "Get some gear": "Obter equipamento",
    "The abandoned mine is still infested. Fight your way to the end and loot the cargo bays where the station's late crew made their final stand.":
        "A mina abandonada continua infestada. Abra caminho em combate e vasculhe os compartimentos de carga.",
    "Oh, that's cool. I guess you can also just go and do whatever you like.":
        "Beleza, bacana. Acho que voce tambem pode simplesmente ir e fazer o que quiser.",

    # --- Diálogos Iniciais da Anna ---
    "What do you want?": "O que voce quer?",
    "I see you're already strutting around like you own the place.": "Ja vi que esta se achando o dono do pedaco por aqui.",
    "Can I help you leave sooner?": "Posso ajudar voce a cair fora mais cedo?",
    "You better be staying out of trouble.": "E bom ficar longe de encrenca.",
    "Here to waste my time?": "Veio aqui desperdicar meu tempo?",
    "You haven't touched anything have you?": "Voce nao encostou em nada, ne?",
    "Pretty nice little fort you've got here.": "Forte bem ajeitadinho que voce tem aqui.",
    "I'm watching you. Don't try anything stupid while you're here.": "Estou de olho em voce. Nao tente nenhuma besteira por aqui.",
    "So where do you get some coffee around here?": "Onde se consegue um cafe por aqui?",
    "I'm up to any challenge, no matter how unreasonable.": "Topo qualquer desafio, nao importa o absurdo.",
    "Did you try the bartender?": "Ja tentou o barman?",
    "Is that a joke?": "Isso e piada?",
    "Oh alright, I'll try that.": "Ah certo, vou tentar isso.",
    "I thought you were going to be leaving sooner. Is there some way I can help you finish your business here?":
        "Achei que voce ia embora mais cedo. Posso ajudar voce a encerrar seus assuntos por aqui?",
    "Hey, so I've got some questions.": "Ola, tenho algumas perguntas.",
    "Nope, just passing through.": "Nao, so de passagem.",
    "Great. Unfortunately I'm far too busy to answer questions from people I don't like.":
        "Otimo. Pena que estou ocupada demais para tirar duvidas de quem nao gosto.",
    "Too busy? You're sitting here drinking...is that rum?":
        "Ocupada demais? Voce esta sentada bebendo... isso e rum?",
    "Oh...well nevermind then.": "Ah... bem, deixa pra la entao.",
    "Yes that sounds great": "Sim, parece otimo",
    "Actually I want answers but...": "Na verdade eu queria respostas, mas...",
    "Not really...": "Na verdade nao...",
    "That sounds easy enough.": "Parece facil o bastante.",
    "Shut up. How about you go and do your fancy crossing the blockade thing and bring my people some food.":
        "Cala a boca. Que tal voce usar essa sua habilidade de furar o bloqueio e trazer comida para o meu povo?",
    "You shut up!...also that sounds easy enough.":
        "Cala a boca voce!... mas isso parece facil o suficiente.",
    "Great, so we agree. You go away, and if we meet again you'll have something better for me to drink than this toilet wine, and then we'll be great buddies. Sound good?":
        "Otimo, combinado. Voce cai fora e, se nos vermos de novo, traga algo melhor para beber do que esse gororoba. Ai seremos grandes amigos. Fechado?",
    "I mean, typically a job like this pays more than friendship.":
        "Digo, normalmente um trabalho desses paga mais do que amizade.",
    "Fine, uuh...oh I know, you can have friendship. You bring us food, and friendship is yours to take.":
        "Tudo bem, hmmm... ja sei: voce ganha amizade. Voce traz a comida e a amizade e sua."
}

def build():
    with open(DICT_PATH, "w", encoding="utf-8") as f:
        json.dump(TRANSLATIONS, f, indent=2, ensure_ascii=False)
    print(f"[OK] Dicionario salvo em {DICT_PATH} ({len(TRANSLATIONS)} traducoes catalogadas)")

if __name__ == "__main__":
    build()

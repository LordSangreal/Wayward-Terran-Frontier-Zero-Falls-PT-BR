# -*- coding: utf-8 -*-
import json
import os
import unicodedata

BASE_DIR = r"d:\Tradução WTF Zero falls"
AGENTS_JSON = os.path.join(BASE_DIR, "batch_agents.json")
TRANS_JSON = os.path.join(BASE_DIR, "translations_exe.json")

def strip_accents(text):
    nfkd = unicodedata.normalize('NFKD', text)
    return ''.join([c for c in nfkd if not unicodedata.combining(c)])

AGENT_DIALOGUES = {
    "Did you try the bartender?": "Ja tentou falar com o barman?",
    "Oh alright, I'll try that.": "Ah certo, vou tentar isso.",
    "I thought you were going to be leaving sooner. Is there some way I can help you finish your business here?":
        "Achei que voce fosse embora mais cedo. Posso ajudar voce a encerrar seus assuntos por aqui?",
    "Hey, so I've got some questions.": "Ola, tenho algumas perguntas.",
    "Nope, just passing through.": "Nao, so de passagem.",
    "Great. Unfortunately I'm far too busy to answer questions from people I don't like.":
        "Otimo. Pena que estou ocupada demais para tirar duvidas de quem nao gosto.",
    "Too busy? You're sitting here drinking...is that rum?":
        "Ocupada demais? Voce esta sentada bebendo... isso e rum?",
    "Oh...well nevermind then.": "Ah... bem, deixa pra la entao.",
    "It slightly resembles rum and yes, as you can clearly see the sky is dark, so it must be evening, which means unless you are here to deliver more rum, you are interfering with the important night shift business that I intend to be conducting all night long.":
        "Lembra vagamente rum e sim, como voce pode ver o ceu esta escuro, logo e noite, o que significa que, a menos que traga mais rum, voce esta atrapalhando meus negocios noturnos que pretendo conduzir a noite toda.",
    "I could deliver rum...erm or snacks...or whatever...you remember I can cross the blockade right?":
        "Eu poderia trazer rum... ou lanches... sei la... voce lembra que posso cruzar o bloqueio, ne?",
    "...Okay weirdo. You wanna be friends and share stories and hang out and stuff?":
        "...Ta bom, seu esquisito. Quer ser amigo, trocar historias e bater papo?",
    "Yes that sounds great": "Sim, parece otimo",
    "Actually I want answers but...": "Na verdade eu queria respostas, mas...",
    "Shut up. How about you go and do your fancy crossing the blockade thing and bring my people some food.":
        "Cala a boca. Que tal voce usar essa sua habilidade de furar o bloqueio e trazer comida para o meu povo?",
    "That sounds easy enough.": "Parece facil o bastante.",
    "You shut up!...also that sounds easy enough.":
        "Cala a boca voce!... mas isso parece facil o suficiente.",
    "Great, so we agree. You go away, and if we meet again you'll have something better for me to drink than this toilet wine, and then we'll be great buddies. Sound good?":
        "Otimo, combinado. Voce cai fora e, se nos vermos de novo, traga algo melhor para beber do que esse gororoba. Ai seremos grandes amigos. Fechado?",
    "I mean, typically a job like this pays more than friendship.":
        "Digo, normalmente um trabalho desses paga mais do que amizade.",
    "Fine, uuh...oh I know, you can have friendship. You bring us food, and friendship is yours to take.":
        "Tudo bem, hmmm... ja sei: voce ganha amizade. Voce traz a comida e a amizade e sua.",
    "I mean, wait are we in a loop here?": "Espera, estamos presos num loop aqui?",
    "Shut up. Uhh, I mean no...friendship is the name we gave to the new warship we've been gluing together. I guess I see why that would be confusing. You like warships?":
        "Cala a boca. Digo, nao... 'Amizade' e o nome do novo navio de guerra que estamos colando. Entendo a confusao. Voce gosta de navios de guerra?",
    "I pretty much live in them!": "Eu praticamente moro neles!",
    "Oh you'll love this one then. It's got missiles and like...lasers or something I don't know look do you want the job?":
        "Ah, entao voce vai adorar essa nave. Tem misseis e tipo... lasers ou coisa assim, sei la, olha aqui: voce quer o trabalho ou nao?",
    "Sure, I'll do it.": "Claro, eu faco.",
    "Sure, but why'd you name it friendship?": "Claro, mas por que deram o nome de 'Amizade'?",
    "Because when you blow someone up with it, they get to experience the magic of friendship. Now go get the food.":
        "Porque quando voce explode alguem com ela, a pessoa experimenta a magia da amizade. Agora va buscar a comida.",
    "I can do that.": "Posso fazer isso.",
    "Consider it done.": "Considere feito.",
    "Hey, about that food...": "Ola, sobre aquela comida...",
    "Do you have the food yet?": "Ja conseguiu a comida?",
    "Not yet, still working on it.": "Ainda nao, estou trabalhando nisso.",
    "Yep, got it right here.": "Sim, esta bem aqui.",
    "Awesome. Here is your reward.": "Demais. Aqui esta sua recompensa.",
    "Thanks.": "Obrigado.",
    "What else do you need?": "O que mais voce precisa?",
    "Need any more work done?": "Precisa de mais algum servico?",
    "I'm looking for a job.": "Estou procurando trabalho.",
    "Who are you?": "Quem e voce?",
    "What is this place?": "Que lugar e este?",
    "Tell me about the blockade.": "Conte-me sobre o bloqueio.",
    "Tell me about the SSC.": "Conte-me sobre a SSC.",
    "Tell me about the pirates.": "Conte-me sobre os piratas.",
    "Goodbye.": "Ate logo.",
    "See you around.": "A gente se ve por ai.",
    "Later.": "Ate mais."
}

def run():
    print("Traduzindo pacote de dialogos de NPCs...")
    with open(AGENTS_JSON, "r", encoding="utf-8-sig") as f:
        agents = json.load(f)

    with open(TRANS_JSON, "r", encoding="utf-8-sig") as f:
        trans = json.load(f)

    added = 0
    for k, v in AGENT_DIALOGUES.items():
        if k in agents and k not in trans:
            trans[k] = strip_accents(v)
            added += 1

    with open(TRANS_JSON, "w", encoding="utf-8") as f:
        json.dump(trans, f, indent=2, ensure_ascii=False)

    print(f"[OK] {added} novas falas de NPCs adicionadas ao dicionario.")
    print(f"Total acumulado: {len(trans)} termos.")

if __name__ == "__main__":
    run()

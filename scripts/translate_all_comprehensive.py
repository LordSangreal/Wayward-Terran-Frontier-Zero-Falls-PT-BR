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
UNIFIED_JSON = os.path.join(BASE_DIR, r"scripts\unified_to_translate.json")
CACHE_JSON = os.path.join(BASE_DIR, r"scripts\translation_cache.json")
TRANS_JSON = os.path.join(BASE_DIR, "translations_exe.json")
PATCHER_JSON = os.path.join(BASE_DIR, r"patcher\translations_exe.json")

def strip_accents(text):
    if not text:
        return text
    nfkd = unicodedata.normalize('NFKD', text)
    return ''.join([c for c in nfkd if not unicodedata.combining(c)])

# Dicionario completo de termos manuais, itens, interacoes e frases-chave
MANUAL_TERMS = {
    # Interacoes e avisos de canto inferior esquerdo (HUD/Chat)
    "You were specifically designed not to sleep": "Voce foi projetado especificamente para nao dormir",
    "Activate airlock": "Ativar eclusa",
    "Door is locked open": "Porta travada aberta",
    "Use navigation console": "Usar console de navegacao",
    "Use gunnery console": "Usar console de artilharia",
    "This console is connected to nothing": "Este console nao esta conectado a nada",
    "Needs repair": "Necessita de reparos",
    "SELF DESTRUCT:": "AUTODESTRUICAO:",
    "Nap": "Soneca",
    "Craft goo": "Fabricar gosma",
    "Craft Gun": "Fabricar arma",
    "to Undock": "para Desacoplar",
    "para Acoplar": "para Acoplar",
    "Press": "Pressione",
    "Station has no hangar. Use an airlock": "A estacao nao possui hangar. Use uma eclusa",
    "There was no room in your inventory so your ship was packed and placed into safe storage which you can access from your home base.": "Nao havia espaco em seu inventario, entao sua nave foi guardada no armazenamento seguro acessivel em sua base.",
    "The store doesn't buy that item type": "A loja nao compra este tipo de item",
    "Deposit refunded for mission cargo that could not be loaded": "Deposito reembolsado para carga de missao que nao pode ser carregada",
    "a mission's cargo could not be loaded due to lack of space": "a carga de uma missao nao pode ser carregada por falta de espaco",
    "Your mission cargo would not fit into ship storage so it remains waiting for you in the hangar when you depart": "A carga da sua missao nao cabia no armazenamento da nave, portanto permanecera esperando por voce no hangar quando voce partir",
    "You departed in a ship that didn't have sufficient free space for existing cargo": "Voce partiu em uma nave que nao tinha espaco livre suficiente para a carga existente",
    "You departed in a ship that didn't have a bedroom for every passenger": "Voce partiu em uma nave que nao tinha um quarto para cada passageiro",
    "There were insufficient beds so some of your passengers remain waiting for you in the hangar when you depart": "Nao havia leitos suficientes, portanto alguns de seus passageiros continuam esperando por voce no hangar quando voce partir",
    "One of your passengers received a free room upgrade because there weren't enough beds at their ticket price": "Um dos seus passageiros recebeu uma melhoria gratuita de quarto porque nao havia leitos suficientes pelo valor da passagem",
    "A fire has started on your ship.": "Um incendio comecou na sua nave.",
    "Warning: A reactor has limited its output to prevent overheating": "Aviso: Um reator limitou sua potencia para evitar superaquecimento",
    "A missile factory failed to find sufficient grey goo in ship cargo to manufacture missiles": "Uma fabrica de misseis nao encontrou gosma cinza suficiente na carga da nave para fabricar misseis",
    "You already have that research in your queue": "Voce ja possui esta pesquisa em sua fila",
    "Preparing Higgs drive": "Preparando motor Higgs",
    "Console automation": "Automacao de consoles",
    "Click to toggle AI control of your gunnery consoles. Does nothing if the console is occupied": "Clique para alternar o controle por IA dos consoles de artilharia. Nao faz nada se o console estiver ocupado",
    "to open Map and pick destination": "para abrir o Mapa e escolher destino",
    "to target": "para mirar",
    "Access cargo": "Acessar carga",
    "Access Matter Furnace": "Acessar Fornalha de Materia",
    "Access Logistics": "Acessar Logistica",
    "Access Research": "Acessar Pesquisa",
    "Trade with shop": "Comerciar com a loja",
    "Trade with station": "Comerciar com a estacao",
    "Out of order": "Fora de servico",
    "Underlay can": "Janela de fundo",
    "Confirm?": "Confirmar?",
    "What is this for?": "Para que serve isto?",
    "Confirm": "Confirmar",
    "Cancel": "Cancelar",
    "Transfer": "Transferir",
    "Buy": "Comprar",
    "Sell": "Vender",
    "New inventory:": "Novo inventario:",
    "Quantity": "Quantidade",
    "Price $": "Preco $",
    "Scrap:": "Sucata:",
    "Credits:": "Creditos:",
    "Price:": "Preco:",
    "Save game": "Salvar jogo",
    "Save + exit": "Salvar e sair",
    "Spawn ship": "Gerar nave",
    "Edit design": "Editar projeto",
    "Clone design": "Clonar projeto",
    "Unload cargo": "Descarregar carga",
    "Scrap": "Desmanchar",
    "Repair": "Reparar",
    "Restock Ammo": "Reabastecer Municao",
    "Tractor to dock": "Rebocar para a doca",
    "Resources deposited": "Recursos depositados",
    "ship added to inventory": "nave adicionada ao inventario",
    "No space in inventory": "Sem espaco no inventario",
    "enter a unique name for your design": "insira um nome exclusivo para seu projeto",
    "You can only scrap packed ships": "Voce so pode desmanchar naves embaladas",
    "Scrap and research": "Desmanchar e pesquisar",
    "Pack ship": "Embalar nave",
    "Provision": "Abastecer",
    "Tip: Capture ships and reverse engineer them to unlock new designs": "Dica: Capture naves e faca engenharia reversa para desbloquear novos projetos",
    "Ship Navigation": "Navegacao da Nave",
    "A bug seems to have caused your home station to stop existing so you are being dumped into empty space. Sorry for the inconvenience.": "Um erro fez sua estacao inicial deixar de existir, entao voce esta no espaco vazio. Desculpe pelo transtorno.",
    "Unable to abandon quest: ": "Impossivel abandonar missao: ",
    " no clean abort code available. i.e. we didn't code a way for you to skip that quest and dropping it would break the story progression.": " nenhum codigo de cancelamento disponivel. Ou seja, nao ha como pular esta missao sem quebrar a progressao da historia.",
    " could not be loaded": " nao pode ser carregado",
    " out of ": " de ",
    "Budd ship has arrived!": "A nave Budd chegou!",

    # Itens e materiais
    "Dirt": "Terra",
    "Lead Ore": "Minerio de Chumbo",
    "Silicon": "Silicio",
    "Steel ingots": "Barras de Aco",
    "Insulation": "Isolamento",
    "Gears": "Engrenagens",
    "Valve": "Valvula",
    "Magnet": "Ima",
    "Microcontroller": "Microcontrolador",
    "Bracket": "Suporte",
    "Pipe": "Tubo",
    "Bolts": "Parafusos",
    "Biomass": "Biomassa",
    "Snacks": "Lanches",
    "Assorted parts": "Pecas sortidas",
    "Pecas sortidas": "Pecas sortidas",
    "Troilite Ore": "Minerio de Troilita",
    "Troilite": "Troilita",
    "Minerio de Troilita": "Minerio de Troilita",
    "Gold Ore": "Minerio de Ouro",
    "Iron Ore": "Minerio de Ferro",
    "Titanium Ore": "Minerio de Titanio",
    "Rhodium Ore": "Minerio de Rodio",
    "Ithacit Ore": "Minerio de Itacita",
    "Itthacit Ore": "Minerio de Itacita",
    "Mitraxit Ore": "Minerio de Mitraxita",
    "Iron Ingot": "Barra de Ferro",
    "Barra de Ferro": "Barra de Ferro",
    "Gold Wire": "Fio de Ouro",
    "Titanium Ingot": "Barra de Titanio",
    "Rhodium Ingot": "Barra de Rodio",
    "Ithacit Ingot": "Barra de Itacita",
    "Mitraxit Ingot": "Barra de Mitraxita",
    "Carbon Nanotubes": "Nanotubos de Carbono",
    "Graphene sheets": "Folhas de Grafeno",
    "Steel springs": "Molas de Aco",
    "Electronic Components": "Componentes Eletronicos",
    "Core node": "No Central",
    "Neutralino shard": "Fragmento de Neutralino",
    "Crystal spore": "Esporo de Cristal",
    "Crystal Shard": "Fragmento de Cristal",
    "Exotic matter": "Materia Exotica",
    "Dense Exotic matter": "Materia Exotica Densa",
    "Subspace tube": "Tubo Subespacial",
    "Remote brain transmitter": "Transmissor Cerebral Remoto",
    "Serenity unit": "Unidade de Serenidade",
    "Really green fuse": "Fusivel Muito Verde",
    "Ablative Armor": "Blindagem Ablativa",
    "Armor Plate": "Placa de Blindagem",
    "Deflective Plate": "Placa Defletora",
    "Collision Avoider": "Evitador de Colisoes",
    "Shield Capacitor": "Capacitor de Escudo",
    "Shield Coil": "Bobina de Escudo",
    "Shield Hardener": "Endurecedor de Escudo",
    "PD Extender": "Extensor de Defesa de Ponto",
    "Regen Enhancer": "Aprimorador de Regeneracao",
    "Regen Extender": "Extensor de Regeneracao",
    "Bullet Range Chip": "Chip de Alcance de Projeteis",
    "Bullet Damage Chip": "Chip de Dano de Projeteis",
    "Bullet Velocity Chip": "Chip de Velocidade de Projeteis",
    "Turret Accuracy Chip": "Chip de Precisao de Torreta",
    "Turret Fire Rate Chip": "Chip de Cadencia de Torreta",
    "Turret Traverse Chip": "Chip de Giro de Torreta",
    "Double Machine Gun": "Metralhadora Dupla",
    "Fixed Machine Gun": "Metralhadora Fixa",
    "Handgun": "Pistola",
    "Large Barrel": "Cano Grande",
    "Medium Barrel": "Cano Medio",
    "Small Barrel": "Cano Pequeno",
    "Tiny Barrel": "Cano Minusculo",
    "Rifle barrel": "Cano de Fuzil",
    "Large Hail Cannon": "Canhao de Granizo Grande",
    "Small Hail Cannon": "Canhao de Granizo Pequeno",
    "Tiny Hail cannon": "Canhao de Granizo Minusculo",
    "Medium flak": "Flak Medio",
    "Small flak": "Flak Pequeno",
    "Medium rail gun": "Canhao Eletromagnetico Medio",
    "Small rail gun": "Canhao Eletromagnetico Pequeno",
    "Small rail array": "Matriz Eletromagnetica Pequena",
    "Small rail repeater": "Repetidor Eletromagnetico Pequeno",
    "Tiny rail repeater": "Repetidor Eletromagnetico Minusculo",
    "Titanium Housing": "Carcaca de Titanio",
    "Triple Machine Gun": "Metralhadora Tripla",
    "Tiny Machine Gun": "Metralhadora Minuscula",
    "Medium bearing": "Rolamento Medio",
    "Small bearing": "Rolamento Pequeno",
    "Tiny bearing": "Rolamento Minusculo",
    "Medium Rail": "Trilho Medio",
    "Small Rail": "Trilho Pequeno",
    "130mm Mass Driver": "Propulsor de Massa de 130mm",
    "150mm Auto Cannon": "Canhao Automatico de 150mm",
    "150mm repeater": "Repetidor de 150mm",
    "220mm Auto Cannon": "Canhao Automatico de 220mm",
    "380mm Auto Cannon": "Canhao Automatico de 380mm",
    "64mm Mass Driver": "Propulsor de Massa de 64mm",
    "95mm Mass Driver": "Propulsor de Massa de 95mm",

    # Descricoes de itens
    "This is dirt, also known as stuff you dug up that isn't very valuable. However matter is matter, and matter is also energy, so why not throw it into the furnace and move on with your life?": "Isto e terra, tambem conhecida como coisas que voce desenterrou e que nao tem muito valor. Porem materia e materia, e materia tambem e energia, entao por que nao joga-la na fornalha e seguir em frente com sua vida?",
    "Lead ore is boring, but heavy. Heavy matter is good matter. It also converts easily into gold which is useful in ship construction.": "O minerio de chumbo e sem graca, mas pesado. Materia pesada e boa materia. Tambem se converte facilmente em ouro, util na construcao de naves.",
    "Troilite is a rare iron sulfide mineral which can be smelted to yield iron ingots.": "A troilita e um mineral raro de sulfeto de ferro que pode ser fundido para produzir barras de ferro.",
    "Ore rich in itthacit deposits.": "Minerio rico em depositos de itacita.",
    "These are assorted parts and metal things meant to replace failing components in a space station as well as for use constructing new stuff.": "Estas sao pecas variadas e itens de metal destinados a substituir componentes com defeito em uma estacao espacial, bem como para a construcao de coisas novas.",
    "Delicious Human Chow. Made for Humans, not from Humans.": "Deliciosa Comida Humana. Feita para Humanos, nao de Humanos.",
    "This is a big pile of flesh that may or may not contain parts of clothing. For some reason this increases your station's capacity to produce space ships. The hope when writing this was that you would never read it so we wouldn't need to explain further.": "Esta e uma grande pilha de carne que pode ou nao conter pedacos de roupa. Por alguma razao, isso aumenta a capacidade da sua estacao de produzir naves espaciais. A esperanca ao escrever isso era que voce nunca lesse, para nao precisarmos explicar mais.",
    "It was once believed that these rocks were needed in order to grow crystals. However in modern crystal farms crystals are found to easily grow on dirt leading many to believe that we exist in a strange alternate universe where neutralino shards have never had any value at all.": "Acreditava-se que essas rochas eram necessarias para cultivar cristais. No entanto, em fazendas modernas, os cristais crescem facilmente na terra, levando muitos a crer que existimos em um universo alternativo onde fragmentos de neutralino nunca tiveram valor algum.",
    "An inert shard of a once living crystaline life form. With some fancy genetic engineering it could be brought back to life. Use this on some dirt to plant a crystal with your currently configured genome.": "Um fragmento inerte de uma forma de vida cristalina que ja foi viva. Com engenharia genetica avancada, poderia ser revivida. Use isto em um pouco de terra para plantar um cristal com o genoma atualmente configurado.",
    "A shard of crystal matter. This currency may be relevant in the future when the lazy good for nothing developers add that content to the game.": "Um fragmento de materia cristalina. Esta moeda podera ser relevante no futuro quando os desenvolvedores preguicosos adicionarem esse conteudo ao jogo.",
    "This energy dense byproduct of reactor field collapse is so valuable that most mainstream manufacturers subsidize insurance payouts with the stuff. On purchasing a reactor, users agree to return the matter apon detonation of their drive. In practice however, both returns and claims are rare on account of fraud and the user-death invalidation clause.": "Este subproduto de colapso de campo de reator e tao valioso que os fabricantes subsidiam pagamentos de seguro com ele. Ao comprar um reator, os usuarios concordam em devolver a materia caso o motor exploda. Na pratica, porém, devolucoes e reivindicacoes sao raras devido a fraudes e a clausula de morte do usuario.",
    "Press right click to use.": "Pressione o botao direito do mouse para usar.",
    "A currency which allows unlocking module research.": "Uma moeda que permite desbloquear pesquisas de modulos.",

    # Profissoes e Oficios
    "Weaponsmithing": "Armaria",
    "Artificing": "Artificie",
    "Engineering": "Engenharia",
    "Smelting": "Fundicao",
    "Salvaging": "Desmanche",
    "Archeology": "Arqueologia",

    # Textos do banco crafts
    "Crafts a turret.": "Fabrica uma torreta.",
    "Crafts a reverse engineered artifact": "Fabrica um artefato de engenharia reversa.",
    "Crafts a crew weapon with randomized stats.": "Fabrica uma arma de tripulacao com atributos aleatorios.",
    "Crafts a side arm suitable for personal defense.": "Fabrica uma arma secundaria adequada para defesa pessoal.",
    "Crafts a rifled barrel suitable for construction of small arms.": "Fabrica um cano estriado adequado para a construcao de armas leves.",
    "Crafts a machine gun for use with small size weapon mounts on a ship.": "Fabrica uma metralhadora para uso em suportes de armas de tamanho pequeno em uma nave.",
    "Crafts a tiny fixed machine gun for use with fixed weapon mounts on a ship.": "Fabrica uma metralhadora fixa minuscula para suportes fixos de armas em uma nave.",
    "Crafts a tiny machine gun for use with weapon mounts on a ship.": "Fabrica uma metralhadora minuscula para suportes de armas em uma nave.",
    "Crafts a titanium weapon housing.": "Fabrica uma carcaca de arma feita de titanio.",
    "Crafts a Large barrel, a component used in crafting.": "Fabrica um cano grande, componente usado na fabricacao.",
    "Crafts a Medium barrel, a component used in crafting.": "Fabrica um cano medio, componente usado na fabricacao.",
    "Crafts a small barrel, a component used in crafting.": "Fabrica um cano pequeno, componente usado na fabricacao.",
    "Crafts a tiny barrel, a component used in crafting.": "Fabrica um cano minusculo, componente usado na fabricacao.",
    "Crafts a Medium rail, a component used in crafting.": "Fabrica um trilho medio, componente usado na fabricacao.",
    "Crafts a small rail, a component used in crafting.": "Fabrica um trilho pequeno, componente usado na fabricacao.",
    "Crafts a medium bearing, a component used in crafting.": "Fabrica um rolamento medio, componente usado na fabricacao.",
    "Crafts a small bearing, a component used in crafting.": "Fabrica um rolamento pequeno, componente usado na fabricacao.",
    "Crafts a tiny bearing, a component used in crafting.": "Fabrica um rolamento minusculo, componente usado na fabricacao.",
    "Crafts a handful of steel springs. A low level Tinkering component.": "Fabrica molas de aco. Um componente basico de ajuste.",
    "Crafts some basic circuit boards and micro controllers. A mid level Tinkering component.": "Fabrica placas de circuito basicas e microcontroladores. Um componente intermediario de ajuste.",
    "Used for manipulation of high energy plasma and electromagnetic field interactions. A high level Tinkering component.": "Usado para manipulacao de plasma de alta energia e interacoes de campos eletromagneticos. Um componente avancado de ajuste.",
    "Produces a component which can be used to upgrade a module": "Produz um componente que pode ser usado para aprimorar um modulo.",
    "Produces silicone wafers.": "Produz pastilhas de silicio.",
    "A crafting material used by many professions.": "Um material de fabricacao usado por varias profissoes.",
    "A crafting material used by artificers.": "Um material de fabricacao usado por artifices.",
    "Allow grey goo to consume refined resources to construct more bulk. Produces grey goo.": "Permite que a gosma cinza consuma recursos refinados para construir mais massa. Produz gosma cinza.",
    "Allow grey goo to consume unrefined ores to construct more bulk. Produces grey goo.": "Permite que a gosma cinza consuma minerios brutos para construir mais massa. Produz gosma cinza.",
    "Compress space dust into a random ore. It can produce iron, gold, titanium, or rhodium": "Comprime poeira espacial em um minerio aleatorio. Pode produzir ferro, ouro, titanio ou rodio.",
    "Smelt gold ore, removing impurities and forming a spool of gold wire.": "Funde minerio de ouro, removendo impurezas e formando um carretel de fio de ouro.",
    "Smelt iron ore, removing impurities and forming an ingot.": "Funde minerio de ferro, removendo impurezas e formando uma barra.",
    "Smelt ithacit ore, removing impurities and forming an ingot.": "Funde minerio de itacita, removendo impurezas e formando uma barra.",
    "Smelt mitraxit ore, removing impurities and forming an ingot.": "Funde minerio de mitraxita, removendo impurezas e formando uma barra.",
    "Smelt rhodium ore, removing impurities and extracting the valuable platinum, forming an ingot.": "Funde minerio de rodio, removendo impurezas e extraindo platina valiosa, formando uma barra.",
    "Smelt titanium ore, removing impurities and forming an ingot.": "Funde minerio de titanio, removendo impurezas e formando uma barra.",
    "Break down a pile of electrical scrap into its various components to see if you can find anthing useful.": "Desmonte uma pilha de sucata eletrica em varios componentes para ver se encontra algo util.",
    "Break down a pile of mechanical scrap into its various components to see if you can find anthing useful.": "Desmonte uma pilha de sucata mecanica em varios componentes para ver se encontra algo util.",
    "Break down a pile of polymer scrap into its various components to see if you can find anthing useful.": "Desmonte uma pilha de sucata de polimero em varios componentes para ver se encontra algo util.",
    "Break down a pile of structural scrap into its various components to see if you can find anthing useful.": "Desmonte uma pilha de sucata estrutural em varios componentes para ver se encontra algo util.",

    # Textos de quests
    "A generic quest to reach a destination": "Uma missao geral para alcancar um destino.",
    "Decide if you will accept an application": "Decida se voce aceitara uma solicitacao.",
    "Request to join the colony": "Solicitar entrada na colonia.",
    "Leave your current colony": "Deixar sua colonia atual.",
    "Grants +10 to max trade skill level in exchange for credits": "Concede +10 ao nivel maximo de comercio em troca de creditos.",
    "Determines which spawning facility will be used to respawn if you die": "Determina qual instalacao sera usada para renascer se voce morrer.",
    "Shows the location of a warlord to be defeated for above average loot.": "Mostra o local de um senhor da guerra a derrotar para saques acima da media.",
    "Rewards credits on receipt of digital proof of target's death": "Recompensa creditos mediante recebimento de prova digital da morte do alvo.",
    "Approach the location and report back any findings.": "Aproxime-se do local e relate todas as descobertas.",
    "Deliver passengers": "Transportar passageiros",
    "Deliver cargo": "Entregar carga",
    "Criminal bounty": "Recompensa de criminoso",
    "Investigate": "Investigar"
}

# Regras e regex para padrões repetitivos
REGEX_RULES = [
    # Produces a tuning kit: ...
    (r"^Produces a tuning kit:\s*(.+)$", r"Produz um kit de ajuste: \1"),
    (r"Forward acceleration is increased by (\d+)%", r"Aceleracao frontal aumentada em \1%"),
    (r"Lateral acceleration is increased by (\d+)%", r"Aceleracao lateral aumentada em \1%"),
    (r"Rotational acceleration is increased by (\d+)%", r"Aceleracao de rotacao aumentada em \1%"),
    (r"Turret rate of fire is increased by (\d+)%", r"Cadencia de tiro das torretas aumentada em \1%"),
    (r"Turret magazines hold (\d+)% more ammo", r"Carregadores de torreta comportam \1% mais municao"),
    (r"Turret reload rate is increased by (\d+)%", r"Velocidade de recarga das torretas aumentada em \1%"),
    (r"Turret traverse speed is increased by (\d+)%", r"Velocidade de giro das torretas aumentada em \1%"),
    (r"Damage with projectile weapons is increased by (\d+)%", r"Dano com armas balisticas aumentado em \1%"),
    (r"Bullet duration is increased by (\d+)%", r"Duracao do projetil aumentada em \1%"),
    (r"Sensor strength is increased by (\d+)%", r"Potencia dos sensores aumentada em \1%"),
    (r"Chance to glance incoming damage is increased by (\d+)%", r"Chance de desviar dano recebido aumentada em \1%"),
    (r"Reduces the amount of shield energy needed to create 1 pixel of shield by (\d+)%", r"Reduz em \1% a energia para criar 1 pixel de escudo"),
    (r"Shield generators store (\d+)% more shield energy", r"Geradores de escudo armazenam \1% mais energia"),
    (r"Travel drive charge rate is increased by (\d+)%", r"Taxa de carga do motor de viagem aumentada em \1%"),
    (r"\+(\d+) armor hardness", r"+\1 de dureza da blindagem"),
    (r"\+(\d+) glancing hardness", r"+\1 de dureza de raspao"),
    (r"\+(\d+) shield hardness", r"+\1 de dureza do escudo"),
    (r"\+(\d+)/s shield transfer", r"+\1/s de transferencia de escudo"),
    (r"\+([\d\.]+)s shield regen support buff duration", r"+\1s na duracao do buff de regeneracao de escudo"),
    (r"^A tuning kit can be used while inside a ship to provide a small perminant bonus to that ship's performance attributes\. Using multiple kits of the same type will provide a stacking additive bonus up to a certain limit\.\n\nThis kit provides the following bonus:\s*",
     "Um kit de ajuste concede um bonus permanente a nave. Kits do mesmo tipo acumulam o bonus ate certo limite.\n\nEste kit concede o seguinte bonus: "),
    
    # Schematics
    (r"^Design schematics for a (small|medium|large|tiny) (.+?)\.?$", r"Esquema de projeto: \2 (\1)"),
    (r"^Design schematics for (.+?)\.?$", r"Esquema de projeto: \1"),
    (r"^Unlock crafting of (.+?) in the inventory.*$", r"Desbloqueia a fabricacao de \1 no inventario."),
    (r"^(.+?) crafting$", r"Fabricacao de \1"),

    # Common descriptions
    (r"^Stores up to (\d+) energy in an internal capacitor bank\.", r"Armazena ate \1 de energia em um banco interno de capacitores."),
    (r"^Generates (\d+) units of shield per second\.", r"Gera \1 unidades de escudo por segundo."),
    (r"^Consumes (\d+) energy per second while active\.", r"Consome \1 de energia por segundo enquanto ativo."),
    (r"^Produces (\d+) energy per second\.", r"Produz \1 de energia por segundo.")
]

def clean_term(pt_text):
    if not pt_text:
        return pt_text
    t = pt_text
    # Substituicoes contextuais
    t = re.sub(r'\btravel drive\b', 'motor de viagem', t, flags=re.IGNORECASE)
    t = re.sub(r'\bcloning vat\b', 'tanque de clonagem', t, flags=re.IGNORECASE)
    t = re.sub(r'\bcloning vats\b', 'tanques de clonagem', t, flags=re.IGNORECASE)
    t = re.sub(r'\bmatter furnace\b', 'fornalha de materia', t, flags=re.IGNORECASE)
    t = re.sub(r'\bgrey goo\b', 'gosma cinza', t, flags=re.IGNORECASE)
    t = re.sub(r'\bairlock\b', 'eclusa', t, flags=re.IGNORECASE)
    t = re.sub(r'\bairlocks\b', 'eclusas', t, flags=re.IGNORECASE)
    t = re.sub(r'\bshipyard\b', 'estaleiro', t, flags=re.IGNORECASE)
    t = re.sub(r'\bbridge\b', 'ponte de comando', t, flags=re.IGNORECASE)
    t = re.sub(r'\bwaypoint\b', 'ponto de rota', t, flags=re.IGNORECASE)
    t = re.sub(r'\bpoise\b', 'postura', t, flags=re.IGNORECASE)
    t = re.sub(r'\bbuff\b', 'efeito benefico', t, flags=re.IGNORECASE)
    return strip_accents(t)

def apply_rules(text):
    if text in MANUAL_TERMS:
        return strip_accents(MANUAL_TERMS[text])
    for pat, repl in REGEX_RULES:
        if re.search(pat, text, re.IGNORECASE):
            res = re.sub(pat, repl, text, flags=re.IGNORECASE)
            return strip_accents(res)
    return None

def translate_api(text):
    # Tenta clientes diferentes se houver erro
    clients = ['dict-chrome-ex', 'webapp', 't']
    for client in clients:
        for attempt in range(2):
            try:
                url = f"https://translate.googleapis.com/translate_a/single?client={client}&sl=en&tl=pt&dt=t&q=" + urllib.parse.quote(text)
                req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36'})
                with urllib.request.urlopen(req, timeout=8) as resp:
                    data = json.loads(resp.read().decode('utf-8'))
                    res = ''.join([part[0] for part in data[0] if part[0]])
                    return clean_term(res)
            except Exception as e:
                time.sleep(0.4 * (attempt + 1))
    return clean_term(text)

def translate_string(text, cache):
    if text in cache:
        return cache[text]
    
    # 1. Regras manuais / regex
    rule_res = apply_rules(text)
    if rule_res:
        cache[text] = rule_res
        return rule_res
    
    # 2. Se for somente numeros / simbolos
    if re.match(r'^[\d\s\+\-\%\.\,\:\;\[\]\(\)\/]+$', text):
        cache[text] = text
        return text

    # 3. Traducao online
    res = translate_api(text)
    cache[text] = res
    return res

def main():
    print("Iniciando processo abrangente de traducao e localizacao...")
    
    with open(UNIFIED_JSON, 'r', encoding='utf-8') as f:
        unified_list = json.load(f)
    print(f"Total de termos no arquivo unificado: {len(unified_list)}")

    # Carregar cache se existir
    cache = {}
    if os.path.exists(CACHE_JSON):
        try:
            with open(CACHE_JSON, 'r', encoding='utf-8') as f:
                cache = json.load(f)
            print(f"Cache carregado com {len(cache)} termos traduzidos.")
        except Exception as e:
            print("Erro ao ler cache:", e)

    # Filtrar os que ainda faltam traduzir
    to_translate = [s for s in unified_list if s not in cache or cache[s] == s]
    print(f"Termos que precisam de traducao: {len(to_translate)}")

    # Primeiro aplica regras em memória nos termos faltantes
    rule_applied = 0
    remaining_api = []
    for s in to_translate:
        rule_res = apply_rules(s)
        if rule_res:
            cache[s] = rule_res
            rule_applied += 1
        else:
            remaining_api.append(s)

    print(f"Aplicadas {rule_applied} traducoes via regras/termos manuais.")
    print(f"Restam {len(remaining_api)} termos para a API online.")

    # Processamento paralelo para os restantes
    completed = 0
    save_interval = 100
    if remaining_api:
        with ThreadPoolExecutor(max_workers=6) as executor:
            future_to_str = {executor.submit(translate_api, s): s for s in remaining_api}
            for future in as_completed(future_to_str):
                s = future_to_str[future]
                try:
                    res = future.result()
                    cache[s] = res
                except Exception as e:
                    cache[s] = clean_term(s)
                
                completed += 1
                if completed % save_interval == 0 or completed == len(remaining_api):
                    print(f"Progresso API: {completed}/{len(remaining_api)}...")
                    # Salva cache incrementalmente
                    with open(CACHE_JSON, 'w', encoding='utf-8') as f:
                        json.dump(cache, f, indent=2, ensure_ascii=False)

    # Salva cache final
    with open(CACHE_JSON, 'w', encoding='utf-8') as f:
        json.dump(cache, f, indent=2, ensure_ascii=False)

    print(f"Cache final salvo com {len(cache)} traducoes.")

    # Agora faz o merge completo no translations_exe.json
    with open(TRANS_JSON, 'r', encoding='utf-8-sig') as f:
        main_trans = json.load(f)

    updated_count = 0
    for k in unified_list:
        if k in cache and cache[k]:
            clean_v = strip_accents(cache[k])
            if k not in main_trans or main_trans[k] != clean_v:
                main_trans[k] = clean_v
                updated_count += 1

    print(f"Total de termos atualizados no translations_exe.json: {updated_count}")
    print(f"Total consolidado no dicionario: {len(main_trans)} termos.")

    # Salvar translations_exe.json na raiz e no patcher
    with open(TRANS_JSON, 'w', encoding='utf-8-sig') as f:
        json.dump(main_trans, f, indent=2, ensure_ascii=False)
    with open(PATCHER_JSON, 'w', encoding='utf-8-sig') as f:
        json.dump(main_trans, f, indent=2, ensure_ascii=False)
    print("translations_exe.json atualizado na raiz e na pasta patcher!")

    # Agora atualizar parameterdb\world_params.sqlite
    import sqlite3
    db_paths = [
        r"D:\SteamLibrary\steamapps\common\Zero Falls\parameterdb\world_params.sqlite",
        os.path.join(BASE_DIR, r"pt_br\parameterdb\world_params.sqlite")
    ]
    # Cria diretorio pt_br\parameterdb se nao existir
    os.makedirs(os.path.join(BASE_DIR, r"pt_br\parameterdb"), exist_ok=True)
    # Se o arquivo de pt_br ainda nao existir, copia da base
    if not os.path.exists(db_paths[1]) and os.path.exists(db_paths[0]):
        import shutil
        shutil.copyfile(db_paths[0], db_paths[1])

    for dbp in db_paths:
        if not os.path.exists(dbp):
            continue
        print(f"Atualizando banco SQLite: {dbp}")
        conn = sqlite3.connect(dbp)
        c = conn.cursor()
        
        # Atualiza crafts
        c.execute("SELECT rowid, name, description, profession FROM crafts")
        craft_rows = c.fetchall()
        for rowid, name, desc, prof in craft_rows:
            new_name = cache.get(name, name)
            new_desc = cache.get(desc, desc)
            new_prof = cache.get(prof, prof)
            c.execute("UPDATE crafts SET name = ?, description = ?, profession = ? WHERE rowid = ?",
                      (strip_accents(new_name), strip_accents(new_desc), strip_accents(new_prof), rowid))
        
        # Atualiza quests
        c.execute("SELECT rowid, name, description FROM quests")
        quest_rows = c.fetchall()
        for rowid, name, desc in quest_rows:
            new_name = cache.get(name, name)
            new_desc = cache.get(desc, desc)
            c.execute("UPDATE quests SET name = ?, description = ? WHERE rowid = ?",
                      (strip_accents(new_name), strip_accents(new_desc), rowid))
            
        conn.commit()
        conn.close()
        print(f"Banco SQLite {dbp} atualizado com sucesso!")

    print("\nProcesso concluido com sucesso!")

if __name__ == '__main__':
    main()

# -*- coding: utf-8 -*-
"""
Script de geração e tradução dos arquivos da Fase 1 (Textos externos)
Wayward Terran Frontier: Zero Falls -> Português (PT-BR)
"""
import os
import shutil

BASE_DIR = r"d:\Tradução WTF Zero falls"
PT_BR_DIR = os.path.join(BASE_DIR, "pt_br")
ORIG_DIR = os.path.join(BASE_DIR, "original")
GAME_DIR = r"D:\SteamLibrary\steamapps\common\Zero Falls"

TRANSLATIONS = {
    # ------------------ TUTORIALS (XML) ------------------
    os.path.join("Tutorials", "Insufficient Resources.xml"): """<?xml version="1.0" encoding="utf-8"?>
<Tutorial>
	<Name>Recursos insuficientes</Name>
	<Page>
		<img src="page1.png" />
		<body>
			<text>Esqueceu de coletar os recursos basicos necessarios para construir sua proxima nave?
			Boas noticias: voce nao ficara preso na sua estacao para sempre.
			Afinal, esta e uma estacao de mineracao.</text> 
		</body>
	</Page>

	<Page>
		<img src="page2.png" />
		<body>
			<text>Pegue o bonde ate a area de mineracao da sua base principal e voce encontrara alguns cristais crescendo que podem ser minerados para obter recursos basicos.
			(Voce precisara de um laser de mineracao, que pode comprar na loja local de itens)
			Minerar dentro da base principal depositara os recursos diretamente nos estoques da sua estacao.</text> 
		</body>
	</Page>

	<Page>
		<img src="page3.png" />
		<body>
			<text>Cristais totalmente desenvolvidos fornecerao significativamente mais recursos do que os menores.
			Consulte os tutoriais sobre cultivo para aprender como os cristais funcionam e como cultivar os seus proprios.</text> 
		</body>
	</Page>
	
</Tutorial>""",

    os.path.join("Tutorials", "Farming", "Basics", "Farming.xml"): """<?xml version="1.0" encoding="utf-8"?>
<Tutorial>
	<Name>Cultivo</Name>
	<Page>
		<img src="page1.png" />
		<body>
			<text>Encontrou um esporo de cristal?
			O cultivo e um minijogo opcional que permite produzir seus proprios recursos passivamente.</text> 
		</body>
	</Page>

	<Page>
		<img src="page2.png" />
		<body>
			<text>Usar um esporo de cristal em blocos de terra plantara um cristal com o genoma configurado no momento.
			Sua base principal possui uma area com terra, e colher cristais aqui adicionara os recursos diretamente ao estoque da sua estacao para uso na construcao de naves.</text> 
		</body>
	</Page>

	<Page>
		<img src="page3.png" />
		<body>
			<text>Seus cristais crescem devagar, entao saia em uma aventura e confira-os quando voltar (ou mais provavelmente quando morrer).
			Voce precisara de um laser de mineracao para colhe-los ou inspecionar seu estado.
			Voce pode comprar um na loja local de itens.</text> 
		</body>
	</Page>
	
</Tutorial>""",

    os.path.join("Tutorials", "Farming", "Genomes", "Crystal genomes.xml"): """<?xml version="1.0" encoding="utf-8"?>
<Tutorial>
	<Name>Genomas de cristal</Name>
	<Page>
		<img src="page1.png" />
		<body>
			<text>Os cristais crescem seguindo regras simples determinadas por seu genoma.
			Para acessar o genoma do seu cristal, clique no espaco de genoma na ficha do seu personagem aqui.</text>
		</body>
	</Page>

	<Page>
		<img src="page2.png" />
		<body>
			<text>Toda vez que plantar um cristal a partir de um esporo, ele usara a configuracao atual do seu genoma.
			Aviso: e possivel criar um genoma que nao cresca de forma alguma!
			Experimente combinacoes diferentes para ver o que funciona melhor.</text>
		</body>
	</Page>

	<Page>
		<img src="page3.png" />
		<body>
			<text>Voce pode configurar seu genoma com os genes que coleta durante suas aventuras.
			Dica: minerar minerios as vezes rende um gene que faz seus cristais produzirem aquele tipo de recurso.</text>
		</body>
	</Page>
	
</Tutorial>""",

    os.path.join("Tutorials", "Mining", "Basics", "Mining.xml"): """<?xml version="1.0" encoding="utf-8"?>
<Tutorial>
	<Name>Mineracao</Name>
	<Page>
		<img src="page1.png" />
		<body>
			<text>Localize uma <font color="c4faff">Jazida de Mineracao</font>. Elas estao distribuidas por varios setores, comumente encontradas perto de cinturoes de asteroides.
O tipo de setor afeta os tipos de jazidas possiveis que podem surgir.
Se o icone estiver vermelho, significa que ha operacoes de mineracao em andamento. Certifique-se de que sua nave esta preparada para combate, mineradores nao toleram concorrencia.</text> 
		</body>
	</Page>

	<Page>
		<img src="page2.png" />
		<body>
			<text>Aproxime-se de um dos <font color="c4faff">Nodulos de Mineracao</font> ao redor da jazida. Voce pode escanear o nodulo usando <keybind type="44" />.
Use suas armas para destruir os pixels destacados e extrair o minerio. Ative o <font color="c4faff">Modo Coletor</font> usando <keybind type="19" /> e aproxime-se do minerio extraido para recolhe-lo.
<font color="700707"><font color="a92d2d">O Modo Coletor</font> limita a velocidade maxima da sua nave.</font>
A chance de extracao bem-sucedida de minerio requer que <font color="c4faff">Rendimento Base + Rendimento de Torre + Bonus</font> seja maior que 0.
Se voce nao conseguir recolher o minerio extraido, significa que seus compartimentos de carga estao cheios.</text> 
		</body>
	</Page>
	
</Tutorial>""",

    # ------------------ LOGS (TDB) ------------------
    os.path.join("Data", "LOGENGZDEF.tdb"): """Bem-vindo ao seu PDA pessoal. Estamos no futuro distante, a humanidade se espalhou pelas estrelas com tecnologia maravilhosa e, mesmo assim, ainda nao conseguimos orcamento para dublagem, entao temos que ler registros de texto. Que lastima.

Sempre que acessar uma entrada de registro durante suas viagens, ela ficara guardada permanentemente aqui para voce consultar quando quiser.

Fechou a janela imediatamente porque se recusa a ler um bloco de texto? Nao se preocupe, os pontos essenciais ficam salvos para sempre e podem ser acessados mais tarde, quando tiver tempo de repensar seu problema de atitude.

Pra ser sincero, mal acredito que preciso explicar isso. Digo, voce vai sequer ler isto? Eu ainda nem clonei voce e ja estou desapontado.

-One""",

    os.path.join("Data", "LOGENGZ0002.tdb"): """Registros do console da Sala C:

Entrada 1:

Eu compreendo que os resultados das minhas acoes sejam ondulatorios. Tomo todas as atitudes possiveis, e as consequencias delas se propagam como ondas a partir de mim na velocidade da luz pelo espaco e pelo tempo. O que nao consigo entender e como o diretor cientifico chefe acha que a coerencia macroscopica pode ser controlada para provocar o teletransporte quantico de uma nave inteira sem violar algo de suma importancia para os pobres coitados enfiados na maldita lata.

A primeira vista, essa missao ja e uma loucura.

Uma nave que viaja para todos os destinos possiveis do universo baseada no puro acaso e ai, com o apertar de um botao, nos podemos trocar de lugar com versoes alternativas de nos mesmos so porque deixamos de saber qual caminho tomamos? Essa foi a explicacao mastigada que me deram, e me deu vontade de fazer birra que nem uma crianca de cinco anos.

Mas quando voce investiga a fundo, fica obvio que essa missao nao e so loucura: e loucura nivel hospicio.

Mudei de ideia. Dizer que escolhas sao ondas de probabilidade propagando-se no tempo a velocidade da luz nao faz sentido nenhum pra mim, e eu me considero alguem bastante racional. Claro que conheco a teoria dos muitos mundos e aceito o basico, mas fui criado seguindo a primeira lei da termodinamica. Se existe outra versao minha em cada canto do universo em alguma dimensao paralela, gostaria que essas versoes continuassem la como elas mesmas enquanto eu fico bem aqui. A ideia de escolher qual delas serei a qualquer momento nao agrada a nenhum de nos. E se eu escolher virar uma versao de mim mesmo virada do avesso sem motivo nenhum?

Quero cair fora dessa nave.

Nao, ja vi coisas que ninguem deveria ver e cheguei a conclusao de que e bem melhor deixar minha funcao de onda se propagar pelo caminho normal -- sabe, aquele onde o futuro e o passado nao trocam de lugar, nao conversam entre si e nao esquecem de proposito quem e quem. Eu vi o que aquele dispositivo faz, estive la em todos os testes preliminares. Nao e um aparelho que cria uma ponte para outro ponto do espaco-tempo; e um troco que se extirpa do universo para que o universo pare de se importar com o local exato onde ele esta. Podem insistir o quanto quiserem que e uma perspectiva filosofica, nao me importo com o quanto bate com os calculos matematicos: esses psicopatas vao nos apagar do universo com uma maquina que aniquila informacao...

Quando o funcionamento do seu aparelho exige, pelas leis da fisica da propagacao de informacao atraves das 3 dimensoes temporais, (EXIGE) que o operador nao tenha a minima pista de qual sera o resultado... quer saber? Dane-se. Estou me demitindo e vou arranjar emprego de faxineiro.""",

    os.path.join("Data", "LOGENGZ0003.tdb"): """Registros do console da Sala C:

Entrada 2:

Acabei de ler o registro do cara anterior e preciso dizer... QUE SORTE A MINHA!

Como eu sei que voces, espioes da SSC, leem meus registros pessoais, so quero que saibam que estou empolgadissimo com essa vaga e voces nao vao se arrepender de ter me contratado de ultima hora.

Aquele cara era pirado das ideias, totalmente anti-profissional.

"Ah nao, um pequeno buraco de minhoca vai me virar do avesso, bua bua."

A ciencia manda, vadias!""",

    os.path.join("Data", "LOGENGZ0004.tdb"): """Devido ao alto custo energetico de manter aberta uma rede de portais, os portais de dobra permanecem fechados, exceto quando uma nave esta cruzando por eles. Sendo assim, a transmissao de dados em longa distancia no espaco profundo pode ser um grande transtorno, ja que as informacoes precisam ser transportadas fisicamente pelos portais a bordo de uma nave.

Os nos de nucleo quantico sao uma solucao moderna para a necessidade do transporte fisico de dados. Eles permitem que a decisao sobre "qual informacao" seja tomada no exato momento da recepcao dos dados, e nao no momento do envio, possibilitando transacoes instantaneas de informacao em qualquer parte da galaxia sem nenhuma dor de cabeca relativistica.

Ha mais de uma forma de compreender como um no de nucleo quantico funciona.

A interpretacao classica define que existe uma quantidade finita de nos de nucleo e um numero finito de fragmentos de dados que podem ser distribuidos pela rede de nos, porem a informacao e inserida nos nos de tal maneira que se torna impossivel saber qual informacao esta em qual no. Dessa forma, cada no se encontra em um estado de superposicao contendo todos os fragmentos de informacao alocados na rede. A estrutura cristalina interna de cada no e projetada para agir como um mapa, permitindo ao usuario colapsar seletivamente a funcao de onda no fragmento exato de informacao desejado.

No entanto, a interpretacao dos muitos mundos e muito mais divertida.

De acordo com a teoria dos muitos mundos, cada no existe como uma matriz infinita espalhada por universos paralelos infinitos. Em cada universo, um no recebe exatamente 1 fragmento de informacao e o retem para sempre. Qual informacao vai para qual no segue uma elaborada funcao de onda de probabilidade multidimensional, mas isso pouco importa para o usuario. Quem utiliza um no quantico descobre que e ele proprio quem esta em superposicao, existindo em todos os universos simultaneamente; e, convenientemente, enquanto o no permanecer fechado, a pessoa nem percebe, ja que esses universos infinitos sao quase identicos entre si. Ao abrir o no e seguir o mapa, ele guia a pessoa no colapso de si mesma em um universo mais especifico, onde ela constatara que a informacao desejada sempre esteve armazenada no no que ela carrega.

Um sabio estudioso notara que essas interpretacoes sao apenas 2 perspectivas distintas sobre o mesmo mecanismo. Pode-se crer que o conteudo do no seja uma superposicao que colapsa naquilo que voce procura, ou pode-se crer que voce mesmo seja a superposicao que colapsou em um universo que contem o que voce buscava. De qualquer modo, o notavel triunfo tecnologico reside no mapa, capaz de guiar uma funcao de onda distribuida por tantas dimensoes a colapsar perfeitamente em todas elas, de forma que, seja qual for a perspectiva adotada, o resultado final e satisfatorio.""",

    # ------------------ GREY SALES (TXT) ------------------
    os.path.join("grey sales", "Courier b.txt"): """Courier b
O Courier b da Frente de Liberacao de Energia e o que ha de mais avancado em projeto de nave de uso geral. Equipado integralmente com tecnologia de nivel 2, esta variante do Courier divide igualmente sua capacidade entre espaco de carga, acomodacoes de tripulacao, defesas e instalacoes de suporte de vida de longa duracao.""",

    os.path.join("grey sales", "Courier t.txt"): """Courier t
O Courier t da Frente de Liberacao de Energia e uma nave de transporte moderna e luxuosa. Conta com beliches de alta capacidade, quartos privativos e ate mesmo um par de suites de luxo para passageiros espaciais que exigem viajar com estilo.""",

    os.path.join("grey sales", "Dagger A1.txt"): """Dagger A1
A Traverse Shipworks possui contratos de defesa nao exclusivos com 6 das 7 colonias humanas e, raramente, suas naves chegam a ser vendidas para civis. A Dagger A1 (configuracao de assalto 1) e uma corveta interceptadora veloz utilizada tanto por forcas militares quanto policiais na maior parte do espaco humano. Devido ao seu uso frequente no controle de imigracao e alfandega comercial, este modelo ja vem com um scanner de carga.""",

    os.path.join("grey sales", "Glaive A1.txt"): """Glaive A1
A Traverse Shipworks possui contratos de defesa nao exclusivos com 6 das 7 colonias humanas e, raramente, suas naves chegam a ser vendidas para civis. O Glaive A1 (configuracao de assalto 1) e um destroier de misseis equipado com 3 lancadores e capacidade para 72 misseis. (Misseis vendidos separadamente)""",

    os.path.join("grey sales", "Hauler b.txt"): """ELF Hauler b
O Hauler b da Frente de Liberacao de Energia e uma versao mais balanceada de seu popular cargueiro, trocando um pouco da capacidade de carga por melhores opcoes de defesa e maior conforto para a tripulacao.""",

    os.path.join("grey sales", "Hauler c.txt"): """ELF Hauler c
O Hauler c da Frente de Liberacao de Energia representa o apice quando o assunto e transporte de carga pesada a longas distancias. Com defesas decentes e comodidades suficientes para manter a tripulacao confortavel em viagens muito longas, esta nave nao abre mao de nada em capacidade de carga. Voce simplesmente nao encontrara no espaco humano outra nave com maior capacidade de transporte pesado.""",

    # ------------------ ORANGE SALES (TXT) ------------------
    os.path.join("orange sales", "Boulder b.txt"): """Monolith Boulder b
Uma reliquia de quando a Monolith ainda tentava se consolidar como fornecedora militar, o Boulder e a unica nave da Monolith construida em torno de um lancador dorsal de foguetes pesados montado na proa. Hoje em dia, a variante b e a unica que ainda mantem esse lancador desatualizado, ja que todas as outras variantes o removeram para dar espaco a outros modulos. O B vem de "balanceada", significando que e comercializada como uma nave polivalente; no entanto, sua origem como projeto militar a torna especialmente popular entre piratas.""",

    os.path.join("orange sales", "Boulder m.txt"): """Monolith Boulder m
Esta conversao total do Boulder substitui o lancador central de foguetes por uma abundancia de espaco de carga e troca as armas principais por um conjunto de cargas de mineracao, transformando-o em uma respeitavel barcaca mineradora. As acomodacoes da tripulacao permanecem quase inalteradas.""",

    os.path.join("orange sales", "Eagle b.txt"): """Monolith Eagle b
Traga um amigo amante da liberdade, pois a Monolith Eagle b e a nave de guerra economica perfeita para 2 jogadores pilotando em equipe. Ela nao tem uma capacidade impressionante de carga nem de passageiros para o seu porte, mas conta com uma cozinha de bordo excelente e reciclagem de residuos solidos, permitindo que uma dupla se aventure fundo em territorio hostil com poder de fogo suficiente para abrir caminho a bala na volta. A ponte principal comanda um suporte balistico medio frontal e um lancador de misseis, enquanto a estacao de artilharia opera um par de pequenos suportes rotativos de 360 graus para defesa de ponto, contando com escudos de regeneracao passiva e ativa.""",

    os.path.join("orange sales", "Hammer b.txt"): """Monolith Hammer b
O Hammer b e a tentativa da Monolith de criar uma versao equilibrada para a sua linha Hammer. Possui os mesmos suportes de torreta da versao de mineracao, mas vem de fabrica com canhoes automaticos em vez de cargas mineradoras. Seu interior foi adaptado para oferecer mais espaco e conforto aos passageiros, incluindo uma pequena cozinha e reciclagem de agua.""",

    os.path.join("orange sales", "Hammer m.txt"): """Monolith Hammer m
O Hammer m e otimizado para operacoes de mineracao em pequena escala. Vem com um par de pequenas cargas frontais de mineracao para fragmentar o minerio e duas pequenas metralhadoras na traseira para espantar quem tentar atrapalhar seus trabalhos. Embora seus 3 beliches nao sejam dos mais confortaveis, sua capacidade de carga respeitavel e escudos modestos fazem dele um verdadeiro burro de carga para cumprir o trabalho gastando pouco.""",

    os.path.join("orange sales", "heavy shuttle.txt"): """ELF Heavy Shuttle
Para nao ficar atras da Monolith, os engenheiros da Frente de Liberacao de Energia criaram a variante Heavy Shuttle sobre o popular chassi da Runner. Seu layout e extremamente eficiente e oferece uma melhoria clara em relacao ao Shuttle convencional: o dobro de capacidade de carga, um beliche adicional e um conjunto extra de sensores para travar miras com mais facilidade em naves menores.""",

    os.path.join("orange sales", "Junebug b.txt"): """Monolith Junebug b
Este e o layout interno que fez da Junebug uma sensacao absoluta entre autonomos e corsarios, tornando-se uma das naves mais populares de toda a galaxia. Projetada como um iate de luxo, com cabines privativas para a tripulacao, cozinha e sistema de reciclagem de agua, ela ainda traz armamento acima da media, espaco de carga generoso e escudos minimos que garantem sua sobrevivencia em tiroteios. Pelo preco surpreendentemente baixo, a Junebug b (balanceada) desempenha multiplas funcoes com excelencia impressionante.""",

    os.path.join("orange sales", "Junebug c.txt"): """Monolith Junebug c
Utilitaria ao extremo, a Junebug c (de carga) aproveita cada centimetro cubico da popular carcaca da Junebug para armazenar mercadorias. Tambem vem com escudos acima da media para o seu porte e uma pequena torreta traseira destinada a desencorajar piratas. O conforto da tripulacao e minimo e apertado.""",

    os.path.join("orange sales", "Junebug m.txt"): """Monolith Junebug m
Apesar de nao contar com grande capacidade de carga para transportar minerios brutos, esta variante de mineracao da Junebug possui excelentes torretas de extracao mineral, sendo uma adicao valiosa para qualquer comboio de mineracao de grande porte.""",

    os.path.join("orange sales", "Junebug t.txt"): """Monolith Junebug t
A Junebug t (transporte) e tida como a balsa das estrelas. E lenta e desprovida de defesas, mas proporciona o maximo conforto aos passageiros com um orcamento minusculo. E tao economica de operar que inumeros pequenos governos mantem frotas inteiras delas para transportar civis de estacao em estacao sob a seguranca dos seus sistemas mais populosos.""",

    os.path.join("orange sales", "Welder b.txt"): """Monolith Welder b
A Welder b (balanceada) e uma nave muito equilibrada. Com duas torretas leves frontais e um gerador de escudo de porte completo, seu poder de combate fica bem acima da media, mantendo ainda uma capacidade de carga muito digna e espaco suficiente para 3 passageiros. Um diferencial notavel da Welder B e seu sistema de reciclagem de residuos solidos, permitindo cultivar cubos de algas para complementar as racoes alimentares.""",

    os.path.join("orange sales", "Welder c.txt"): """Monolith Welder c
Projetada como um transportador leve de carga, esta nave tambem e muito procurada para pequenas operacoes de mineracao gracas a sua capacidade de montar duas cargas mineradoras leves frontais. A nave possui excelente espaco de carga e escudos basicos de protecao."""
}

def main():
    print("Gerando arquivos traduzidos em pt_br...")
    for rel_path, content in TRANSLATIONS.items():
        target = os.path.join(PT_BR_DIR, rel_path)
        os.makedirs(os.path.dirname(target), exist_ok=True)
        with open(target, "w", encoding="utf-8") as f:
            f.write(content)
        print(f"  [OK] {rel_path}")

    # Copiar PNGs de tutoriais
    print("\nCopiando imagens de tutoriais...")
    tut_src = os.path.join(ORIG_DIR, "Tutorials")
    for root, dirs, files in os.walk(tut_src):
        for file in files:
            if file.lower().endswith(".png"):
                rel_dir = os.path.relpath(root, tut_src)
                src_file = os.path.join(root, file)
                dst_file = os.path.join(PT_BR_DIR, "Tutorials", rel_dir, file)
                os.makedirs(os.path.dirname(dst_file), exist_ok=True)
                shutil.copy2(src_file, dst_file)
                print(f"  [PNG] {file} -> {rel_dir}")

    print(f"\nConcluído com sucesso! {len(TRANSLATIONS)} arquivos traduzidos gerados em pt_br.")

if __name__ == "__main__":
    main()

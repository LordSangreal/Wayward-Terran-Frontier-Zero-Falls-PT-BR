# Tradução PT-BR v1.0.1 - Wayward Terran Frontier: Zero Falls

Tradução completa e atualizada para **Português do Brasil (PT-BR)** do jogo **Wayward Terran Frontier: Zero Falls** (Steam AppID: `392080`).

---

## 🚀 Como Instalar (Instalação Fácil em 1 Clique)

1. Baixe o pacote **`Zero_Falls_PTBR_v1.0.1.zip`** na aba [Releases](https://github.com/LordSangreal/Wayward-Terran-Frontier-Zero-Falls-PT-BR/releases).
2. Extraia o arquivo ZIP em qualquer pasta do seu computador.
3. Dê um duplo clique no arquivo **`Instalar_Traducao.bat`**.
4. O instalador detectará automaticamente a pasta do jogo na sua Steam (ou solicitará o caminho caso esteja em outra partição personalizada).
5. Pronto! Inicie o jogo normalmente pela sua biblioteca da **Steam**.

---

## 📋 Notas de Atualização - Versão 1.0.1

### 🛠️ Correções Críticas de Estabilidade e Compatibilidade
- **Suporte Total à Nova Versão de Testes Steam (`unstable` beta):** Compatibilidade com a versão de teste mais recente do jogo.
- **Proteção Total de Desserialização e Saves:** 
  - Corrigido o erro que impedia o carregamento de saves antigos e corrompia os fluxos binários (`crashlog4.bin` / `crashlog5.bin`).
  - Isoladas todas as rotinas internas de leitura e gravação de dados (`AgentTracker`, `AGENTBAG`, `TriggerEvent`, `GameFile` e métodos `read`/`write`) para garantir integridade matemática dos fluxos de dados do jogo.
  - Mantida a integridade referencial do banco de dados `world_params.sqlite`, garantindo que chaves internas de missões e receitas não quebrem o carregamento de mundo.
- **Auto-migração de Saves Antigos (`professions`):** Injeção de auto-migração nativa via Mono.Cecil no método `CHARACTER_DATA::loadWorldCharacter` para criar automaticamente a tabela `professions` ausente em saves de versões anteriores.
- **Desbloqueio de Campanha / Singleplayer:** Corrigido o travamento do menu principal que forçava apenas o modo multijogador na branch de testes (`CONFIG::openMP` revertido para `false`).
- **Detecção Inteligente de Atualizações Steam:** O instalador detecta atualizações automáticas da Steam, preserva a versão anterior em `WTF_backup_anterior.exe` e cria um backup limpo do novo executável oficial.

### 🌐 Localização Aprofundada e Novos Termos
- **Tradução e Renderização dos Cabeçalhos da Árvore de Talentos / Maestria (`TalentsScreenV`):**
  - As 5 categorias no topo da árvore de talentos/maestrias, que eram imagens em inglês pré-renderizadas na engine (`mastery_cat_*.xnb`), agora são interceptadas e renderizadas nativamente em português, perfeitamente alinhadas com o frame e tipografia do jogo:
    - *Ballistic weapons* $\rightarrow$ **Armas balisticas**
    - *Energy weapons* $\rightarrow$ **Armas de energia**
    - *Emitters* $\rightarrow$ **Emissores**
    - *Missiles* $\rightarrow$ **Misseis**
    - *Construction* $\rightarrow$ **Construcao**
- **Eliminação de Sobreposição de Textos nos Tooltips de Talentos (`TipStatSmall`):**
  - Ajustados os nomes de estatísticas para comprimentos compactos ($\le 22$ caracteres), eliminando completamente a colisão sobre os valores numéricos e porcentagens alinhados à direita na caixa de tamanho fixo (272 px):
    - *"Aceleracao lateral"* (corrigindo *"Multiplicador de aceleracao lateral"* que colidia com `150%`)
    - *"Propulsor -> acel."* (corrigindo *"Propulsores contribuem -> aceleracao"* que colidia com `50%`)
    - *"Alavanca de propulsor"* (corrigindo *"Aumento do braco da alavanca do propulsor"*)
    - Unidade de distância corrigida para **" px"** (eliminada a tradução bizarra onde *" pix"* aparecia como *" foto"*)
    - Termo de armamentos corrigido para **"Alcance"** (eliminada a tradução de *"Range"* que aparecia como *"Faixa"*)
    - Carga elétrica corrigida para **"Carga extra por tick"** (eliminado *"Cobranca"*)
    - Revisadas mais de 20 estatísticas de talentos (*"Risco explosao reator"*, *"Risco fogo capacitor"*, *"Anulacao dano blindagem"*, *"Mitigacao dano global"*, *"Energia drenada/arco"*, etc.).
- **Correção dos Prompts de Interação no Cenário:**
  - `[F] Entrar na barra` corrigido para a tradução natural e correta: **`[F] Entrar no bar`**.
  - `[F] Comerciar com a loja` e `Comerciar com a estacao` refinados para mensagens concisas e fluidas: **`[F] Acessar loja`** e **`[F] Acessar estacao`**.
- **Correção da Interface de Pesquisa e HUD (Research UI & Stats):**
  - **Eliminação de Sobreposição de Textos:** Corrigida a colisão horizontal e a sobreposição vertical no painel de detalhes da tela de pesquisas (`ResearchScreenRev3`). A descrição dos desbloqueios de módulos foi reorganizada com formatação compacta em tópicos limpos, cabendo perfeitamente dentro do limite de altura do painel sem transbordar para cima do quadro de pré-visualização (`ImageCanvas`).
  - **Tradução e Padronização dos Filtros e Níveis no Topo:** As abas superiores da tela de pesquisa foram traduzidas e padronizadas: *"Geral"* (corrigido do anterior "Em geral"), *"Naves"* (padronizado de "Navios"), além da uniformização dos botões de camada para *"Nivel 1"*, *"Nivel 2"* e *"Nivel 3"* (eliminando misturas entre "Camada" e "Nivel").
  - **Tradução Integral do HUD de Estatísticas e Recursos (`WidgetResources`):** Localização completa de todas as categorias do painel de atributos da nave: `[Estatisticas de Manobra]`, `[Estatisticas de Torretas]`, `[Estatisticas Defensivas]`, `[Defesa de Ponto]`, `[Outras Estatisticas]`, `Habilidades`, `Reputacoes`, bem como todos os parâmetros de combate e navegação (`Aceleracao`, `Giro`, `Deslocamento lateral`, `Multiplicador de cadencia`, `Multiplicador de dano`, `Chance de critico`, `Chance de tiro duplo`, `Velocidade de recarga`, `Precisao`, `Esquiva`, `Raspao`, `Dureza do escudo`, `Dureza da blindagem`, `Alcance PD`, `Dano PD`, `Sensores`, `Visibilidade`, etc.).
  - **Revisão Completa de Suportes de Torretas e Esquemas de Armas:** Tradução fluida de dezenas de suportes e módulos que estavam misturados em inglês ou corrompidos (`Suporte de Arma Minusculo`, `Suportes de Torreta Pequeno/Medio/Grande`, `Drones de Ataque com Projeteis 4x`, `Compartimento de Misseis de Cerco`, `Alcance de visao`), além de mais de 50 descrições de esquemas de projetos de armas e torretas (`Design schematics`).
- **Tradução Completa de Itens e Textos de Sabor:**
  - 100% dos nomes e descrições de minérios, materiais, sucatas, componentes artesanais, sementes de cristal e chips de módulos (`ITEMBAG`, `LOOTBAG`, `TILEBAG`).
  - Textos de sabor traduzidos e adaptados (descrições bem-humoradas da Terra Antiga, Comida Humana, Matéria Exótica, Confinamento molecular de caixas, etc.).
- **Mensagens Contextuais e Avisos no Canto Inferior Esquerdo (HUD/Chat):**
  - Interação com dormitório/cama: *"Voce foi projetado especificamente para nao dormir"*.
  - Portas e Eclusas: *"Ativar eclusa"*, *"Porta travada aberta"*.
  - Consoles da ponte: *"Usar console de navegacao"*, *"Usar console de artilharia"*, *"Este console nao esta conectado a nada"*, *"Necessita de reparos"*.
  - Ações de criação: *"Fabricar gosma"* (corrigido do antigo "gosma artesanal") e *"Fabricar arma"*.
  - Manifesto de voo e passageiros (`SPManifest`): avisos de decolagem sem espaço no inventário, passageiros aguardando no hangar por falta de leitos, reembolso de caução de missões e upgrades gratuitos de cabine.
  - Voo e Combate (`VNavigationRev3`): avisos de desacoplamento, automação de consoles de tiro, piloto automático e carregamento do motor Higgs.
  - Comércio e Armazém: avisos de recusa *"A loja nao compra este tipo de item"* e botões de transferência.
- **Dicionário Expandido:** Mais de **8.270 termos consolidados** e mais de **12.700 instruções IL traduzidas** com estrita observância do padrão ASCII puro (sem caracteres gráficos que quebram a renderização de fontes do MonoGame).

---

## 🔄 Como Restaurar o Jogo Original em Inglês
Caso deseje voltar o jogo para o inglês original a qualquer momento:
1. Dê um duplo clique em **`Restaurar_Original.bat`**.
2. O script restaurará o executável original a partir de `WTF_backup.exe` e o banco de parâmetros original.
3. Se preferir fazer via Steam: clique com o botão direito no jogo na Steam -> **Propriedades** -> **Arquivos Instalados** -> **Verificar integridade dos arquivos**.

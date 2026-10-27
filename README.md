# Tradução PT-BR - Wayward Terran Frontier: Zero Falls

Tradução completa para **Português do Brasil (PT-BR)** do jogo **Wayward Terran Frontier: Zero Falls** (Steam AppID: `392080`).

---

## 🚀 Como Instalar (Instalação Fácil em 1 Clique)

1. Baixe o repositório clicando em **Code** -> **Download ZIP** no GitHub (ou baixe na aba [Releases](https://github.com/LordSangreal/Wayward-Terran-Frontier-Zero-Falls-PT-BR/releases)).
2. Extraia o arquivo ZIP em qualquer pasta do seu computador.
3. Dê um duplo clique no arquivo **`Instalar_Traducao.bat`**.
4. O instalador detectará automaticamente a pasta do jogo na sua Steam (ou solicitará o caminho caso esteja em outra partição personalizada).
5. Pronto! Inicie o jogo normalmente pela sua biblioteca da **Steam**.

---

## 📂 Onde fica a pasta do jogo? (Como encontrar)

Caso o instalador peça para informar a pasta do jogo:
1. Abra a sua **Steam**.
2. Vá na sua **Biblioteca**, clique com o botão direito em **Wayward Terran Frontier: Zero Falls**.
3. Selecione **Gerenciar** -> **Navegar pelos arquivos locais**.
4. Copie o endereço da barra do Explorador de Arquivos (por exemplo: `C:\Program Files (x86)\Steam\steamapps\common\Zero Falls` ou `D:\SteamLibrary\steamapps\common\Zero Falls`).

---

## 🛠️ Instalação Manual (Opcional)

Se preferir fazer sem o script `.bat`:
1. Copie o conteúdo da pasta `pt_br/` (`Tutorials`, `Data`, `grey sales`, `orange sales`) diretamente para dentro da pasta do jogo (`Zero Falls/`).
2. Abra a pasta `patcher/`, copie os arquivos para a pasta do jogo e execute `Patcher.exe`.
3. O `Patcher.exe` criará automaticamente um backup `WTF_backup.exe` e aplicará a tradução com segurança.

---

## 🔄 Como Desinstalar / Restaurar o Inglês Original

- **Pelo Restaurador:** Execute o arquivo **`Restaurar_Original.bat`**. Ele restaurará o `WTF_backup.exe` criado na instalação.
- **Pela Steam:**
  1. Clique com o botão direito no jogo na Steam e vá em **Propriedades**.
  2. Na aba **Arquivos Instalados**, clique em **Verificar integridade dos arquivos do jogo**.

---

## 🎮 O que está traduzido nesta versão:

- [x] **Diálogos e NPCs:** 100% das conversas com a IA *One*, capitães piratas, passageiros, tripulantes e estações espaciais.
- [x] **Missões e História:** Todas as campanhas da Fase 1, Fase 2, Desafios Gauntlet, Enseada dos Piratas, Cerco SSC e missões secundárias.
- [x] **Naves e Módulos:** Nomes e descrições de torretas, canhões de raio, mísseis, propulsores, reatores e escudos.
- [x] **Interface e Menus (HUD):** Telas de configuração, controle de voo, atracação, motor de viagem (`[T]`), barras de status e log de mensagens.
- [x] **Construção e Economia:** Todos os blocos de construção de naves/estações (`TILEBAG`), minérios (ouro, ferro, platina, neutralino) e comércio.
- [x] **Talentos e Habilidades:** Árvore de evolução do personagem, implantes, auras e bônus passivos.
- [x] **Arquivos Externos:** Todos os tutoriais ilustrados em XML, logs de estações (`.tdb`) e catálogos das concessionárias (`grey sales` e `orange sales`).

---

## ℹ️ Nota sobre Acentos e Fontes

Por limitações técnicas da biblioteca gráfica nativa do jogo (**MonoGame / SpriteFont**), a fonte do jogo suporta estritamente os caracteres ASCII 32 a 126. Acentuações gráficas (`á`, `é`, `ç`, `ã`, `õ`, etc.) desaparecem ou ficam invisíveis na tela. Por esse motivo, todo o texto foi adaptado e normalizado sem acentos gráficos para garantir legibilidade perfeita, sem cortes ou falhas na interface.

---

## 🛡️ Estabilidade e Compatibilidade

- **Proteção contra crash de Shaders:** O patcher possui uma lista de exclusão com 311 parâmetros e técnicas de shaders internos da engine gráfica do jogo, garantindo estabilidade absoluta ao pilotar naves, atracar e alternar visões de câmera (F1/F2).
- Compatível com a versão Steam **0.3.0.0+**.

---

## 🤝 Créditos
- **Tradução e Patching:** LordSangreal & Antigravity
- **Ferramentas:** Mono.Cecil (Assembly Inspection & Injection)

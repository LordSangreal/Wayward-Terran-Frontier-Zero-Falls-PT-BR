using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

public class Program {
    public static void Main() {
        string[] paths = new string[] {
            @"d:\Tradução WTF Zero falls\patcher\translations_exe.json",
            @"d:\Tradução WTF Zero falls\translations_exe.json"
        };

        var updates = new Dictionary<string, string>() {
            { "Factory", "Fabrica" },
            { "Processing", "Processamento" },
            { "Trade hub", "Centro Comercial" },
            { "Mining", "Mineracao" },
            { "Shipyard", "Estaleiro" },
            { "Farm", "Fazenda" },
            { "Repair station", "Estacao de Reparo" },
            { "Abandoned station", "Estacao Abandonada" },
            { "Deactivated gate", "Portal Desativado" },
            { "Active gate", "Portal Ativo" },
            { "Home station", "Estacao Principal" },
            { "Waypoint (right click)", "Ponto de rota (clique dir.)" },
            { "Point of interest", "Ponto de interesse" },
            { "Unknown nebula", "Nebulosa desconhecida" },
            { "some stuff in here", "algumas coisas aqui" },
            { "Domum belt", "Cinturao Domum" },
            { "Rich fertile land with an abundance of the low value bulk minerals needed for a massive terraforming operation. This area was under active prospecting when terraforming operations were shut down.", "Terra rica e fertil com abundancia de minerais basicos de baixo valor necessarios para uma operacao massiva de terraformacao. Esta area estava sob prospeccao ativa quando as operacoes de terraformacao foram encerradas." },
            { "Grey belt", "Cinturao Cinzento" },
            { "Bigger pockets of valuable resources and lower quantities of bulk materials made this belt less desirable for terraforming operations and more beneficial for commerce and the production of goods and services for terraforming workers.", "Reservas maiores de recursos valiosos e menor quantidade de materiais basicos tornaram este cinturao menos desejavel para operacoes de terraformacao e mais benefico para o comercio e a producao de bens e servicos para os trabalhadores da terraformacao." },
            { "Wolf nebula", "Nebulosa Lobo" },
            { "Originally a mineral rich asteroid belt, this region was drained of all valuable minerals during the early stages of terraforming. The process used genetically engineered synthetic crystals to transform the larger asteroids into inert crystalline husks and clouds of red dust which block nearly all useful solar radiation.", "Originalmente um cinturao de asteroides rico em minerais, esta regiao foi drenada de todos os minerais valiosos durante os primeiros estagios da terraformacao. O processo usou cristais sinteticos geneticamente modificados para transformar os asteroides maiores em cascas cristalinas inertes e nuvens de poeira vermelha que bloqueiam quase toda a radiacao solar util." },
            { "Infested nebula", "Nebulosa Infestada" },
            { "This mysterious region has been labeled as totally off limits by the SSC and all information regarding it is classified top secret.", "Esta regiao misteriosa foi classificada como totalmente proibida pela SSC e todas as informacoes sobre ela sao ultrasecretas." },
            { "Outer belt", "Cinturao Externo" },
            { "The outer limits of the system, these vast ice fields at the edge of insterstellar space seem to indicate that the local star may have had a more violent past.", "Os limites externos do sistema, estes vastos campos de gelo na borda do espaco interestelar parecem indicar que a estrela local pode ter tido um passado mais violento." },
            { "Guaranteed occurrence", "Ocorrencia garantida" },
            { "Possible occurrence", "Ocorrencia possivel" },
            { "Base yield:", "Rendimento base:" },
            { "Base yield: ", "Rendimento base: " },
            { "Elite flotilla", "Flotilha de Elite" },
            { "A danagerous hostile ship has been detected in this area", "Uma nave hostil perigosa foi detectada nesta area" },
            { "Storm", "Tempestade" },
            { "This sector is experiencing a space weather event", "Este setor esta enfrentando um evento climatico espacial" },
            { "Hive", "Colmeia" },
            { "Animated invaders have built a hive in this session.", "Invasores animados construiram uma colmeia nesta sessao." },
            { "Invasion", "Invasao" },
            { "The system is being invaded by animated ships. This entire sector is currently a battleground. Local defense authorities will reward anyone who can assist with the war effort.", "O sistema esta sendo invadido por naves animadas. Todo este setor e atualmente um campo de batalha. As autoridades locais de defesa recompensarao quem puder ajudar no esforco de guerra." },
            { "Warp to location", "Dobra para o local" },
            { "Station Overview - Click magrail to teleport", "Visao Geral da Estacao - Clique no magrail para teletransportar" },
            { "Cargo Scan:", "Varredura de Carga:" },
            { " to Hail", " para Chamar" },
            { " to Dock", " para Acoplar" },
            { "Turret yield", "Rendimento da torreta" },
            { "Bonus yield", "Rendimento bonus" }
        };

        foreach (var path in paths) {
            var lines = new List<string>(File.ReadAllLines(path));
            var regex = new Regex(@"^\s*""((?:\\.|[^""\\])*)""\s*:\s*""((?:\\.|[^""\\])*)""");
            var keyLineIndex = new Dictionary<string, int>();

            for (int i = 0; i < lines.Count; i++) {
                var m = regex.Match(lines[i]);
                if (m.Success) {
                    string k = Regex.Unescape(m.Groups[1].Value);
                    if (!keyLineIndex.ContainsKey(k)) {
                        keyLineIndex[k] = i;
                    }
                }
            }

            foreach (var kvp in updates) {
                string escKey = kvp.Key.Replace("\\", "\\\\").Replace("\"", "\\\"");
                string escVal = kvp.Value.Replace("\\", "\\\\").Replace("\"", "\\\"");
                string newLine = "  \"" + escKey + "\": \"" + escVal + "\",";

                if (keyLineIndex.ContainsKey(kvp.Key)) {
                    int lineIdx = keyLineIndex[kvp.Key];
                    // Se for a ultima chave sem virgula
                    if (!lines[lineIdx].TrimEnd().EndsWith(",")) {
                        newLine = "  \"" + escKey + "\": \"" + escVal + "\"";
                    }
                    lines[lineIdx] = newLine;
                    Console.WriteLine("Updated: " + kvp.Key + " => " + kvp.Value);
                } else {
                    // Inserir antes da ultima linha "}"
                    int lastBrace = lines.FindLastIndex(l => l.Trim() == "}");
                    if (lastBrace > 0) {
                        // Garantir que a linha anterior tenha virgula
                        if (!lines[lastBrace - 1].TrimEnd().EndsWith(",")) {
                            lines[lastBrace - 1] = lines[lastBrace - 1].TrimEnd() + ",";
                        }
                        lines.Insert(lastBrace, "  \"" + escKey + "\": \"" + escVal + "\"");
                        Console.WriteLine("Inserted: " + kvp.Key + " => " + kvp.Value);
                    }
                }
            }

            File.WriteAllLines(path, lines, Encoding.UTF8);
            Console.WriteLine("Saved: " + path);
        }
    }
}

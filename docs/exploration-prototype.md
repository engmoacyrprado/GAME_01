# Prototipo de exploracao e primeira mudanca visual

## Base aprovada

O jogador avaliou o combate como mais balanceado. Mantidos os 10 segundos de decisao, escala ATB 1.8, preparacao por aliado, custos, atributos e intervalo visual de 0.55s.

## Jogar o percurso

Executavel: Builds/Adventure/GAME_01.exe. A pasta completa Adventure deve permanecer junta. O combate isolado continua em Builds/PC/GAME_01.exe.

1. Mova o Heroi com WASD ou setas. Caminhos, arvores, agua e limites possuem geometria simples; arvores, agua e bordas bloqueiam o movimento.
2. Siga o objetivo indicado no rodape ate o Lobo e o Bandido no cruzamento central.
3. Aproxime-se e pressione E para entrar na batalha. O encontro nao dispara so por encostar.
4. Prepare os aliados e use Iniciar teste. O combate e o mesmo que foi aprovado.
5. Ao terminar, use Voltar ao mapa. A vitoria remove o encontro e desbloqueia o marco. A derrota permite tentar novamente.
6. Caminhe ate o marco dourado a nordeste e pressione E. O mapa ganha outra paleta, mais detalhes e o Heroi recebe uma representacao um pouco mais completa.
7. Procure o portal a leste e pressione E para concluir este percurso de teste.

Reiniciar percurso limpa o progresso desta demo e volta ao inicio. Fechar o executavel tambem perde esse progresso: salvamento em disco ainda nao existe.

## Implementacao

- ExplorationPrototype e BattlePrototype sao cenas separadas, ambas incluidas na compilacao da aventura.
- AdventureRun guarda posicao, encontro ativo/vencido, era e conclusao. A instancia persiste em memoria nas trocas de cena.
- ExplorationController le input, normaliza diagonais, movimenta Rigidbody2D, aplica alcance de interacao e acompanha a camera.
- ExplorationWorld monta o mapa e colisores. WorldActorView faz a apresentacao geometrica do personagem.
- ExplorationHud mostra objetivo, direcao, distancia e interacao disponivel.
- BattleDebugBootstrap permite retornar apenas apos BattleEnded e transfere o resultado ao percurso.
- A vitoria desbloqueia a segunda era. A mudanca visual nao altera colisores, atributos ou regras do combate.

## Recriar e compilar

No Unity: GAME_01 > Adventure > Open Exploration Prototype. Para compilar: GAME_01 > Adventure > Build PC Vertical Slice.

O gerador preserva cenas existentes e inclui ambas nas Build Settings. Cada build valida regras de combate e progressao antes de gerar o executavel.

Validacoes por linha de comando: Game01.Adventure.Editor.AdventureValidation.Run (batchmode com -quit) e Game01.Adventure.Editor.AdventurePlayModeValidation.Run (batchmode sem -quit e com graficos).

## O que esta sendo validado

O ciclo explorar -> interagir -> combater -> voltar -> desbloquear -> evoluir visualmente -> concluir. A historia, os nomes do mundo e os eventos canonicos nao foram definidos nesta entrega. O encontro e o marco sao placeholders tecnicos.

As duas eras usam geometria provisoria e cores. Nao representam a arte final, uma implementacao completa de pixel art nem transicao para 3D. A passagem de era atual e uma troca visual imediata; cinematica, audio e efeitos de transicao ficam para depois.

O mapa controla apenas o Heroi; os dois aliados continuam disponiveis no combate. Os participantes da batalha iniciam com os recursos dos assets. Inventario, XP, persistencia de HP/MP entre encontros, salvamento e versao mobile ainda nao foram implementados.

## Verificacao

11 verificacoes de progressao: estado inicial, bloqueio de avancos, encontro unico, derrota/repeticao, vitoria, preservacao de posicao, transicao, conclusao e reinicio.

Play Mode: movimento real com fisica, bloqueio por arvore, alcance de interacao, entrada na cena de combate, botao de retorno bloqueado antes do fim, vitoria, retorno/posicao, remocao do encontro vencido, atualizacao da representacao do Heroi e conclusao.

O teste integrado forca a derrota dos inimigos para verificar o caminho de vitoria rapidamente; nao substitui uma partida humana completa. As regras de combate permanecem cobertas pelas 27 verificacoes existentes.

O movimento automatizado fornece direcao por MovementSource e passa pelo mesmo Rigidbody2D e colisores. A avaliacao humana de WASD/setas, conforto da camera e orientacao continua sendo o proximo teste. A compilacao Windows e a abertura do mapa foram conferidas; o percurso completo foi validado no Play Mode.

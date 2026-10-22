# GAME_01

JRPG para PC, com futura adaptacao para celular. A apresentacao evolui de personagens de palito e cenarios simples para pixel art e 3D ao longo da historia.

## Ambiente

- Unity 6.5, versao exata 6000.5.6f1.
- C# e ScriptableObjects para regras e dados.
- Unity UI (uGUI) 2.5.0, fornecido pelo Editor instalado.
- Repositorio: https://github.com/engmoacyrprado/GAME_01

## Jogar o percurso de exploracao

Abra Builds/Adventure/GAME_01.exe. Use WASD ou setas para mover e E para interagir perto do objetivo. O percurso conecta exploracao, batalha, retorno ao mapa e uma mudanca visual simplificada entre duas eras. O combate mantem os valores aprovados pelo jogador.

No Unity, abra Assets/Scenes/ExplorationPrototype.unity. O menu GAME_01 > Adventure > Open Exploration Prototype cria a cena caso falte; Build PC Vertical Slice gera a compilacao com as duas cenas.

Roteiro e limites: docs/exploration-prototype.md. O progresso existe somente durante a execucao; ainda nao ha salvamento em disco.

## Testar o combate isolado

Executavel local gerado: Builds/PC/GAME_01.exe. Mantenha os arquivos e subpastas de Builds/PC juntos. Essa pasta e ignorada pelo Git; o menu GAME_01 > Combat > Build PC Prototype gera uma nova compilacao.

1. Abra esta pasta pelo Unity Hub com o Editor 6000.5.6f1.
2. Espere a importacao e a compilacao terminarem.
3. Abra Assets/Scenes/BattlePrototype.unity. Se a cena ainda nao existir, use GAME_01 > Combat > Open Playable Prototype.
4. Pressione Play e clique em um aliado, no cartao ou no boneco, para abrir seus poderes.
5. Prepare uma acao e o alvo para a proxima vez desse aliado. Defender, Esperar e Passar Turno nao exigem alvo inimigo.
6. Use Reiniciar batalha para restaurar os participantes e comecar novamente.

A cena cria sua interface ao entrar em Play. Nao e preciso conectar botoes pelo Inspector.

O modo de teste aguarda Iniciar teste e permite Pausar/Continuar. O roteiro de tres rodadas esta em docs/battle-playtest-guide.md; as medicoes estao em docs/reports/battle-pacing-2026-10-22.md.

Ajuste apos playtest: decisao de 10 segundos, escala ATB 1.8 e apresentacao de 0.55s por acao. E possivel preparar acoes antes de iniciar, entre turnos e durante a pausa de teste. O cartao mostra PROX; Limpar proxima acao cancela o plano. O plano e usado uma vez, validado na execucao e nao gasta recursos antecipadamente.

## O que existe

- Area de exploracao com movimento, colisao, camera, interacao e objetivos.
- Encontro conectado a batalha e retorno ao mapa com desbloqueio apos vitoria.
- Primeira mudanca visual simplificada entre duas eras e conclusao do percurso.
- Batalha 2x2 com Heroi, Mago, Lobo e Bandido como dados provisorios.
- ATB individual, prioridade dinamica, pausa durante a decisao e timeout que passa o turno.
- Ataque basico, golpe pesado, magia, defesa, espera e passagem de turno.
- Selecao de alvo, HP, MP, ATB, contador, registro de acoes e resultado.
- Validacao de recursos/alvos, vitoria, derrota, empate e reinicio.

As formulas e os valores sao de prototipo. A influencia de Tagmar ainda nao representa uma implementacao completa de suas regras.

## Documentos

- docs/project-review-2026-10-22.md: revisao, correcoes e limites.
- docs/combat-system-tagmar-jrpg.md: regras atuais de combate.
- docs/unity-prototype-setup.md: execucao e verificacao.
- docs/unity-combat-architecture.md: organizacao tecnica atual.
- docs/game-concept.md e docs/mini-gdd.md: conceito e propostas de narrativa.
- docs/chatgpt-share-import.md: registro historico da conversa compartilhada.

A historia pode continuar no ChatGPT Web. Decisoes aprovadas devem ser registradas em docs/ para orientar a programacao. A conversa externa nao possui sincronizacao automatica com este repositorio.

## Escopo seguinte

Validar o percurso completo e a leitura da mudanca visual; depois definir a primeira area com a historia aprovada e aprofundar a apresentacao. Inventario, itens, fuga, progressao de personagem, regras completas de status, salvamento e versao mobile ficam para etapas posteriores.

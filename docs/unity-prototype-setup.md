# Setup do prototipo Unity

## Abrir e jogar

Para testar fora do Editor, execute Builds/PC/GAME_01.exe. Para recompilar, use GAME_01 > Combat > Build PC Prototype. O executavel abre em janela 1280x720 e permite redimensionamento. A pasta completa Builds/PC deve ser mantida junta.

Engine fixada em Unity 6000.5.6f1. Abra a pasta do repositorio no Unity Hub e aguarde a compilacao. Abra Assets/Scenes/BattlePrototype.unity e pressione Play.

Se a cena nao existir, execute GAME_01 > Combat > Open Playable Prototype. O gerador cria os dados faltantes, a cena, a camera, o bootstrap e registra a cena nas Build Settings. Dados e cenas existentes sao preservados; para editar valores, use os assets no Inspector.

O Canvas e os controles sao criados em runtime, sem ligacoes manuais. Use uma janela Game em formato paisagem, preferencialmente 1280x720 ou 16:9. A interface deste prototipo tem foco em PC; retrato/mobile ainda exige adaptacao.

## Controles e ciclo

1. Clique em um aliado e prepare sua proxima acao; isso funciona antes e durante a batalha, inclusive na pausa de teste.
2. As barras ATB avancam. Na vez do aliado, um plano valido executa automaticamente uma vez; sem plano, as barras pausam e abre a decisao de 10 segundos.
3. Clique em uma acao. Para ataques e magia, clique depois no cartao do inimigo.
4. Durante a selecao do alvo, o mesmo contador continua correndo.
5. Ao confirmar, a acao aplica dano, recurso e custo de tempo.
6. Se o contador terminar, o aliado passa o turno sem atacar nem gastar MP.
7. Cada acao tem 0.55s de apresentacao com ATB parado. Depois as barras voltam a avancar; o proximo participante pronto age.
8. A batalha termina quando um dos grupos nao possui sobreviventes. Reiniciar restaura a batalha.

O prototipo nao possui rodadas globais nem multiplayer. Um jogador controla os aliados; os inimigos escolhem automaticamente a primeira acao utilizavel.

Atualizacao de playtest: a batalha aguarda Iniciar teste. Pausar teste congela barras e contador; Continuar teste retoma. Reiniciar restaura a sessao e volta a aguardar o inicio. A pausa manual e uma ferramenta de avaliacao do prototipo.

## Configuracao

- Assets/GameData/Combat/Configs/BattleConfig_Prototype.asset: limiar 100, escala de tempo 1.8, decisao 10 segundos, pausa ativa e apresentacao de 0.55s por acao.
- Assets/GameData/Combat/Actions/: seis acoes de demonstracao.
- Assets/GameData/Combat/Combatants/: atributos e acoes de cada participante.

Custos acima da barra criam divida de recuperacao. A barra visual fica vazia enquanto o texto mostra a recuperacao restante. Esperar conserva no maximo metade do limiar configurado. Defender reduz danos pela metade ate o inicio da proxima vez do defensor.

## Validacao automatizada

No Editor, execute GAME_01 > Combat > Validate Combat Rules. A rotina verifica cenarios de regressao e uma batalha 2x2 completa; falhas geram excecao.

Para automacao, execute Unity.exe com os argumentos abaixo, substituindo PROJETO pelo caminho absoluto desta pasta:

    -batchmode -nographics -quit -projectPath "PROJETO" -executeMethod Game01.Combat.Editor.CombatValidation.Run -logFile "PROJETO/Logs/validation.log"

Feche o Editor deste projeto antes de executar outra instancia por linha de comando.

Game01.Combat.Editor.CombatPlayModeValidation.Run valida a integracao da cena em Play Mode. Essa rotina deve ser executada em batchmode sem -quit e sem -nographics: ela entra em Play, aciona botoes, verifica dano/reinicio e encerra o Editor com codigo 0 ou 1. Tambem captura Logs/prototype-review.png.

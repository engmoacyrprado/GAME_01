# Arquitetura de combate - implementacao atual

## Ambiente e separacao

Unity 6000.5.6f1, C#, uGUI 2.5.0. A dependencia da UI e declarada no manifest e resolvida no packages-lock.

- Data/: ScriptableObjects editaveis para participantes, acoes, status e configuracao.
- Runtime/BattleSession: cria os participantes e a maquina de estados; recebe o tempo por Tick. Pode ser executada sem uma cena.
- Runtime/BattleContext: estado da batalha, sistemas, selecao validada e registro das ultimas acoes.
- Runtime/CombatantRuntime: HP, MP, ATB, velocidade e estado de defesa de cada participante.
- Systems/: crescimento de ATB, ordem, alvos, dano, contador e condicao de fim.
- StateMachine/: controla a sequencia de decisao e resolucao.
- Debug/BattleDebugBootstrap: conecta os assets e a sessao a Unity; reinicia criando uma sessao nova.
- UI/BattleHUDController: recebe escolhas e atualiza a apresentacao.
- UI/PrototypeBattleView: monta a interface provisoria e os personagens de palito com uGUI.
- Editor/: geracao de assets/cena e verificacoes de regras e Play Mode.

As regras usam alguns tipos Unity, como Mathf e ScriptableObject; portanto ainda nao constituem uma biblioteca independente da engine.

## Fluxo

StartCombat -> CheckEnd -> TimelineUpdate -> ActorReady -> DecisionPhase -> ResolveAction -> Aftermath -> CheckEnd.

CheckEnd retorna para TimelineUpdate ou entra em BattleEnded, um estado terminal registrado. ResolveAction aplica os efeitos uma vez e conserva a fase durante actionPresentationDuration (0.55s no prototipo), com ATB parado. PresentedActor, PresentedTarget e PresentationTime orientam os movimentos geometricos provisorios. Animacoes finais ainda precisam de integracao de arte e eventos.

BattleSession.Tick recebe tempo nao escalado pelo bootstrap. A pausa e controlada por TimelinePaused, sem alterar Time.timeScale. Assim o contador continua correndo enquanto as barras estao pausadas.

## Entrada e regras

PlanPlayerAction recebe aliado, acao e alvo mesmo fora de DecisionPhase. O contexto armazena um CombatActionCommand por aliado. Na vez dele, BeginPlayerDecision inicia o contador e TryCommitPlannedAction remove/revalida o plano antes de confirmar. Um plano invalido deixa os 10s completos para nova escolha. A selecao direta na vez atual continua usando SetPlayerSelection.

O HUD separa aliado inspecionado de CurrentActor. Os cartoes e bonecos de aliados abrem o menu; o alvo inimigo conclui o plano. O cartao mostra PROX e o botao Limpar proxima acao cancela apenas o plano inspecionado. Reinicio e fim de batalha descartam os planos.

BattleContext.SetPlayerSelection devolve true somente quando existe uma decisao de aliado aberta, o contador esta ativo, a acao pertence ao personagem, os recursos sao suficientes e o alvo e valido. Uma segunda confirmacao no mesmo turno e rejeitada.

Ataques, tecnicas de dano e magia suportam um inimigo. Acoes Self implementadas: defender, esperar e passar. Itens, fuga e alvos em area permanecem indisponiveis, mesmo que sejam inseridos nos assets. Nao sao convertidos silenciosamente em ataques.

A interface guarda uma acao pendente do aliado inspecionado enquanto o jogador escolhe um alvo; isso nao reinicia o contador do participante atual. A troca explicita de aliado ou o reinicio descarta essa selecao incompleta. O plano completo permanece no contexto ate a proxima vez do aliado, cancelamento ou fim da batalha.

## Apresentacao e extensoes

A interface atual e de prototipo. A previsao de ordem estima tempo restante a partir do ATB e da velocidade atuais; nao representa uma sequencia garantida de turnos futuros.

Alguns tipos do esqueleto anterior, como BattleSnapshot, CombatActionRequest e componentes de Presentation, nao participam da cena atual. Nao representam funcionalidades entregues. A sessao e os assets serao preservados quando substituirmos a apresentacao por sprites, animacoes e depois 3D.

Nao estao implementados: eventos de animacao, persistencia, duracao/ticks de status, inventario, fuga, efeitos em area, cura e IA tatica. O bloqueio de acao e os modificadores de velocidade possuem suporte basico; o ciclo completo de status ainda depende de regras de design.

## Verificacao

CombatValidation executa cenarios deterministas diretamente na sessao. CombatPlayModeValidation verifica a integracao entre bootstrap, Canvas, acao, alvo, dano e reinicio no Editor em Play Mode. Ambas falham por excecao ou codigo de saida diferente de zero.

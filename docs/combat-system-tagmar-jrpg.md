# Combate ATB - regras do prototipo

## Principios confirmados

JRPG para um jogador, com aliados controlados pelo jogador e inimigos automaticos. Nao existem rodadas fixas. A ordem depende do tempo de cada participante.

1. Cada participante acumula ATB conforme velocidade e tempo decorrido.
2. Ao atingir o limiar, participa da fila de prontidao.
3. Prioridade: maior ATB, maior velocidade, maior Habilidade e ID estavel.
4. Na vez de um aliado, executa o plano preparado se ele ainda for valido. Sem plano valido, o ATB pausa por padrao e abre uma decisao de 10 segundos.
5. O jogador escolhe a acao e, quando necessario, o alvo dentro do mesmo prazo.
6. Ao confirmar, a acao resolve e consome tempo/MP. Depois a linha de tempo continua.
7. Ao expirar, o turno passa sem ataque e sem gasto de MP, mesmo sem asset Passar Turno.
8. Quando um grupo fica sem sobreviventes, a batalha encerra. Ambos derrotados resulta em empate.

Nao existe reset global de barras entre turnos. Cada participante mantem seu estado ate o fim da batalha. Reiniciar cria uma batalha nova, com HP/MP cheios e ATB zerado.

## Velocidade

    SpeedBase = HAB + bonus_classe + bonus_equipamento
    SpeedPenalty = penalidade_armadura + penalidade_peso + penalidades_status
    SpeedFinal = max(1, SpeedBase - SpeedPenalty) * multiplicadores_status
    ATB += SpeedFinal * TickScale * deltaTime

Limiar inicial: 100. Escala inicial: 1.8. Derrotados nao acumulam ATB. Um multiplicador zero pode impedir o crescimento da barra. Modificacoes em status exigem recalcular os atributos; duracao automatica ainda nao existe.

## Custos de tempo

    ATB = ATB - TimeCost

Revisao tecnica de 2026-10-22: ATB negativo representa divida de recuperacao. O rascunho anterior zerava o negativo, fazendo um golpe de 150 custar o mesmo que um de 100 quando a barra estava em 100. Agora, por exemplo, o golpe de 150 deixa a barra em -50; o personagem precisa recuperar 150 ate voltar a 100. Esta e uma escolha de balanceamento do prototipo, sujeita a teste jogavel.

- Ataque basico: 100 tempo, 0 MP.
- Golpe pesado: 150 tempo, 0 MP, maior poder.
- Faisca: 120 tempo, 8 MP, usa poder magico.
- Defender: 80 tempo, 0 MP; reduz todos os danos pela metade ate o inicio da proxima vez do defensor.
- Esperar: conserva no maximo 50% do limiar configurado, sem gastar MP.
- Passar Turno: 100 tempo, 0 MP.
- Timeout ou bloqueio de acao: consome um limiar de tempo.

## Dano provisoriamente determinista

    Dano fisico = max(1, ataque + poder_da_acao - defesa_do_alvo)
    Dano magico = max(1, poder_magico + poder_da_acao - defesa_do_alvo)
    Dano contra defesa = dano * 0.5

Nao ha erros de acerto, criticos, resistencias, tabela Tagmar, cura ou status por ataque nesta entrega. A formula e para testar ritmo; nao e uma adaptacao completa das regras de mesa.

## Planejamento entre turnos

Clique em um aliado para ver seus poderes mesmo fora da vez dele. Uma acao e alvo validos podem ser preparados, substituidos ou removidos. O plano nao gasta MP nem ATB antecipadamente e executa uma unica vez ao atingir prontidao. Alvo morto ou recurso insuficiente invalida o plano e abre uma decisao completa de 10s. Preparar outro aliado nao altera o turno nem o contador do atual.

ResolveAction tem um intervalo configuravel de apresentacao (0.55s no prototipo), com barras paradas e efeitos aplicados uma vez. A preparacao de comandos continua disponivel durante esse intervalo. O movimento de espera nao depende do crescimento do ATB.

## Validacao de acoes

Ataques e magia exigem inimigo vivo da batalha atual. MP insuficiente impede confirmar e desabilita o botao. Acoes nao possuidas ou nao implementadas sao rejeitadas.

Itens, fuga, acoes em area e selecao de aliado sao previstos no modelo, mas permanecem indisponiveis. Inimigos usam a primeira acao utilizavel de sua lista. A pausa pode ser desligada no BattleConfig: as barras crescem durante a decisao, mas as acoes continuam sendo resolvidas uma por vez.

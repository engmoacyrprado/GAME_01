# Sistema de Combate - Tagmar JRPG

## Resumo

Sistema de combate por `turnos nao fixos`, baseado em `ATB`, com influencia de atributos e penalidades inspiradas em `Tagmar`.

O objetivo e entregar:

- ritmo continuo
- estrategia tatica
- identidade propria
- traducao de regras de mesa para experiencia digital fluida

## Principios

- o combate nao e por rodada fixa
- cada combatente possui sua propria barra de tempo
- a ordem de combate muda dinamicamente
- o jogador tem tempo limitado para decidir
- se nao agir, o turno e consumido
- acoes diferentes possuem custos de tempo diferentes

## Recursos por combatente

- `HP`
- `MP/Aura`
- `ATB`
- `SpeedBase`
- `SpeedPenalty`
- `SpeedFinal`
- `status`
- `peso`
- `armadura`

## Formula base sugerida

```txt
SpeedBase = HAB + bonus_classe + bonus_equip
SpeedPenalty = penalidade_armadura + penalidade_peso + status_penalty
SpeedFinal = max(1, SpeedBase - SpeedPenalty)
SpeedFinal *= status_multiplier
```

## Crescimento da barra

```txt
ATB += SpeedFinal * TickScale
```

Quando:

```txt
ATB >= ATB_THRESHOLD
```

O combatente entra na `ReadyQueue`.

Parametros iniciais sugeridos:

- `ATB_THRESHOLD = 100`
- `TickScale` ajustavel em testes

## Prioridade da fila de prontidao

Quando mais de um combatente fica pronto ao mesmo tempo:

1. maior `ATB`
2. maior `SpeedFinal`
3. maior atributo de desempate, como `HAB`
4. desempate aleatorio estavel

## Estados principais do combate

- `StartCombat`
- `TimelineUpdate`
- `ActorReady`
- `DecisionPhase`
- `ResolveAction`
- `Aftermath`
- `CheckEnd`
- `NextCycle`

## Fase de decisao

Quando um personagem do jogador fica pronto:

```txt
TimelinePause = true
```

Entao:

- abrir menu de acao
- iniciar `DecisionTimeLimit`
- aguardar escolha

Se o tempo expirar:

- executar acao padrao
- decisao atual: `Passar turno`

## Tipos de acao

- `AtaqueBasico`
- `Tecnica`
- `Magia`
- `Item`
- `Defender`
- `Fugir`
- `Esperar`

## Custos de tempo iniciais

- ataque rapido: `90`
- ataque basico: `100`
- ataque pesado: `130`
- defender: `80`
- item: `110`
- magia simples: `120`
- magia alta ou ancestral: `160-220`

Aplicacao:

```txt
ATB = ATB - TimeCost
if (ATB < 0) ATB = 0
```

## Acao Esperar / Hold

Mantem parte da barra para uso tatico.

```txt
ATB = clamp(ATB, 0, ATB_THRESHOLD * 0.5)
```

## Camada de resolucao

O projeto pode seguir por dois caminhos:

- `Tabela invisivel`: mais fiel ao espirito de Tagmar
- `Formula continua`: mais padrao JRPG

Interface sugerida:

```txt
CombatResolver
ResolveHit(attacker, target, action) -> HitResult
```

## Recomendacao para o prototipo

Para a primeira entrega no Unity:

- montar a maquina de estados de combate
- implementar 2 personagens aliados e 2 inimigos
- incluir 4 a 6 acoes
- testar pausa no menu
- testar expiracao de turno
- validar se o ritmo fica divertido antes de aprofundar formulas de dano

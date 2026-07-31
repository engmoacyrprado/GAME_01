# Importacao da Conversa Compartilhada

## Origem

Base importada da conversa compartilhada:

- [ChatGPT Share - Tagmar para JRPG](https://chatgpt.com/share/6a6cd757-4ae4-83e9-b574-70d314d62b21)

Data de leitura no Codex:

- `2026-07-31`

## O que a conversa define com clareza

- o projeto esta sendo tratado como um `JRPG`
- a referencia sistemica e `Tagmar`
- o foco principal atual esta no `sistema de combate`
- o modelo de combate desejado e `ATB tatico`
- se o jogador nao agir, `o turno deve passar automaticamente`
- a recomendacao da conversa foi `pausar o tempo quando o menu de acao abre`

## Elementos importados

### Base do combate

- combate por turnos nao fixos
- ordem dinamica baseada em velocidade
- barra ATB por combatente
- fila de prontidao
- custo de tempo diferente por acao
- leitura estrategica da ordem de combate

### Influencia de Tagmar

- `Habilidade` influencia velocidade
- `Aura` e recurso central para magia
- `peso` e `armadura` afetam desempenho
- `status` alteram ritmo, custo ou disponibilidade de acoes

### Comportamento do turno

- ao encher a barra, o personagem entra na fila de prontidao
- quando for um personagem do jogador, o jogo abre o menu de acao
- o tempo deve pausar, por padrao, durante a decisao
- se o tempo de decisao expirar, a acao padrao e `passar turno`

### Acoes previstas

- ataque basico
- habilidade ou tecnica
- magia
- item
- defender
- fugir
- esperar/hold

## Tensao de design identificada

Existe uma tensao positiva entre duas ideias do projeto:

- `JRPG Tagmar`
- `evolucao visual por eras`

A conversa compartilhada aprofunda muito o `combate`, mas ainda nao fecha como a evolucao visual entra no gameplay e na narrativa.

## Decisao provisoria registrada

A base autoritativa do projeto neste momento passa a ser:

1. `JRPG com sistema Tagmar digital`
2. `foco inicial em combate`
3. `evolucao visual` permanece como diferencial criativo a ser integrado nas proximas etapas

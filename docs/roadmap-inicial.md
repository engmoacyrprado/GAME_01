# Roadmap Inicial

## Situacao em 2026-10-22

Fundacao e prototipo de combate foram revisados. Existe uma cena 2x2 com seis acoes, selecao de alvo, contador, barras e reinicio. Compilacao, regras e integracao em Play Mode foram verificadas. O detalhamento esta em project-review-2026-10-22.md.

O jogador aprovou o balanceamento apos testar decisao de 10s, escala ATB 1.8 e planos por aliado. Esses valores passam a ser a referencia desta etapa.

Novo marco implementado: percurso de exploracao conectado a batalha, retorno ao mapa, desbloqueio de marco e mudanca visual simplificada entre duas eras. O roteiro de teste e os limites estao em exploration-prototype.md. As fases abaixo continuam como planejamento de produto; nao representam uma lista de funcionalidades ja implementadas.

## Fase 1 - Fundacao do projeto

- consolidar o conceito de `JRPG Tagmar`
- definir o protagonista e o tom da historia
- definir quantas eras existirao na versao 1
- escolher a versao do `Unity`
- definir formato do primeiro combate jogavel
- definir se a primeira entrega sera visualmente simples

## Fase 2 - Pre-producao

- escrever um mini GDD
- desenhar o loop principal de exploracao + combate
- listar mecanicas fixas e mecanicas que evoluem
- definir referencia visual de cada era
- desenhar uma transicao jogavel entre duas eras
- traduzir as regras de Tagmar em sistemas digitais simplificados

## Fase 3 - Prototipo jogavel

- criar estrutura de combatente
- implementar barra ATB
- implementar fila de prontidao
- implementar menu de acao com timer de decisao
- criar uma batalha curta 1x2 ou 3x3
- validar a passagem automatica de turno

## Fase 4 - Vertical slice

- entregar uma experiencia curta mas completa
- incluir narrativa basica
- incluir UI minima de combate
- incluir ao menos uma evolucao visual clara
- validar se o sistema Tagmar digital continua divertido alem da novidade visual

## Decisoes recomendadas para agora

- priorizar `PC` como plataforma principal
- usar `Unity 6.5 (6000.5.6f1)`
- comecar pelo `sistema de combate`
- deixar `mobile` para fase posterior
- limitar a primeira versao a `1 combate vertical slice + 1 ideia de evolucao visual`

## Entregavel ideal da primeira milestone

Uma demo curta com:

- tela de combate funcional
- personagens com atributos base
- barra de tempo funcionando
- menu de acoes com limite de decisao
- custo de tempo por acao
- base pronta para evoluir para exploracao e narrativa

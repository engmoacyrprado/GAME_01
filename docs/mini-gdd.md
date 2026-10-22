# Mini GDD - GAME_01

> Revisao em 2026-10-22: o conceito de PC, evolucao visual e combate ATB foi mantido. Protagonista, mundo, tom e nomes das eras abaixo sao propostas de design, ainda dependentes das decisoes da historia. O estado implementado e os testes estao em project-review-2026-10-22.md.

## Resumo

`GAME_01` e um `JRPG` para `PC`, com futura adaptacao para `mobile`, inspirado na ambientacao e na logica sistemica de `Tagmar`.

O diferencial do projeto e combinar:

- combate tatico em estilo `ATB`
- progressao de personagem, tecnicas, magia e equipamento
- evolucao visual gradual ao longo da historia

O jogo deve comecar com representacao visual muito simples e evoluir por eras cada vez mais sofisticadas, ate atingir uma apresentacao 3D mais rica e cinematografica.

## Visao do jogador

O jogador deve sentir que:

- esta atravessando a historia de um mundo em transformacao
- cada nova fase representa um salto tecnologico, cultural ou perceptivo
- o combate fica mais profundo sem perder legibilidade
- a evolucao visual e uma recompensa narrativa e sistemica

## Fantasia central

O jogador controla um protagonista ligado a um mundo que esta "despertando" ou "reconstruindo" suas eras perdidas.

Cada marco da jornada revela:

- novas formas de enxergar o mundo
- novas camadas de memoria, magia e tecnologia
- novas maneiras de representar personagens, cidades, criaturas e batalhas

## Estrutura de gameplay

O loop principal do jogo e:

1. explorar area, cidade, ruina ou regiao
2. encontrar NPCs, inimigos, desafios e eventos narrativos
3. entrar em combate com sistema ATB Tagmar JRPG
4. ganhar recursos, experiencia, equipamento ou tecnicas
5. avancar na historia e desbloquear nova era visual

## Estrutura de combate

O combate usa `turnos nao fixos`, com ordem dinamica baseada em tempo.

### Regras-base

- cada combatente possui uma barra `ATB`
- a barra cresce de acordo com `SpeedFinal`
- ao atingir o limiar, o combatente entra na fila de prontidao
- quando um personagem do jogador fica pronto, o tempo pausa para decisao
- se o jogador nao agir dentro do limite, o turno e consumido

### Variaveis principais

- `HP`
- `MP/Aura`
- `ATB`
- `Habilidade`
- `peso`
- `armadura`
- `status`

### Tipos de acao iniciais

- ataque basico
- tecnica
- magia
- item
- defender
- esperar
- fugir

## Adaptacao de Tagmar

O jogo nao tenta portar Tagmar de mesa literalmente. A direcao e fazer uma `adaptacao digital`.

### Elementos preservados

- relevancia de `Habilidade`
- uso de `Aura` como recurso magico
- impacto de `peso` e `armadura`
- influencia de `status` e especializacao

### Elementos simplificados

- tabelas complexas podem virar formulas internas
- parte das rolagens pode ser traduzida em resolucao automatica
- numeros precisam ser legiveis no ritmo de JRPG

## Eras visuais

### Era 1 - Primitive

- visual extremamente simples
- personagens quase abstratos
- cenarios minimos
- UI reduzida

### Era 2 - Retro

- maior clareza visual
- mais animacao
- mapas maiores
- leitura de combate mais rica

### Era 3 - Pixel

- pixel art definida
- cidades e dungeons mais detalhadas
- maior expressao narrativa

### Era 4 - Early 3D

- ganho de profundidade
- cenarios mais volumetricos
- apresentacao mais dramatica

### Era 5 - High Fidelity

- 3D mais detalhado
- animacoes mais refinadas
- fechamento narrativo com o auge da evolucao

## Protagonista

Direcao provisoria:

- personagem ligado a memoria, legado ou restauracao do mundo
- funciona como testemunha e agente da evolucao
- deve justificar mecanicamente porque atravessa eras diferentes

## Tom narrativo

- fantasia epica
- descoberta arqueologica e espiritual
- nostalgia dos videogames como linguagem visual
- crescimento progressivo de escala

## Primeira entrega jogavel

O primeiro vertical slice deve conter:

- uma batalha funcional
- 2 personagens aliados
- 2 inimigos
- 4 a 6 acoes disponiveis
- UI basica de combate
- uma identidade visual inicial simples
- um indicio claro de que o mundo pode evoluir para a proxima era

## Riscos principais

- escopo alto por causa de multiplas identidades visuais
- excesso de fidelidade a Tagmar prejudicando o ritmo digital
- transicao ruim entre eras
- combate interessante no papel, mas lento na pratica

## Decisoes atuais

- foco principal em `PC`
- engine alvo: `Unity 6.5 (6000.5.6f1)`
- combate como primeiro sistema a prototipar
- mobile fica para adaptacao posterior
- `evolucao visual` continua como diferencial central do projeto

## Proximos passos recomendados

1. definir protagonista, nome do mundo e conflito principal
2. desenhar a arquitetura tecnica do combate no Unity
3. prototipar a maquina de estados de batalha
4. decidir como a Era 1 se conecta ao primeiro combate jogavel

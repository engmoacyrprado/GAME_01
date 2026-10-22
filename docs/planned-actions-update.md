# Ajuste de combate apos o playtest

## Pedido do jogador

Decisao de 10 segundos, espera um pouco mais lenta e acesso aos poderes ao clicar em um personagem mesmo fora da vez dele. A preparacao deve deixar o combate dinamico e considerar movimentos e personagens visualmente diferentes.

## Resultado

- BattleConfig usa escala 1.8 (10% menos crescimento de ATB) e decisao de 10s.
- Um plano por aliado, substituivel e cancelavel, com acao e alvo. Nao reserva nem gasta MP/ATB antecipadamente.
- Plano valido executa uma vez na proxima prontidao. Plano invalido e descartado e abre 10s completos para decidir, sem troca automatica de alvo.
- Durante a decisao de um personagem, e possivel preparar outro; o contador do atual continua correndo.
- Cartoes mostram PROX, o HUD distingue VEZ DE do aliado inspecionado, e clicar no boneco ou no cartao abre seus poderes.
- Cada acao tem 0.55s de apresentacao provisoria com ATB parado. Movimento de espera continua durante decisoes. A pausa de teste congela tambem os movimentos.
- Heroi, Mago, Lobo e Bandido possuem cores, silhuetas e acessorios geometricos diferentes. Arte e animacoes finais continuam pendentes.

## Verificacao

27 verificacoes de regras aprovadas. Play Mode aprovado para preparar os dois aliados pela UI, executar na ordem correta, gastar MP apenas na vez do Mago, pausar/continuar, confirmar uma acao atual e reiniciar sem planos antigos.

Compilacao Windows aprovada. No executavel, cliques reais no boneco do Mago abriram seu menu; Faisca foi preparada para o Bandido sem alterar HP/MP e depois cancelada por Limpar proxima acao.

19 perfis de ritmo no relatorio battle-pacing-2026-10-22.md, em reports/. Atual: basico com decisao de 1s leva 48.8s; pesado + magia com decisao de 1s leva 28.0s; planos automaticos ideais de pesado/magia levam 21.9s. Os perfis automaticos nao representam o tempo humano de leitura e escolha.

O ganho de dinamismo vem de usar o tempo entre turnos para planejar. O intervalo visual desacelera a apresentacao sem acelerar barras ou consumir recursos duas vezes. O valor de 0.55s ainda precisa ser julgado pelo jogador e ajustado quando existirem animacoes finais.

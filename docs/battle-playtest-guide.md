# Teste jogavel: planejamento entre turnos

## Ajustes aprovados apos o primeiro teste

- Prazo de decisao: 10 segundos.
- Barras 10% mais lentas: escala de 2 para 1.8.
- Acao e alvo podem ser preparados antes da vez do aliado.
- Personagens devem ser diferentes e se movimentar; o ritmo deve incluir a apresentacao das acoes.

## Como jogar esta versao

1. Abra Builds/PC/GAME_01.exe. A batalha aguarda Iniciar teste.
2. Clique no cartao ou no boneco do Heroi. Escolha Golpe Pesado e clique no Lobo.
3. Clique no Mago. Escolha Faisca e clique no Bandido.
4. Confira PROX nos cartoes e clique em Iniciar teste.
5. Os planos executam uma vez quando chega a vez de cada aliado. Enquanto as barras avancam, prepare novas acoes.
6. Para substituir um plano, escolha outra acao e alvo. Limpar proxima acao cancela o plano do aliado selecionado.

Voce tambem pode preparar acoes durante a pausa manual de teste. Isso nao aplica efeitos nem gasta MP antes da execucao.

O campo VEZ DE indica quem esta aguardando uma decisao; a linha de baixo indica de quem sao os poderes abertos. Inspecionar outro aliado nao reinicia o contador do personagem atual. Se ele ainda estiver esperando, os 10 segundos continuam passando.

Se o plano tiver alvo morto, recurso insuficiente ou acao indisponivel na hora de executar, ele e descartado e abre uma decisao nova de 10 segundos. O sistema nao muda o alvo silenciosamente. Planos nao se repetem para sempre e nao sobrevivem ao reinicio da batalha.

Defender, Esperar e Passar Turno nao exigem alvo inimigo. Esperar conserva no maximo metade do limiar de ATB; nao e uma acao de tempo gratuito.

## Movimento e personagens

A apresentacao atual usa figuras geometricas provisorias: Heroi com espada/armadura, Mago com chapeu/cajado, Lobo quadrupede e Bandido com mascara/adaga. Ha movimento de espera, deslocamento curto do atacante, destaque no alvo e figuras apagadas ao serem derrotadas.

Cada acao reserva 0.55s com ATB parado. O movimento de espera continua enquanto o jogador decide, mesmo com as barras pausadas. A pausa manual de teste congela tambem a apresentacao. Esses movimentos servem ao teste de ritmo; nao substituem animacoes finais, sprites, rig ou arte definitiva.

## O que avaliar agora

- Os 10 segundos sao confortaveis quando voce precisa decidir na hora?
- Preparar ambos os aliados enquanto as barras avancam ficou claro?
- O cartao PROX mostra a acao e o alvo que voce esperava?
- O intervalo de apresentacao permite perceber os ataques sem parecer demorado?
- A troca entre selecionar um aliado e mirar em um inimigo e natural?

## Medicoes

O relatorio reports/battle-pacing-2026-10-22.md preserva os perfis anteriores e inclui os tempos atuais. Perfis de planos automaticos ideais nao representam o tempo humano necessario para ler e escolher. Os resultados anteriores de 35.3s (basico) e 20.4s (misto) foram obtidos antes deste ajuste, sem intervalo de apresentacao.

Os cliques reais no executavel usam o botao principal configurado no Windows. No computador do teste, os botoes estao invertidos; a configuracao foi preservada.

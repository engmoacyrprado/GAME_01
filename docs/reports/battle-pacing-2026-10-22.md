# Ritmo da batalha - medicao de 2026-10-22

Simulacao determinista da BattleSession com os assets reais da cena 2x2. Tempo total inclui espera ATB e demora artificial de decisao; nao mede FPS, latencia do computador ou tempo de leitura humano. Perfis sem prefixo Atual preservam a medicao anterior, com apresentacao de duracao zero. Perfis Atual usam os novos tempos e intervalo visual. Cada perfil e uma execucao determinista, nao uma amostra estatistica.

Alvo padrao: primeiro inimigo vivo (Lobo, depois Bandido). IA: primeira acao utilizavel, primeiro aliado vivo. Magia/pesado usam ataque basico como fallback quando indisponiveis. Defesa alternada afeta apenas o Heroi; Mago ataca. Baseline: escala 2, limiar 100, decisao 6s, pausa ativa. Atual: escala 1.8, decisao 10s e apresentacao 0.55s. Planos automaticos ideais escolhem imediatamente pesado/magia e alvo vivo; nao medem o tempo de planejamento humano.

| Perfil | Resultado | Total (s) | ATB ativo (s) | Decisao (s) | Apresentacao (s) | Acoes aliadas | Golpes inimigos | Timeouts | HP restante | MP gasto |
|---|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| Basico / decisao 1s | Vitoria | 35.3 | 23.1 | 12.2 | 0.0 | 12 | 8 | 0 | 83 / 200 | 0 |
| Basico / decisao 3s | Vitoria | 59.3 | 23.1 | 36.2 | 0.0 | 12 | 8 | 0 | 83 / 200 | 0 |
| Basico / decisao 5.5s | Vitoria | 89.1 | 23.1 | 66.0 | 0.0 | 12 | 8 | 0 | 83 / 200 | 0 |
| Pesado / decisao 1s | Vitoria | 26.8 | 19.6 | 7.1 | 0.0 | 7 | 6 | 0 | 112 / 200 | 0 |
| Magia / decisao 1s | Vitoria | 22.8 | 15.7 | 7.1 | 0.0 | 7 | 5 | 0 | 127 / 200 | 40 |
| Heroi pesado + Mago magia / 1s | Vitoria | 20.4 | 14.3 | 6.1 | 0.0 | 6 | 5 | 0 | 127 / 200 | 24 |
| Heroi pesado + Mago magia / 3s | Vitoria | 32.4 | 14.3 | 18.1 | 0.0 | 6 | 5 | 0 | 127 / 200 | 24 |
| Defesa alternada do Heroi / 1s | Vitoria | 48.1 | 30.8 | 17.3 | 0.0 | 17 | 9 | 0 | 90 / 200 | 0 |
| Somente esperar / 1s | Derrota | 50.9 | 28.6 | 22.4 | 0.0 | 22 | 14 | 0 | 0 / 200 | 0 |
| Somente passar / 1s | Derrota | 40.8 | 28.6 | 12.2 | 0.0 | 12 | 14 | 0 | 0 / 200 | 0 |
| Sem responder / timeout 6s | Derrota | 100.6 | 28.6 | 72.0 | 0.0 | 0 | 14 | 12 | 0 / 200 | 0 |
| Basico / decisao 7s / limite 10s | Vitoria | 107.1 | 23.1 | 84.0 | 0.0 | 12 | 8 | 0 | 83 / 200 | 0 |
| Basico / 3s / escala 2.5 | Vitoria | 54.7 | 18.5 | 36.2 | 0.0 | 12 | 8 | 0 | 83 / 200 | 0 |
| Basico / 3s / escala 3 | Vitoria | 51.6 | 15.4 | 36.2 | 0.0 | 12 | 8 | 0 | 83 / 200 | 0 |
| Basico / priorizar Bandido / 1s | Vitoria | 39.1 | 26.9 | 12.2 | 0.0 | 12 | 10 | 0 | 64 / 200 | 0 |
| Basico / 1s / simulacao 30 FPS | Vitoria | 35.1 | 23.1 | 12.0 | 0.0 | 12 | 8 | 0 | 83 / 200 | 0 |
| Atual: basico / decisao 1s | Vitoria | 48.8 | 25.6 | 12.2 | 11.0 | 12 | 8 | 0 | 83 / 200 | 0 |
| Atual: pesado + magia / decisao 1s | Vitoria | 28.0 | 15.9 | 6.1 | 6.1 | 6 | 5 | 0 | 127 / 200 | 24 |
| Atual: planos automaticos ideais | Vitoria | 21.9 | 15.9 | 0.0 | 6.1 | 6 | 5 | 0 | 127 / 200 | 24 |

## Primeira abertura de decisao

No perfil basico de 1s: Heroi em 3.60s e Mago em 4.87s de tempo total simulado. O Mago depende da demora do Heroi porque as barras ficam pausadas durante a decisao. O Lobo tem velocidade 14, igual ao Heroi; desempata por Habilidade (14 contra 12) e ataca primeiro.

## Recuperacao teorica entre acoes

Sem pausas de outros participantes, sem excesso inicial de ATB e com escala 2.

| Acao | Heroi (velocidade 14) | Mago (velocidade 13) |
|---|---:|---:|
| Ataque basico | 3.57s | 3.85s |
| Golpe pesado | 5.36s | 5.77s |
| Magia | 4.29s | 4.62s |
| Defender | 2.86s | 3.08s |
| Esperar (50%) | 1.79s | 1.92s |

## Leitura dos resultados

- Com pausa, demorar para decidir alonga o tempo real, mas nao da ataques extras aos inimigos antes do timeout.
- Esperar/defender sao ferramentas de apoio. So esperar ou passar nao causa dano e leva a derrota.
- A IA concentra dano sempre no primeiro aliado vivo; estes resultados nao representam IA que troca de alvo.
- A nova apresentacao separa acoes por 0.55s com ATB parado. Os movimentos sao provisorios.
- Dano e duracao nao equivalem a diversao. Conforto do prazo e clareza precisam da avaliacao do jogador.

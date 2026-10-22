# Dialogos e respostas

## Apresentacao proposta

A fala aparece em uma legenda larga na parte inferior. O nome do falante e um balao simbolico com reticencias, acima do personagem, indicam quem fala. Heroi e Mago possuem cores distintas na legenda e no balao. Narrador usa apenas a legenda.

O jogador avanca pelo botao Continuar, E, Espaco ou Enter. Respostas aparecem em botoes e aceitam tambem 1, 2 ou 3. Conversas nao possuem contador; o prazo de 10s pertence somente ao combate.

O movimento e as interacoes com o mapa ficam bloqueados durante a abertura e a conversa. Um comando que encerra uma fala nao pode acionar outra interacao no mesmo frame. O cenario continua visivel e os movimentos de espera continuam funcionando.

## Dados editaveis

Assets/GameData/Narrative/Introduction.asset e um DialogueGraph. Cada node possui id unico, speaker, text, nextNodeId e ate tres choices. Cada resposta possui id, label e nextNodeId. Um proximo id vazio encerra a conversa.

DialogueSession valida ids e links, controla o node atual e exige uma resposta nos nodes de escolha. NarrativePresenter monta a UI e segue a posicao do falante. AdventureRun registra as respostas e se a introducao terminou; esses dados persistem entre cenas, mas nao em disco.

O gerador cria o roteiro apenas quando o asset nao existe. Nao sobrescreve textos ou respostas editados. Alteracoes na estrutura dos ids ou ramificacoes da demonstracao podem exigir atualizar os testes de conteudo.

## Limites desta entrega

As duas respostas da primeira conversa pedem mais contexto ou seguem adiante; ambas chegam a mesma missao. Ainda nao existem reputacao, finais alternativos, testes de atributo, condicoes para liberar respostas, dublagem ou salvamento narrativo.

A introducao e provisoria. O formato permite testar a leitura, a associacao entre balao e personagem e a escolha de respostas enquanto a historia definitiva e desenvolvida separadamente.

## Verificacao

DialogueValidation verifica abertura, troca de falante, bloqueio do avanco em escolhas, indices invalidos, ramificacoes, registro da resposta, convergencia explicita, encerramento, repeticao, links quebrados, ids duplicados e limpeza do progresso.

O teste integrado verifica bloqueio do movimento durante conversa, escolha registrada, liberacao da exploracao e ausencia de repeticao da introducao ao voltar da batalha. Capturas de abertura, fala e respostas ficam em Logs/story-*.png, fora do Git.

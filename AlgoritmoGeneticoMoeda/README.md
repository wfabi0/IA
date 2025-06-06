# Algoritmo Genético para o Problema do Troco

Este projeto é uma implementação de um Algoritmo Genético (AG) simples, desenvolvido em C#, com o objetivo de encontrar a solução otimizada para o problema do troco.

## O que é um Algoritmo Genético?

Algoritmos Genéticos são técnicas de busca e otimização inspiradas nos princípios da evolução natural e da genética de Charles Darwin. Eles operam sobre uma população de indivíduos (soluções candidatas), aplicando operadores genéticos como seleção, crossover (recombinação) e mutação ao longo de várias gerações para encontrar soluções progressivamente melhores.

## Como Funciona

- Problema: Encontrar a combinação de moedas de R$ 0.20, R$ 0.11, R$ 0.05 e R$ 0.01 que some um valor exato definido pelo usuário, usando o menor número de moedas possível.

- Indivíduo (Cromossomo): Cada solução é representada por uma string de 20 bits, que codifica a quantidade de cada uma das quatro moedas.

- Aptidão (Fitness): Cada indivíduo recebe uma pontuação de aptidão. A nota é menor (e melhor) para soluções que acertam o valor alvo e utilizam poucas moedas.
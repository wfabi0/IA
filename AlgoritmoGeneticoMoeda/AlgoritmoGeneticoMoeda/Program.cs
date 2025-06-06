using System;
using System.Globalization;

namespace AlgoritmoGeneticoMoeda
{
    class Program
    {
        static readonly Random rand = new Random();
        static readonly double[] VALORES_MOEDAS = { 0.20, 0.11, 0.05, 0.01 };

        const int BITS_POR_MOEDA = 5;
        const int TAMANHO_CROMOSSOMO = 20;

        static double VALOR_ALVO = 0.0;

        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = new CultureInfo("pt-BR", false);

            int populationSize = 0;
            while (populationSize < 2 || (populationSize % 2) != 0)
            {
                try
                {
                    Console.Write("Insira o tamanho da sua população (número par >= 2): ");
                    populationSize = Convert.ToInt32(Console.ReadLine());

                    if (populationSize < 2 || (populationSize % 2) != 0)
                    {
                        Console.WriteLine("População inválida. Pressione ENTER para tentar novamente.");
                        Console.ReadKey();
                        Console.Clear();
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Entrada inválida. Pressione ENTER para tentar novamente.");
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            while (VALOR_ALVO <= 0)
            {
                try
                {
                    Console.Write("Valor do troco a ser encontrado (ex: 0,78): ");

                    string input = Console.ReadLine();

                    VALOR_ALVO = Convert.ToDouble(input);

                    if (VALOR_ALVO <= 0)
                    {
                        Console.WriteLine("Por favor, insira um valor positivo.");
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Valor inválido. Tente novamente.");
                    VALOR_ALVO = 0;
                }
            }


            string[] population = new string[populationSize];
            for (int i = 0; i < populationSize; i++)
            {
                population[i] = GerarCromossomoAleatorio(TAMANHO_CROMOSSOMO);
            }

            Console.WriteLine($"\nPopulação inicial com {populationSize} cromossomos de {TAMANHO_CROMOSSOMO} bits.");
            Console.WriteLine($"Buscando a combinação de moedas para o valor alvo de {VALOR_ALVO:C}\n");

            int maxGenerations = 1000;
            string bestCromossomoOverall = population[0];
            double bestFitnessOverall = EvaluateCromossomo(bestCromossomoOverall);

            for (int generation = 0; generation < maxGenerations; generation++)
            {
                string[] newPopulation = new string[populationSize];

                for (int i = 0; i < populationSize / 2; i++)
                {
                    string pai = population[2 * i];
                    string mae = population[2 * i + 1];

                    string[] filhos = CrossingOver(pai, mae);
                    newPopulation[2 * i] = filhos[0];
                    newPopulation[2 * i + 1] = filhos[1];

                    double fitnessFilho1 = EvaluateCromossomo(filhos[0]);
                    if (fitnessFilho1 < bestFitnessOverall)
                    {
                        bestFitnessOverall = fitnessFilho1;
                        bestCromossomoOverall = filhos[0];
                    }

                    double fitnessFilho2 = EvaluateCromossomo(filhos[1]);
                    if (fitnessFilho2 < bestFitnessOverall)
                    {
                        bestFitnessOverall = fitnessFilho2;
                        bestCromossomoOverall = filhos[1];
                    }
                }

                population = newPopulation;

                for (int i = 0; i < population.Length; i++)
                {
                    if (rand.NextDouble() <= 0.20)
                    {
                        string cromossomoMutado = Mutate(population[i]);
                        population[i] = cromossomoMutado;

                        double fitnessMutado = EvaluateCromossomo(cromossomoMutado);
                        if (fitnessMutado < bestFitnessOverall)
                        {
                            bestFitnessOverall = fitnessMutado;
                            bestCromossomoOverall = cromossomoMutado;
                        }
                    }
                }

                if ((generation + 1) % 100 == 0)
                {
                    Console.WriteLine($"Geração {generation + 1}: Melhor Aptidão (Fitness) = {bestFitnessOverall:F2}");
                }
            }

            Console.WriteLine("\n--- MELHOR SOLUÇÃO ENCONTRADA ---");
            Console.WriteLine($"Cromossomo: {bestCromossomoOverall}");

            int[] bestCounts = DecodeChromosome(bestCromossomoOverall);
            double finalValue = 0;
            int totalCoins = 0;

            Console.WriteLine("Detalhes da Solução:");
            for (int i = 0; i < VALORES_MOEDAS.Length; i++)
            {
                Console.WriteLine($"- Moeda de {VALORES_MOEDAS[i]:C}: {bestCounts[i]} unidades");
                finalValue += bestCounts[i] * VALORES_MOEDAS[i];
                totalCoins += bestCounts[i];
            }

            Console.WriteLine($"\nValor Total: {Math.Round(finalValue, 2):C}");
            Console.WriteLine($"Número Total de Moedas: {totalCoins}");
            Console.WriteLine($"Aptidão Final (Fitness): {bestFitnessOverall:F2}\n");
        }

        // Fitness (Função para ver o quao perto esta da melhor resposta)
        static double EvaluateCromossomo(string cromossomo)
        {
            int[] coinCounts = DecodeChromosome(cromossomo);

            double currentValue = 0;
            int numberOfCoins = 0;

            for (int i = 0; i < coinCounts.Length; i++)
            {
                currentValue += coinCounts[i] * VALORES_MOEDAS[i];
                numberOfCoins += coinCounts[i];
            }

            currentValue = Math.Round(currentValue, 2);

            double valueDifference = Math.Abs(VALOR_ALVO - currentValue);

            double fitness = (valueDifference * 1000) + numberOfCoins;

            return fitness;
        }

        static int[] DecodeChromosome(string chromosome)
        {
            if (chromosome.Length != TAMANHO_CROMOSSOMO)
                throw new ArgumentException("Cromossomo com tamanho incorreto.");

            int[] counts = new int[VALORES_MOEDAS.Length];
            for (int i = 0; i < VALORES_MOEDAS.Length; i++)
            {
                string binaryPart = chromosome.Substring(i * BITS_POR_MOEDA, BITS_POR_MOEDA);
                counts[i] = Convert.ToInt32(binaryPart, 2);
            }
            return counts;
        }

        static string Mutate(string binary)
        {
            char[] bits = binary.ToCharArray();
            int position = rand.Next(binary.Length);
            bits[position] = bits[position] == '0' ? '1' : '0';
            return new string(bits);
        }

        static string GerarCromossomoAleatorio(int length)
        {
            char[] cromossomo = new char[length];
            for (int i = 0; i < length; i++)
            {
                cromossomo[i] = rand.Next(2) == 0 ? '0' : '1';
            }
            return new string(cromossomo);
        }

        static string[] CrossingOver(string pai, string mae)
        {
            int pontoCorte = rand.Next(1, pai.Length);

            string partePaiInicio = pai.Substring(0, pontoCorte);
            string partePaiFim = pai.Substring(pontoCorte);

            string parteMaeInicio = mae.Substring(0, pontoCorte);
            string parteMaeFim = mae.Substring(pontoCorte);

            string filho1 = partePaiInicio + parteMaeFim;
            string filho2 = parteMaeInicio + partePaiFim;

            return new string[] { filho1, filho2 };
        }
    }
}
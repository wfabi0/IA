using System;

namespace AlgoritmoGeneticoComPopulacao
{
    internal class Program
    {

        static Random rand = new Random();

        static void Main(string[] args)
        {
            int populationSize = 0;
            while (populationSize < 2 || (populationSize % 2) != 0)
            {
                try
                {
                    Console.Write("Insira o tamanho da sua população: ");
                    populationSize = Convert.ToInt32(Console.ReadLine());

                    if (populationSize < 1 || (populationSize % 2) != 0)
                    {
                        Console.WriteLine("Você inseriu uma população inválida. Tente novamente pressionando ENTER.");
                        Console.ReadKey();
                        Console.Clear();
                    }
                }
                catch (Exception e) {}
            }

            if (populationSize <= 0)
            {
                Console.WriteLine("Você inseriu uma população inválida.");
                return;
            }

            string[] population = new string[populationSize];
            for (int i = 0; i < populationSize; i++)
            {
                population[i] = randomCromossomo(10);
            }

            // apos gerar a população preciso fazer a recombinação
            // metade pai, metade mae

            Console.WriteLine($"População inicial: {populationSize} cromossomos");

            string bestCromossomoOverall = population[0];
            double bestResultOverall = EvaluateCromossomo(bestCromossomoOverall);

            for (int i = 1; i < populationSize; i++)
            {
                double result = EvaluateCromossomo(population[i]);
                if (result < bestResultOverall)
                {
                    bestCromossomoOverall = population[i];
                    bestResultOverall = result;
                }
            }
            Console.WriteLine($"Melhor da população inicial: {bestCromossomoOverall} | {bestResultOverall}");

            // Crossing-Over

            string[] newPopulation = new string[populationSize];

            for (int i = 0; i < populationSize / 2; i++)
            {
                string daddy = population[2 * i];
                string mommy = population[2 * i + 1];

                string[] newChildren = crossingOver(daddy, mommy);
                newPopulation[2 * i] = newChildren[0];
                newPopulation[2 * i + 1] = newChildren[1];
            }
            Console.WriteLine("Filhos gerados através do crossover, população atualizada.");

            population = newPopulation;

            // depois disso verificar se os novos filhos sao melhores
            // e ai sim fazer mutação

            Console.WriteLine("Avaliando os filhos e procurando melhor cromossomo..");
            for (int i = 1; i < population.Length; i++)
            {
                double result = EvaluateCromossomo(population[i]);
                if (result < bestResultOverall)
                {
                    bestCromossomoOverall = population[i];
                    bestResultOverall = result;
                }
            }

            Console.WriteLine($"Melhor filho encontrado: {bestCromossomoOverall} | {bestResultOverall}\n");

            // Mutação
            for (int i = 1; i <= 10000; i++)
            {
                for (int j = 0; j < population.Length; j++)
                {
                    // Mutação
                    string cromossomo = population[j];
                    int pos = rand.Next(cromossomo.Length);
                    string mutated = Mutate(cromossomo, pos);

                    double resultMutated = EvaluateCromossomo(mutated);

                    if (resultMutated < bestResultOverall)
                    {
                        bestCromossomoOverall = mutated;
                        bestResultOverall = resultMutated;
                    }
                }

                if ((i + 1) % 1000 == 0)
                {
                    Console.WriteLine($"Iteração {i + 1}: Melhor cromossomo encontrado até o momento: {bestCromossomoOverall} | {bestResultOverall}");
                }
            }

            string[] bestSplit = separete(bestCromossomoOverall);
            double finalX1 = toDecimal(bestSplit[0]);
            double finalX2 = toDecimal(bestSplit[1]);

            Console.WriteLine($"\nMelhor cromossomo: {bestCromossomoOverall}");
            Console.WriteLine($"x1: {finalX1} | x2: {finalX2}");
            Console.WriteLine($"Melhor f(x): {bestResultOverall}\n");
        }

        static double EvaluateCromossomo(string cromossomo)
        {
            string[] split = separete(cromossomo);
            double x1 = toDecimal(split[0]);
            double x2 = toDecimal(split[1]);
            double result = Function(x1, x2);

            if ((x1 + x2) < 4)
            {
                return 1000.0;
            }
            return result;
        }

        static string Mutate(string binary, int position)
        {
            char[] bits = binary.ToCharArray();

            bits[position] = bits[position] == '0' ? '1' : '0';

            return new string(bits);
        }

        static double Function(double x1, double x2)
        {
            double f = 0.25 * Math.Pow(x1, 4.0) - 3.0 * Math.Pow(x1, 3.0) + 11.0 * Math.Pow(x1, 2.0) - 13.0 * x1 + 0.25 * Math.Pow(x2, 4.0) - 3.0 * Math.Pow(x2, 3.0) + 11.0 * Math.Pow(x2, 2.0) - 13.0 * x2;
            return f;
        }

        static double X(string x)
        {
            return Convert.ToInt32(x, 2) / 31.0 * 6.0;
        }


        static string randomCromossomo(int length = 10)
        {
            char[] cromossomo = new char[length];
            for (int i = 0; i < length; i++)
            {
                cromossomo[i] = rand.Next(2) == 0 ? '0' : '1';
            }
            return new string(cromossomo);
        }

        static string[] separete(string cromossomo)
        {
            if (cromossomo.Length != 10) throw new ArgumentException("Erro");
            return new string[] { cromossomo.Substring(0, 5), cromossomo.Substring(5, 5) };
        }

        static double toDecimal(string binary)
        {
            int value = Convert.ToInt32(binary, 2);
            return value / 31.0 * 6.0;
        }

        static string[] crossingOver(string daddy, string mommy)
        {
            int splitter = rand.Next(1, daddy.Length);

            string daddyEnd = daddy.Substring(splitter);
            string mommyEnd = mommy.Substring(splitter);

            string daddyStart = daddy.Substring(0, splitter);
            string mommyStart = mommy.Substring(0, splitter);

            string newDaddy = daddyStart + mommyEnd;
            string newMommy = mommyStart + daddyEnd;

            return new string[] { newDaddy, newMommy };
        }

    }
}

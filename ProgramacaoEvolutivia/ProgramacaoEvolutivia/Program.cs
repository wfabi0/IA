using System;

namespace ProgramacaoEvolutivia
{
    internal class Program
    {

        static Random rand = new Random();

        static void Main(string[] args)
        {

            Console.Write("Insira o tamanho da sua população: ");
            int populationSize = Convert.ToInt32(Console.ReadLine());
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

            Console.WriteLine($"Gerados: {populationSize} cromossomos");

            string bestCromossomo = population[0];
            string[] split = separete(bestCromossomo);
            double x1 = toDecimal(split[0]);
            double x2 = toDecimal(split[1]);
            double bestResult = Function(x1, x2);

            if ((x1 + x2) < 4)
            {
                bestResult = 1000;
            }

            for (int i = 1; i <= 10000; i++)
            {
                for (int j = 0; j < population.Length; j++)
                {

                    string cromossomo = population[j];
                    int pos = rand.Next(cromossomo.Length);
                    string mutated = Mutate(cromossomo, pos);

                    string[] genes = separete(mutated);
                    double newX1 = toDecimal(genes[0]);
                    double newX2 = toDecimal(genes[1]);
                    double result = Function(newX1, newX2);

                    if (result < bestResult)
                    {
                        bestResult = result;
                        bestCromossomo = mutated;
                    }

                }
            }

            string[] bestSplit = separete(bestCromossomo);
            double finalX1 = toDecimal(bestSplit[0]);
            double finalX2 = toDecimal(bestSplit[1]);

            Console.WriteLine($"\nMelhor cromossomo: {bestCromossomo}");
            Console.WriteLine($"x1: {finalX1} | x2: {finalX2}");
            Console.WriteLine($"Melhor f(x): {bestResult}");
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
            return new string[] { cromossomo.Substring(0, 5), cromossomo.Substring(5, 5)  };
        }

        static double toDecimal(string binary)
        {
            int value = Convert.ToInt32(binary, 2);  
            return value / 31.0 * 6.0;               
        }
    }
}

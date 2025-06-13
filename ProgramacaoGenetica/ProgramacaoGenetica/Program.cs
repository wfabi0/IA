using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ProgramaGeneticoSingleFile
{
    private static readonly Random _random = new Random();

    private abstract class Node
    {
        public abstract double Evaluate(double x);
        public abstract Node Clone();
        public abstract override string ToString();
    }

    private class ConstantNode : Node
    {
        public double Value { get; set; }

        public ConstantNode(double value) { Value = value; }
        public override double Evaluate(double x) => Value;
        public override Node Clone() => new ConstantNode(Value);
        public override string ToString() => Value.ToString("F2");
    }

    private class VariableNode : Node
    {
        public override double Evaluate(double x) => x;
        public override Node Clone() => new VariableNode();
        public override string ToString() => "x";
    }

    private class OperatorNode : Node
    {
        public char Operator { get; }
        public Node Left { get; set; }
        public Node Right { get; set; }

        public OperatorNode(char op, Node left, Node right)
        {
            Operator = op;
            Left = left;
            Right = right;
        }

        public override double Evaluate(double x)
        {
            double leftVal = Left.Evaluate(x);
            double rightVal = Right.Evaluate(x);

            switch (Operator)
            {
                case '+': return leftVal + rightVal;
                case '-': return leftVal - rightVal;
                case '*': return leftVal * rightVal;
                case '/': return rightVal == 0 ? 1 : leftVal / rightVal;
                default: throw new InvalidOperationException($"Operador desconhecido: {Operator}");
            }
        }

        public override Node Clone() => new OperatorNode(Operator, Left.Clone(), Right.Clone());
        public override string ToString() => $"({Left} {Operator} {Right})";
    }

    private class ExpressionTree
    {
        public Node Root { get; set; }
        public double Fitness { get; private set; }

        private static readonly char[] Operators = { '+', '-', '*', '/' };

        public ExpressionTree(Node root)
        {
            Root = root;
            Fitness = double.MaxValue;
        }

        public double Evaluate(double x) => Root.Evaluate(x);

        public void CalculateFitness(List<double> inputs, List<double> expectedOutputs)
        {
            double totalError = 0;
            for (int i = 0; i < inputs.Count; i++)
            {
                double result = Evaluate(inputs[i]);
                totalError += Math.Pow(result - expectedOutputs[i], 2);
            }
            Fitness = totalError;
        }

        public ExpressionTree Clone() => new ExpressionTree(Root.Clone());

        public static ExpressionTree CreateRandomTree(int maxDepth)
        {
            return new ExpressionTree(CreateRandomNode(0, maxDepth));
        }

        private static Node CreateRandomNode(int currentDepth, int maxDepth)
        {
            if (currentDepth >= maxDepth || _random.NextDouble() < 0.4)
            {
                return _random.NextDouble() < 0.5 ? (Node)new VariableNode() : new ConstantNode(_random.Next(-5, 6));
            }
            else
            {
                char op = Operators[_random.Next(Operators.Length)];
                Node left = CreateRandomNode(currentDepth + 1, maxDepth);
                Node right = CreateRandomNode(currentDepth + 1, maxDepth);
                return new OperatorNode(op, left, right);
            }
        }

        public List<Node> GetAllNodes()
        {
            var nodes = new List<Node>();
            CollectNodes(Root, nodes);
            return nodes;
        }

        private void CollectNodes(Node node, List<Node> nodes)
        {
            if (node == null) return;
            nodes.Add(node);
            if (node is OperatorNode opNode)
            {
                CollectNodes(opNode.Left, nodes);
                CollectNodes(opNode.Right, nodes);
            }
        }
    }

    private class GeneticProgrammingLogic
    {
        public int PopulationSize { get; } = 1000;
        public int MaxGenerations { get; } = 200;
        public double CrossoverRate { get; } = 0.8;
        public double MutationRate { get; } = 0.15;
        public int TournamentSize { get; } = 5;
        public int MaxTreeDepth { get; } = 4;
        public double TargetFitness { get; } = 0.01;

        private List<ExpressionTree> _population;
        private readonly List<double> _inputs;
        private readonly List<double> _expectedOutputs;

        public GeneticProgrammingLogic(List<double> inputs, List<double> expectedOutputs)
        {
            _inputs = inputs;
            _expectedOutputs = expectedOutputs;
        }

        public ExpressionTree Run()
        {
            InitializePopulation();
            for (int gen = 0; gen < MaxGenerations; gen++)
            {
                EvaluatePopulation();
                var bestOfGen = _population.OrderBy(t => t.Fitness).First();
                Console.WriteLine($"Geração {gen}: Melhor Fitness = {bestOfGen.Fitness:F4}, Função = {bestOfGen.Root}");

                if (bestOfGen.Fitness <= TargetFitness)
                {
                    Console.WriteLine("\nSolução encontrada!");
                    return bestOfGen;
                }
                EvolvePopulation();
            }
            Console.WriteLine("\nNúmero máximo de gerações atingido.");
            return _population.OrderBy(t => t.Fitness).First();
        }

        private void InitializePopulation()
        {
            _population = new List<ExpressionTree>(PopulationSize);
            for (int i = 0; i < PopulationSize; i++)
            {
                _population.Add(ExpressionTree.CreateRandomTree(MaxTreeDepth));
            }
        }

        private void EvaluatePopulation()
        {
            foreach (var tree in _population)
            {
                tree.CalculateFitness(_inputs, _expectedOutputs);
            }
        }

        private void EvolvePopulation()
        {
            var newPopulation = new List<ExpressionTree>(PopulationSize);
            newPopulation.Add(_population.OrderBy(t => t.Fitness).First().Clone());

            while (newPopulation.Count < PopulationSize)
            {
                var parent1 = TournamentSelection();
                if (_random.NextDouble() < CrossoverRate && newPopulation.Count < PopulationSize)
                {
                    var parent2 = TournamentSelection();
                    var child = Crossover(parent1, parent2);
                    newPopulation.Add(child);
                }
                else
                {
                    newPopulation.Add(parent1.Clone());
                }

                if (_random.NextDouble() < MutationRate)
                {
                    Mutate(newPopulation.Last());
                }
            }
            _population = newPopulation;
        }

        private ExpressionTree TournamentSelection()
        {
            ExpressionTree best = null;
            for (int i = 0; i < TournamentSize; i++)
            {
                var contender = _population[_random.Next(PopulationSize)];
                if (best == null || contender.Fitness < best.Fitness)
                {
                    best = contender;
                }
            }
            return best;
        }

        private ExpressionTree Crossover(ExpressionTree parent1, ExpressionTree parent2)
        {
            ExpressionTree child = parent1.Clone();
            ExpressionTree donor = parent2.Clone();

            List<Node> childNodes = child.GetAllNodes();
            List<Node> donorNodes = donor.GetAllNodes();

            if (childNodes.Count == 0 || donorNodes.Count == 0) return child;

            int childNodeIndex = _random.Next(childNodes.Count);
            Node nodeToReplace = childNodes[childNodeIndex];
            Node subtreeToInsert = donorNodes[_random.Next(donorNodes.Count)];

            if (childNodeIndex == 0)
            {
                child.Root = subtreeToInsert;
            }
            else
            {
                Node parent = FindParentNode(child.Root, nodeToReplace);
                if (parent is OperatorNode opParent)
                {
                    if (opParent.Left == nodeToReplace) opParent.Left = subtreeToInsert;
                    else opParent.Right = subtreeToInsert;
                }
            }
            return child;
        }

        private void Mutate(ExpressionTree tree)
        {
            List<Node> nodes = tree.GetAllNodes();
            if (nodes.Count == 0) return;

            int nodeIndex = _random.Next(nodes.Count);
            Node nodeToMutate = nodes[nodeIndex];

            Node newNode = ExpressionTree.CreateRandomTree(MaxTreeDepth - 2).Root;

            if (nodeIndex == 0)
            {
                tree.Root = newNode;
            }
            else
            {
                Node parent = FindParentNode(tree.Root, nodeToMutate);
                if (parent is OperatorNode opParent)
                {
                    if (opParent.Left == nodeToMutate) opParent.Left = newNode;
                    else opParent.Right = newNode;
                }
            }
        }

        private Node FindParentNode(Node current, Node target)
        {
            if (current is OperatorNode opNode)
            {
                if (opNode.Left == target || opNode.Right == target)
                {
                    return opNode;
                }
                var found = FindParentNode(opNode.Left, target);
                if (found != null) return found;
                return FindParentNode(opNode.Right, target);
            }
            return null;
        }
    }

    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("--- Evolução Automática de Programas com Genética (Versão Final Corrigida) ---");

        var inputs = new List<double> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var expectedOutputs = new List<double> { 6, 2, 0, 0, 2, 6, 12, 20, 30, 42, 56 };

        Console.WriteLine("\nDados de Treinamento:");
        for (int i = 0; i < inputs.Count; i++)
        {
            Console.WriteLine($"Entrada: {inputs[i]}, Saída Esperada: {expectedOutputs[i]}");
        }
        Console.WriteLine("\nIniciando processo evolutivo...");

        var gp = new GeneticProgrammingLogic(inputs, expectedOutputs);
        var bestSolution = gp.Run();

        Console.WriteLine("\n--- Processo Finalizado ---");
        Console.WriteLine($"Melhor Função Encontrada: {bestSolution.Root}");
        Console.WriteLine($"Fitness Final (Erro Total): {bestSolution.Fitness:F4}");

        Console.WriteLine("\nVerificando a performance da função encontrada:");
        for (int i = 0; i < inputs.Count; i++)
        {
            double calculatedOutput = bestSolution.Evaluate(inputs[i]);
            Console.WriteLine($"Para x = {inputs[i]}, f(x) = {calculatedOutput:F2} (Esperado: {expectedOutputs[i]})");
        }

        Console.WriteLine("\nPressione qualquer tecla para sair.");
        Console.ReadKey();
    }
}

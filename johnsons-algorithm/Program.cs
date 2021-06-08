using System;
using System.Collections.Generic;
using System.IO;

namespace johnsons_algorithm
{
    class Program
    {
        static void Main(string[] args) 
        {
            (int, int) sizeGraph = (0, 0);
            List<(int, int, double)> edgesList = Read(ref sizeGraph);
            bool checkMark = true;
            Console.WriteLine("Стартовая вершина");
            int start = Convert.ToInt32(Console.ReadLine());
            List<(int, double)> result = JohnsonAlgorithm(edgesList, sizeGraph, start);
            Console.WriteLine("1. До всех\n2. До одноу");
            switch (Console.ReadLine())
            {
                case "1":
                {
                    Console.WriteLine($"из вершини {start}");
                    for (int i = 0; i < result.Count - 1; i++)
                        Console.WriteLine($"в {result[i].Item1} = {result[i].Item2}");
                    break;
                }
                case "2":
                {
                    Console.WriteLine("Финишная вершина");
                    int finish = Convert.ToInt32(Console.ReadLine());
                    foreach (var item in result)
                    {
                        if (item.Item1 == finish)
                        {
                            Console.WriteLine($"из вершини {start} в {item.Item1} = {item.Item2}");
                        }
                    }
                    break;
                }
            }
        }
        static List<(int, int, double)> Read(ref (int, int) sizeGraph)
        { 
            List<(int, int, double)> list = new List<(int, int, double)>();
            StreamReader read = new StreamReader("test.txt");
            string[] size = read.ReadLine()?.Split(' ');
            if (size != null)
            {
                sizeGraph = (Convert.ToInt32(size[0]), Convert.ToInt32(size[1]));
                for (int i = 0; i < sizeGraph.Item2; ++i)
                {
                    size = read.ReadLine()?.Split(' ');
                    if (size != null)
                        list.Add((Convert.ToInt32(size[0]), Convert.ToInt32(size[1]), Convert.ToDouble(size[2])));
                }
            }
            return list;
        }
        static double[,] AdjMatrix((int, int) sizeMatrix, List<(int, int, double)> edgeList)
        {
            double[,] matrixA = new Double[sizeMatrix.Item1, sizeMatrix.Item1];
            for (int i = 0; i < sizeMatrix.Item1; i++)
            for (int j = 0; j < sizeMatrix.Item1; j++)
                if (i == j)
                    matrixA[i, j] = 0;
                else
                    matrixA[i, j] = double.PositiveInfinity;
            foreach (var item in edgeList)
                matrixA[item.Item1 - 1, item.Item2 - 1] = item.Item3;
            return matrixA;
        }
        static List<(int, double)> BellmanFordAlgorithm(List<(int, int, double)> edgesList, (int, int) sizeGraph, ref bool checkMark, int start)
        {
            List<(int, double)> list = new List<(int, double)>();
            for (int i = 0; i < sizeGraph.Item1; ++i)
                list.Add((i+1, Double.PositiveInfinity));
            list[start - 1] = (start, 0);
            for (int i = 0; i <= list.Count; ++i)
                foreach (var item in edgesList)
                {
                    double a = list[item.Item1 - 1].Item2 + item.Item3;
                    if (list[item.Item2 - 1].Item2 > a)
                    {
                        if (i == list.Count)
                        {
                            checkMark = false;
                            return list;   
                        }
                        list[item.Item2 - 1] = (list[item.Item2 - 1].Item1, list[item.Item1 - 1].Item2 + item.Item3);
                    }
                }
            return list;
        }

        static List<(int, double)> JohnsonAlgorithm(List<(int, int, double)> edgesList, (int, int) sizeGraph, int start)
        {
            List<(int, double)> timeList;
            bool checkMark = true;
            BellmanFordAlgorithm(edgesList, sizeGraph, ref checkMark, start);
            if (checkMark)
            {
                double[,] matrix = AdjMatrix(sizeGraph, edgesList);
                List<(int, int, double)> newEdgesList = AddEdge(edgesList, ref sizeGraph);
                List<(int, double)> bfA = BellmanFordAlgorithm(newEdgesList, sizeGraph, ref checkMark, sizeGraph.Item1);
                for (int i = 0; i < bfA.Count; i++)
                {
                    for (int j = 0; j < edgesList.Count; j++)
                    {
                        if (edgesList[j].Item1 ==  start && edgesList[j].Item2 == bfA[i].Item1)
                        {
                            edgesList[j] = (edgesList[j].Item1, edgesList[j].Item2, (edgesList[j].Item3 + bfA[edgesList[j].Item2].Item2 - bfA[edgesList[j].Item1].Item2));
                        }
                    }
                }
                timeList = DijkstraAlgorithm(matrix, sizeGraph, start);
                for (int i = 0; i < timeList.Count; i++)
                    for (int j = 0; j < edgesList.Count; j++)
                        if (edgesList[j].Item1 ==  start && edgesList[j].Item2 == timeList[i].Item1)
                            timeList[i] = (timeList[i].Item1, (edgesList[j].Item3 + bfA[edgesList[j].Item1].Item2 - bfA[edgesList[j].Item2].Item2));
                
            }
            else
            {
                throw new System.ArgumentException("Ошыбка!!! Есть отрицательные циклы");
            }
            return timeList;
        }
        
        static List<(int, double)> DijkstraAlgorithm(double[,] matrix, (int, int) sizeG, int start)
        {
            List<(int, double, bool)> list = new List<(int, double, bool)>();
            List<int> visitedEdge = new List<int>();
            List<int> preIndex = new List<int>();
            double sumValue = 0;
            visitedEdge.Add(start);
            for (int i = 0; i < sizeG.Item1; i++)
                list.Add((i + 1, Double.PositiveInfinity, false));
            list[start - 1] = (start, 0, true);
            double lengthMatrix = Math.Pow(matrix.Length, 0.5);
            for (int i = 0; i < lengthMatrix; i++)
                preIndex.Add(0);
            while (visitedEdge.Count != lengthMatrix)
            {
                for (int j = 0; j < lengthMatrix; j++)
                {
                    if (matrix[visitedEdge[visitedEdge.Count - 1] - 1,j] + sumValue < list[j].Item2)
                        preIndex[j] = visitedEdge[visitedEdge.Count - 1] - 1;
                    list[j] = (list[j].Item1, MinDuo(matrix[visitedEdge[visitedEdge.Count - 1] - 1,j] + sumValue, list[j].Item2), list[j].Item3);
                }
                visitedEdge.Add(Minimum(ref list));
                sumValue = list[visitedEdge[visitedEdge.Count - 1] - 1].Item2;
            }
            List<(int, double)> result = new List<(int, double)>();
            foreach (var item in list)
                result.Add((item.Item1, item.Item2));
            return result;
        }

        static List<(int, int, double)> AddEdge(List<(int, int, double)> edgesList, ref (int, int) sizeGraph)
        {
            List<(int, int, double)> list = new List<(int, int, double)>();
            foreach (var item in edgesList)
            {
                list.Add(item);
            }
            for (int i = 0; i < sizeGraph.Item1; i++)
            {
                list.Add((sizeGraph.Item1 + 1, i + 1, 0));
            }
            sizeGraph = (sizeGraph.Item1 + 1, sizeGraph.Item2);
            return list;
        }
        static int Minimum(ref List<(int, double, bool)> array)
        {
            double minimum = double.PositiveInfinity;
            int count = 0;
            for (int i = 0; i < array.Count; ++i)
                if (minimum > array[i].Item2 && !array[i].Item3)
                {
                    minimum = array[i].Item2;
                    count = i;
                }
            array[count] = (array[count].Item1, array[count].Item2, true);
            return count + 1;
        }
        static double MinDuo(double a, double b)
        {
            if (a < b)
                return a;
            return b;
        }
    }
}
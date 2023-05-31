using System;
using System.Collections.Generic;

namespace ALFE.TopOpt
{
    public class BFS
    {
        private int _numOfNodes;
        private List<int>[] _adjacencyList;

        public BFS(int numOfNodes, List<int>[] adjacencyList)
        {
            _numOfNodes = numOfNodes;
            _adjacencyList = adjacencyList;
        }

        public List<int> RunBFS(int startNode)
        {
            bool[] visited = new bool[_numOfNodes];
            for (int i = 0; i < _numOfNodes; i++)
            {
                visited[i] = false;
            }

            List<int> traversedNodes = new List<int>();
            Queue<int> queue = new Queue<int>();

            visited[startNode] = true;
            queue.Enqueue(startNode);

            while (queue.Count != 0)
            {
                startNode = queue.Dequeue();
                traversedNodes.Add(startNode);

                foreach (int node in _adjacencyList[startNode])
                {
                    if (!visited[node])
                    {
                        queue.Enqueue(node);
                        visited[node] = true;
                    }
                }
            }

            return traversedNodes;
        }
    }
}
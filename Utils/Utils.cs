using ALFE.FEModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE
{
    public class Utils
    {
        /// <summary>
        /// Compute all neighbours of each node.
        /// </summary>
        /// <param name="nodes"> A list of all nodes. </param>
        /// <param name="elements"> A list of all elements. </param>
        public static void ComputeNeighbours(List<Node> nodes, List<Element> elements)
        {
            foreach (var node in nodes)
                foreach (var item in node.ElementID)
                    foreach (var neighbour in elements[item].NodeID)
                        if (!nodes[neighbour].Active) 
                            node.Neighbours.Add(neighbour);
        }

        /// <summary>
        /// Return a scan dictionary <index, difference>
        /// </summary>
        /// <param name="nodesNum"></param>
        /// <param name="fixedNodesID"></param>
        /// <returns></returns>
        public static Dictionary<int, int[]> Scan(int nodesNum, List<int> fixedNodesID)
        {
            fixedNodesID.Sort();

            var scan = new Dictionary<int, int[]>();
            int t = 0, v = 0;

            for (int i = 0; i < nodesNum; i++)
            {
                for (int j = 0; j < fixedNodesID.Count - v; j++)
                {
                    if (i>= fixedNodesID[j + v])
                    {
                        t = j + v;
                        v += 1;
                        break;
                    }
                }
                if (i == fixedNodesID[t])
                    scan.Add(i, new int[2] { v, 0 });
                else
                    scan.Add(i, new int[2] { v, 1 });
            }
            return scan;
        }
    }
}

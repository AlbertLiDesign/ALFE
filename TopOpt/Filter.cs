using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALFE;
using System.Collections.Concurrent;
using Supercluster.KDTree;

namespace ALFE.TopOpt
{
    public class Filter
    {
        public float Rmin;
        public List<Element> Elements;
        
        /// <summary>
        /// The neighbour elements of each element.
        /// </summary>
        public List<int>[] Neighbours;

        /// <summary>
        /// The weight of the each neighbour element.
        /// </summary>
        public List<float>[] Weights;

        public int Dim;
        public Filter(List<Element> elems, float rmin, int dim)
        {
            Elements = elems;
            Rmin = rmin;
            Dim = dim;
        }

        public void PreFlt()
        {
            // Calculate element centre coordinates
            float[][] centres = new float[Elements.Count][];
            string[] nodes = new string[Elements.Count];

            // Get centres
            int id = 0;
            foreach (var elem in Elements)
            {
                Vector3D centre = new Vector3D();
                foreach (var node in elem.Nodes)
                {
                    centre += node.Position;
                }

                centre /= elem.Nodes.Count;

                centres[id] = new float[3] { centre.X, centre.Y, centre.Z };
                nodes[id] = centres[id].ToString();
                id++;
            }

            // Construct KDTree
            var tree = new KDTree<float, string>(3, centres, nodes, L2Norm_Squared_Float);

            for (int i = 0; i < centres.Length; i++)
            {
                // Get all points with in a distance of 100 from the target.
                var radialTest = tree.RadialSearch(centres[i], 100);
                
            }

            // Searching

            foreach (var elem in Elements)
            {

            }

        }

        // Define the distance function
        private static Func<float[], float[], double> L2Norm_Squared_Float = (x1, x2) =>
        {
            float dist = 0f;
            for (int i = 0; i < x1.Length; i++)
            {
                dist += (x1[i] - x2[i]) * (x1[i] - x2[i]);
            }

            return dist;
        };
    }
}

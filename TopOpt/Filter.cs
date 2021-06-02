using KDTree;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFE.TopOpt
{
    public class Filter
    {
        /// <summary>
        /// Filter Radius
        /// </summary>
        public double FilterRadius;

        /// <summary>
        /// Elements
        /// </summary>
        public List<Element> Elements;

        /// <summary>
        /// A mapping dictionary storing adjacent elements
        /// </summary>
        public Dictionary<Element, List<Element>> FME;

        /// <summary>
        /// A mapping dictionary storing the relevant weight factors 
        /// </summary>
        public Dictionary<Element, List<double>> FMW;

        /// <summary>
        /// The neighbour elements of each element.
        /// </summary>
        public List<int>[] Neighbours;

        /// <summary>
        /// The weight of the each neighbour element.
        /// </summary>
        public List<double>[] Weights;

        /// <summary>
        /// Dimension
        /// </summary>
        public int Dim;
        public Filter(List<Element> elems, double rmin, int dim)
        {
            Elements = elems;
            FilterRadius = rmin;
            Dim = dim;

            FME = new Dictionary<Element, List<Element>>(Elements.Count);
            FMW = new Dictionary<Element, List<double>>(Elements.Count);
        }

        /// <summary>
        /// Function of preparing filter map.
        /// </summary>
        public void PreFlt(bool kdtree = true)
        {
            if (kdtree)
            {
                // Calculate element centre coordinates
                List<Vector3D> centres = new List<Vector3D>(Elements.Count);

                // Construct KDTree
                var tree = new KDTree<int>(3);

                // Get centres
                int id = 0;
                foreach (var elem in Elements)
                {
                    Vector3D centre = new Vector3D();
                    foreach (var node in elem.Nodes)
                        centre += node.Position;

                    centre /= elem.Nodes.Count;

                    centres.Add(centre);
                    tree.AddPoint(new double[3] { centre.X, centre.Y, centre.Z }, id);
                    id++;
                }

                // Searching
                var result = KDTreeMultiSearch(centres, tree, FilterRadius, 1024);

                foreach (var elem in Elements)
                {
                    List<Element> adjacentElems = new List<Element>(result[elem.ID].Count);
                    List<double> weights = new List<double>(result[elem.ID].Count);
                    double sum = 0.0;

                    Vector3D curCentre = centres[elem.ID];

                    foreach (var item in result[elem.ID])
                    {
                        Vector3D adjCentre = new Vector3D(centres[item].X, centres[item].Y, centres[item].Z);

                        adjacentElems.Add(Elements[item]);
                        var weight = FilterRadius - curCentre.DistanceTo(adjCentre);
                        weights.Add(weight);
                        sum += weight;
                    }

                    // Compute weights
                    for (int i = 0; i < weights.Count; i++)
                        weights[i] /= sum;

                    FME.Add(elem, adjacentElems);
                    FMW.Add(elem, weights);
                }
            }
            else
            {
                foreach (var elem in Elements)
                {
                    List<Element> elems = new List<Element>();
                    List<double> weights = new List<double>();

                    var oneRingElems = GetNeighborElements(elem);
                    foreach (var oneElem in oneRingElems)
                    {
                        elems.Add(oneElem);
                        weights.Add(1);
                    }

                    FME.Add(elem, elems);
                    FMW.Add(elem, weights);
                }
            }
        }

        private List<Element> GetNeighborElements(Element elem)
        {
            List<Element> elems = new List<Element>();
            foreach (var node in elem.Nodes)
            {
                foreach (var elemID in node.ElementID)
                {
                    if (elemID != elem.ID)
                        elems.Add(Elements[elemID]);
                }
            }
            return elems;
        }

        private static List<int>[] KDTreeMultiSearch(List<Vector3D> pts, KDTree<int> tree, double radius, int maxReturned)
        {
            List<int>[] indices = new List<int>[pts.Count];
            Parallel.ForEach(Partitioner.Create(0, pts.Count, (int)Math.Ceiling(pts.Count / (double)Environment.ProcessorCount * 2.0)), delegate (Tuple<int, int> rng, ParallelLoopState loopState)
            {
                for (int i = rng.Item1; i < rng.Item2; i++)
                {
                    Vector3D point3d = pts[i];
                    double num = radius;
                    List<int> list = tree.NearestNeighbors(new double[]
                    {
                        point3d.X,
                        point3d.Y,
                        point3d.Z
                    }, maxReturned, num * num).ToList();
                    indices[i] = list;
                }
            });
            return indices;
        }
    }
}

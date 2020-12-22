using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALFE.FEModel;

namespace ALFE.FESystem
{
    public class System2D
    {
        /// <summary>
        ///  Finite element model
        /// </summary>
        public Model2D Model { get; set; }

        public CooMatrix KG;
        public double[] Rhs;
        public double[] X;
        public int dof = 2;
        public System2D(Model2D model, double[,] Ke)
        {
            Model = model;

            // Get the index of each node which has been anchored
            var ids = ApplySupports2D(Model.Nodes, Model.Supports);

            var scan = Utils.Scan(Model.Nodes.Count, ids);

            List<Triplet> triplets = new List<Triplet>();
            for (int i = 0; i < Model.Elements.Count; i++)
            {
                var nodeID = Model.Elements[i].NodeID;
                for (int I = 0; I < nodeID.Count; I++)
                {
                    for (int J = 0; J < nodeID.Count; J++)
                    {

                        var row0 = nodeID[I] * dof + 0 - scan[I] * dof;
                        var row1 = nodeID[I] * dof + 1 - scan[I] * dof;

                        var col0 = nodeID[J] * dof + 0 - scan[J] * dof;
                        var col1 = nodeID[J] * dof + 1 - scan[J] * dof;

                        triplets.Add(new Triplet(row0, col0, Ke[dof * I + 0, dof * J + 0]));
                        triplets.Add(new Triplet(row1, col0, Ke[dof * I + 1, dof * J + 0]));
                        triplets.Add(new Triplet(row0, col1, Ke[dof * I + 0, dof * J + 1]));
                        triplets.Add(new Triplet(row1, col1, Ke[dof * I + 1, dof * J + 1]));
                    }
                }
            }

            int shape = (Model.Nodes.Count - ids.Count) * dof;
            KG = new CooMatrix(triplets, shape, shape);
        }
        public static List<int> ApplySupports2D(List<Node2D> nodes, List<Support2D> supports)
        {
            List<int> ids = new List<int>(supports.Count);
            for (int i = 0; i < supports.Count; i++)
            {
                int id = supports[i].NodeID;
                ids.Add(id);
                nodes[id].Anchored = true;
                if (supports[i].Type == SupportType.Fixed)
                {
                    nodes[id].Displacement.X = 0.0;
                    nodes[id].Displacement.Y = 0.0;
                }
            }
            return ids;
        }


    }
}

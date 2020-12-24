using ALFE.FEModel;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace ALFE.FESystem
{
    public class System2D
    {
        public Model2D Model;
        public int Dim;
        public CooMatrix KG;
        public double[] F;
        public double[] X;
        public int Dof = 2;
        private List<int> FixedID;
        public System2D(Model2D model)
        {
            Model = model;

            // Get the index of each node which has been anchored
            FixedID = ApplySupports2D(Model.Nodes, Model.Supports);
            Dim = (Model.Nodes.Count - FixedID.Count) * Dof;

            F = new double[Dim * Dof];
            X = new double[Dim * Dof];
        }

        public void AssembleF()
        {
            for (int i = 0; i < Model.Loads.Count; i++)
            {
                F[Model.Loads[i].NodeID * Dof + 0] = Model.Loads[i].Load.X;
                F[Model.Loads[i].NodeID * Dof + 1] = Model.Loads[i].Load.Y;
            }
        }
        public void AssembleKG(double[,] Ke)
        {
            var scan = Utils.Scan(Model.Nodes.Count, FixedID);
            HashSet<Triplet> triplets = new HashSet<Triplet>(Dim);

            for (int i = 0; i < Model.Elements.Count; i++)
            {
                var nodeID = Model.Elements[i].NodeID;
                for (int I = 0; I < nodeID.Count; I++)
                    for (int J = 0; J <= I; J++)
                        if (scan[nodeID[I]][1] == 1 && scan[nodeID[J]][1] == 1)
                            for (int p = 0; p < Dof; p++)
                                for (int q = 0; q < Dof; q++)
                                {
                                    var row = (nodeID[I] - scan[nodeID[I]][0]) * Dof + p;
                                    var col = (nodeID[J] - scan[nodeID[J]][0]) * Dof + q;
                                    if (row >= col)
                                        triplets.Add(new Triplet(row, col, Ke[Dof * I + p, Dof * J + q]));
                                }
            }

            KG = new CooMatrix(triplets.ToList(), Dim, Dim);
        }
        public void Solve()
        {
            AssembleF();
            SolveFE(KG.RowArray, KG.ColArray, KG.ValueArray, F, Dim, Dof, KG.NNZ, X);
        }
        private static List<int> ApplySupports2D(List<Node2D> nodes, List<Support2D> supports)
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

        [DllImport("ALSolver.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SolveFE(int[] rowA, int[] colA, double[] valA, double[] F, int dim, int dof, int nnzA,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] double[] X);
        
    }
}

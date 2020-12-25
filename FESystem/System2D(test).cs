//using ALFE.FEModel;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices;
//using CSparse;
//using CSparse.float;
//using CSparse.Storage;
//using CSparse.float.Factorization.MKL;
//using CSparse.Interop.MKL.Pardiso;

//namespace ALFE.FESystem
//{
//    public class System2D
//    {
//        public Model2D Model;
//        public int Dim;
//        public SparseMatrix KG;
//        public int Dof = 2;
//        private List<int> FixedID;
//        public System2D(Model2D model)
//        {
//            Model = model;

//            // Get the index of each node which has been anchored
//            FixedID = ApplySupports2D(Model.Nodes, Model.Supports);
//            Dim = (Model.Nodes.Count - FixedID.Count) * Dof;
//        }

//        public void AssembleKG(float[,] Ke)
//        {
//            var scan = Utils.Scan(Model.Nodes.Count, FixedID);

//            List<int> rowIds = new List<int>();
//            List<int> colIds = new List<int>();
//            List<float> values = new List<float>();

//            for (int i = 0; i < Model.Elements.Count; i++)
//            {
//                var nodeID = Model.Elements[i].NodeID;
//                for (int I = 0; I < nodeID.Count; I++)
//                    for (int J = 0; J <= I; J++)
//                        if (scan[nodeID[I]][1] == 1 && scan[nodeID[J]][1] == 1)
//                            for (int p = 0; p < Dof; p++)
//                                for (int q = 0; q < Dof; q++)
//                                {
//                                    var row = (nodeID[I] - scan[nodeID[I]][0]) * Dof + p;
//                                    var col = (nodeID[J] - scan[nodeID[J]][0]) * Dof + q;
//                                    if (row >= col)
//                                    {
//                                        rowIds.Add(row);
//                                        colIds.Add(col);
//                                        values.Add(Ke[Dof * I + p, Dof * J + q]);
//                                    }
//                                    if (row != col)
//                                    {
//                                        rowIds.Add(col);
//                                        colIds.Add(row);
//                                        values.Add(Ke[Dof * I + p, Dof * J + q]);
//                                    }
//                                }
//            }

//            // CoordinateStorage = triplet storage
//            var coo = new CoordinateStorage<float>(Dim, Dim, rowIds.ToArray(),colIds.ToArray(),values.ToArray());
//            KG = (SparseMatrix)SparseMatrix.OfIndexed(coo);
//        }
//        public void Solve()
//        {
//            float[] F0 = new float[Dim];
//            float[] F1 = new float[Dim];

//            float[] X0 = new float[Dim];
//            float[] X1 = new float[Dim];

//            foreach (var item in Model.Loads)
//            {
//                F0[item.NodeID * Dof] = item.Load.X;
//                F1[item.NodeID * Dof] = item.Load.Y;
//            }

//            var pardiso = new Pardiso(KG, PardisoMatrixType.RealSymmetricPositiveDefinite);

//            pardiso.Solve(F0, X0);
//            pardiso.Solve(F1, X1);

//            int id = 0;

//            foreach (var item in Model.Nodes)
//            {
//                if (item.Anchored != true)
//                {
//                    item.Displacement = new Vector2D(X0[id], X1[id]);
//                    id++;
//                }
//            }
//        }
//        private static List<int> ApplySupports2D(List<Node2D> nodes, List<Support2D> supports)
//        {
//            List<int> ids = new List<int>(supports.Count);
//            for (int i = 0; i < supports.Count; i++)
//            {
//                int id = supports[i].NodeID;
//                ids.Add(id);
//                nodes[id].Anchored = true;
//                if (supports[i].Type == SupportType.Fixed)
//                {
//                    nodes[id].Displacement.X = 0.0;
//                    nodes[id].Displacement.Y = 0.0;
//                }
//            }
//            return ids;
//        }
//    }
//}

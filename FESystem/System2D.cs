using ALFE.FEModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ALFE.FESystem
{
    public class System2D
    {
        /// <summary>
        /// Time cost in each step: 0 = Computing Ke, 1 = Assembling KG, 2 = Solving
        /// </summary>
        public List<double> TimeCost = new List<double>(3);

        /// <summary>
        /// Finite element model
        /// </summary>
        public Model2D Model;

        /// <summary>
        /// Dimensions of the global stiffness matrix
        /// </summary>
        public int Dim;

        /// <summary>
        /// Degree of freedom
        /// </summary>
        public int DOF = 2;

        /// <summary>
        /// If all elements are the same, It should be set True for reducing workload.
        /// </summary>
        public bool Unify = false;

        /// <summary>
        /// An index list of anchored nodes.
        /// </summary>
        private List<int> FixedID;

        /// <summary>
        /// Global stiffness matrix
        /// </summary>
        private CooMatrix KG;

        /// <summary>
        /// Force vector
        /// </summary>
        private float[] F;

        /// <summary>
        /// Displacement vector
        /// </summary>
        private float[] X;

        /// <summary>
        /// Initialize the finite element system.
        /// </summary>
        /// <param name="model"> A finite element model</param>
        public System2D(Model2D model, bool unify = false)
        {
            Model = model;
            Unify = unify;

            ApplySupports2D();
            Dim = (Model.Nodes.Count - FixedID.Count) * DOF;

            F = new float[Dim * DOF];
            X = new float[Dim * DOF];
        }

        /// <summary>
        /// Assemble the force vector.
        /// </summary>
        private void AssembleF()
        {
            for (int i = 0; i < Model.Loads.Count; i++)
            {
                F[Model.Loads[i].NodeID * DOF + 0] = Model.Loads[i].Load.X;
                F[Model.Loads[i].NodeID * DOF + 1] = Model.Loads[i].Load.Y;
            }
        }

        /// <summary>
        /// Assemble the global stiffness matrix
        /// </summary>
        private void AssembleKG()
        {
            var scan = Utils.Scan(Model.Nodes.Count, FixedID);
            HashSet<Triplet> triplets = new HashSet<Triplet>(Dim);

            for (int i = 0; i < Model.Elements.Count; i++)
            {
                var nodeID = Model.Elements[i].NodeID;
                for (int I = 0; I < nodeID.Count; I++)
                    for (int J = 0; J <= I; J++)
                        if (scan[nodeID[I]][1] == 1 && scan[nodeID[J]][1] == 1)
                            for (int p = 0; p < DOF; p++)
                                for (int q = 0; q < DOF; q++)
                                {
                                    var row = (nodeID[I] - scan[nodeID[I]][0]) * DOF + p;
                                    var col = (nodeID[J] - scan[nodeID[J]][0]) * DOF + q;
                                    if (row >= col)
                                        triplets.Add(new Triplet(col, row, Model.Elements[i].Ke[DOF * I + p, DOF * J + q]));
                                }
            }

            KG = new CooMatrix(triplets.ToList(), Dim, Dim);
        }

        /// <summary>
        /// Assemble the global stiffness matrix
        /// </summary>
        /// <param name="Ke">Input an elementary stiffness matrix.</param>
        private void AssembleKG(float[,] Ke)
        {
            var scan = Utils.Scan(Model.Nodes.Count, FixedID);
            HashSet<Triplet> triplets = new HashSet<Triplet>(Dim);

            for (int i = 0; i < Model.Elements.Count; i++)
            {
                var nodeID = Model.Elements[i].NodeID;
                for (int I = 0; I < nodeID.Count; I++)
                    for (int J = 0; J <= I; J++)
                        if (scan[nodeID[I]][1] == 1 && scan[nodeID[J]][1] == 1)
                            for (int p = 0; p < DOF; p++)
                                for (int q = 0; q < DOF; q++)
                                {
                                    var row = (nodeID[I] - scan[nodeID[I]][0]) * DOF + p;
                                    var col = (nodeID[J] - scan[nodeID[J]][0]) * DOF + q;
                                    if (row >= col)
                                        triplets.Add(new Triplet(col, row, Ke[DOF * I + p, DOF * J + q]));
                                }
            }

            KG = new CooMatrix(triplets.ToList(), Dim, Dim);
        }

        /// <summary>
        /// Solve the finite element system. You can get the displacement vector after running this function.
        /// </summary>
        public void Solve()
        {
            Stopwatch sw = new Stopwatch();

            if (Unify == true)
            {
                sw.Start();
                var Ke = Model.ComputeUniformK();
                sw.Stop();
                TimeCost.Add(sw.Elapsed.TotalMilliseconds);

                sw.Restart();
                AssembleKG(Ke);
                sw.Stop();
                TimeCost.Add(sw.Elapsed.TotalMilliseconds);
            }
            else
            {
                sw.Start();
                ComputeAllKe();
                sw.Stop();
                TimeCost.Add(sw.Elapsed.TotalMilliseconds);

                sw.Restart();
                AssembleKG();
                sw.Stop();
                TimeCost.Add(sw.Elapsed.TotalMilliseconds);
            }

            AssembleF();

            sw.Restart();
            SolveFE(KG.RowArray, KG.ColArray, KG.ValueArray, F, Dim, DOF, KG.NNZ, X);
            sw.Stop();
            TimeCost.Add(sw.Elapsed.TotalMilliseconds);

            int id = 0;

            foreach (var item in Model.Nodes)
            {
                if (item.Anchored != true)
                {
                    item.Displacement = new Vector2D(X[id * DOF + 0], X[id * DOF + 1]);
                    id++;
                }
            }
        }

        /// <summary>
        /// Get the global stiffness matrix.
        /// </summary>
        /// <returns> Return the global stiffness matrix.</returns>
        public CooMatrix getKG()
        {
            return KG;
        }

        /// <summary>
        /// Get the displacement vector.
        /// </summary>
        /// <returns> Return the displacement vector.</returns>
        public double[,] getDisplacement()
        {
            double[,] displacement = new double[Dim, DOF];
            for (int i = 0; i < Model.Nodes.Count; i++)
            {
                displacement[i, 0] = Model.Nodes[i].Displacement.X;
                displacement[i, 1] = Model.Nodes[i].Displacement.Y;
            }
            return displacement;
        }

        /// <summary>
        /// Print the time cost in each part.
        /// </summary>
        public void PrintTime()
        {
            FEPrint.PrintTimeCost(TimeCost);
        }

        /// <summary>
        /// Apply the boundary conditions to nodes.
        /// </summary>
        /// <returns></returns>
        private void ApplySupports2D()
        {
            var nodes = Model.Nodes;
            var supports = Model.Supports;

            List<int> ids = new List<int>(supports.Count);
            for (int i = 0; i < supports.Count; i++)
            {
                int id = supports[i].NodeID;
                ids.Add(id);
                nodes[id].Anchored = true;
                if (supports[i].Type == SupportType.Fixed)
                {
                    nodes[id].Displacement.X = 0.0f;
                    nodes[id].Displacement.Y = 0.0f;
                }
            }
            FixedID = ids;
        }

        /// <summary>
        /// Compute all elementary stiffness matrices.
        /// </summary>
        private void ComputeAllKe()
        {
            Parallel.For(0, Model.Elements.Count, i =>
            {
                Model.Elements[i].ComputeK();
            });
        }

        [DllImport("ALSolver.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, SetLastError = false)]
        private static extern void SolveFE(int[] rowA, int[] colA, float[] valA, float[] F, int dim, int dof, int nnzA, float[] X);

    }
}

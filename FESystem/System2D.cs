using ALFE.FEModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Single;
using System.Threading;

namespace ALFE.FESystem
{
    public class System2D
    {
        /// <summary>
        /// Time cost in each step: 0 = Computing Ke, 1 = Assembling KG, 2 = Solving
        /// </summary>
        public List<double> TimeCost = new List<double>(3);

        /// <summary>
        /// Active nodes
        /// </summary>
        public List<Node> ActiveNodes = new List<Node>();

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
        /// If the system has been solved.
        /// </summary>
        public bool Solved = false;

        /// <summary>
        /// An index list of anchored nodes.
        /// </summary>
        private List<int> FixedID;

        /// <summary>
        /// Global stiffness matrix
        /// </summary>
        private CSRMatrix KG;

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

            F = new float[Dim];
            X = new float[Dim];
        }

        /// <summary>
        /// Assemble the force vector.
        /// </summary>
        private void AssembleF()
        {
            foreach (var item in Model.Loads)
            {
                var id = Model.Nodes[item.NodeID].ActiveID * DOF;
                F[id + 0] = item.ForceVector.X;
                F[id + 1] = item.ForceVector.Y;
            }
        }

        /// <summary>
        /// Assemble the global stiffness matrix
        /// </summary>
        private void AssembleKG()
        {
            InitialzeKG();
            KG.Clear();

            foreach (var elem in Model.Elements)
            {
                //var elem = Model.Elements[e];
                for (int i = 0; i < elem.NodeID.Count; i++)
                {
                    Node ni = Model.Nodes[elem.NodeID[i]];
                    if (ni.Active)
                    {
                        for (int j = 0; j < elem.NodeID.Count; j++)
                        {
                            Node nj = Model.Nodes[elem.NodeID[j]];

                            if (nj.Active)
                            {
                                // write the corresponding 2x2 fragment to CSR
                                int idx1 = ni.PositionKG[nj.ActiveID]; // there is a room for optimization here
                                for (int m = 0; m < 2; m++) for (int n = 0; n < 2; n++)
                                        KG.Vals[idx1 + ni.row_nnz * n + m] += elem.Ke[i * 2 + n, j * 2 + m];
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Assemble the global stiffness matrix
        /// </summary>
        /// <param name="Ke">Input an elementary stiffness matrix.</param>
        private void AssembleKG(Matrix Ke)
        {
            InitialzeKG();
            KG.Clear();

            foreach (var elem in Model.Elements)
            {
                //var elem = Model.Elements[e];
                for (int i = 0; i < elem.NodeID.Count; i++)
                {
                    Node ni = Model.Nodes[elem.NodeID[i]];
                    if (ni.Active)
                    {
                        for (int j = 0; j < elem.NodeID.Count; j++)
                        {
                            Node nj = Model.Nodes[elem.NodeID[j]];

                            if (nj.Active)
                            {
                                // write the corresponding 2x2 fragment to CSR
                                int idx1 = ni.PositionKG[nj.ActiveID]; // there is a room for optimization here
                                for (int m = 0; m < 2; m++)
                                    for (int n = 0; n < 2; n++)
                                        KG.Vals[idx1 + ni.row_nnz * n + m] += Ke[i * 2 + n, j * 2 + m];

                            }
                        }
                    }
                }
            }
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
                var Ke = ComputeUniformK();
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
            Solved = SolveFE(KG.Rows, KG.Cols, KG.Vals, F, Dim, DOF, KG.NNZ, X) == 1 ? true : false;
            sw.Stop();
            TimeCost.Add(sw.Elapsed.TotalMilliseconds);

            int id = 0;

            foreach (var item in Model.Nodes)
            {
                if (item.Active == true)
                {
                    item.Displacement = new Vector3D(X[id * DOF + 0], X[id * DOF + 1], 0.0f);
                    id++;
                }
            }
        }

        /// <summary>
        /// Get the global stiffness matrix.
        /// </summary>
        /// <returns> Return the global stiffness matrix.</returns>
        public CSRMatrix GetKG() { return KG; }

        /// <summary>
        /// Get the displacement vector.
        /// </summary>
        /// <returns> Return the displacement vector.</returns>
        public float[,] GetDisplacement()
        {
            float[,] displacement = new float[Model.Nodes.Count, DOF];
            for (int i = 0; i < Model.Nodes.Count; i++)
            {
                displacement[i, 0] = Model.Nodes[i].Displacement.X;
                displacement[i, 1] = Model.Nodes[i].Displacement.Y;
            }
            return displacement;
        }

        /// <summary>
        /// Initialize the global stiffness matrix
        /// </summary>
        public void InitialzeKG()
        {
            // list non-anchored nodes and give them sequential ids
            ActiveNodes = Model.Nodes.FindAll(nd => nd.Active);
            int id = 0;
            foreach (Node nd in ActiveNodes) { nd.ActiveID = id++; }

            GetConnectedElements();
            GetAdjacentNodes();

            // count total number of neighbours in all nodes 
            int count = 0;
            foreach (Node nd in ActiveNodes) count += nd.Neighbours.Count;

            // allocate CSR
            // each neighbor contributes 2 rows and 2 columns to CSR matrix, so NNZ = count * 4
            // the size of the matrix is (number of active nodes)*(2 coordinates)
            KG = new CSRMatrix(ActiveNodes.Count * 2, count * 4);

            // 3) create CSR indices
            count = 0;
            foreach (Node nd in ActiveNodes)
            {
                int row_nnz = (nd as Node).ComputePositionInKG(count, KG.Cols);
                KG.Rows[nd.ActiveID * 2 + 0] = count;
                KG.Rows[nd.ActiveID * 2 + 1] = count + row_nnz;
                count += row_nnz * 2;
            }
        }

        /// <summary>
        /// Get the neighbours of each node.
        /// </summary>
        private void GetAdjacentNodes()
        {
            Parallel.ForEach(Model.Nodes, node =>
            {
                foreach (var item in node.ElementID)
                    foreach (var neighbour in Model.Elements[item].NodeID)
                        if (Model.Nodes[neighbour].Active)
                            lock (node.Neighbours)
                                node.Neighbours.Add(Model.Nodes[neighbour].ActiveID);
            });
        }

        /// <summary>
        /// Get the connected elements of each node.
        /// </summary>
        private void GetConnectedElements()
        {
            // in each node make a list of elements to which it belongs
            for (int i = 0; i < Model.Elements.Count; i++)
            {
                foreach (int nd in Model.Elements[i].NodeID)
                    Model.Nodes[nd].ElementID.Add(i);
            }
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
                nodes[id].Active = false;
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
            foreach (var item in Model.Elements)
                item.ComputeKe();
        }

        /// <summary>
        /// Compute the uniform elementary stiffness matrix.
        /// </summary>
        /// <returns></returns>
        public Matrix ComputeUniformK()
        {
            var ele = Model.Elements[0];
            ele.ComputeKe();
            return ele.Ke;
        }


        [DllImport("ALSolver.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, SetLastError = false)]
        private static extern int SolveFE(int[] rows_offset, int[] cols, float[] vals, float[] F, int dim, int dof, int nnz, float[] X);
    }
}
